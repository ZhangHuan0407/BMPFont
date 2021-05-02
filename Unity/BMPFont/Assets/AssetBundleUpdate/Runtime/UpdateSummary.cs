using ICSharpCode.SharpZipLib.GZip;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AssetBundleUpdate
{
    /// <summary>
    /// 一个版本的更新摘要文件，此文件每次启动重新下载不做保存
    /// </summary>
    [Serializable]
    public class UpdateSummery
    {
        /* const */
        /// <summary>
        /// <see cref="UpdateSummery"/> 没有保存到本地，导致更换资源服务器变得极其困难
        /// 每次获取到最新版本的 <see cref="UpdateSummery"/> 后，会记录 Uri 到本地，下次用上次记录的 Uri 获取 <see cref="UpdateSummery"/>。
        /// 可以通过这个定向机制，让两个资源服务器地址并行一段时间，直至旧的 APK 消亡殆尽，也可以用这个机制进行流量分流。(不推荐)
        /// </summary>
        [JsonIgnore]
        public const string UpdateSummeryUriBaseKey = "UpdateSummery.UriBase";
        /// <summary>
        /// 上一次更新时拿到的目标版本。因为玩家有修改数据的嫌疑，不完全可靠。
        /// </summary>
        [JsonIgnore]
        public const string UpdateSummeryVersionKey = "UpdateSummery.Version";

        /* field */
        [SerializeField]
        private string m_IPV4UriBase;
        public string IPV4UriBase
        {
            get => m_IPV4UriBase;
            set => m_IPV4UriBase = value;
        }

        [SerializeField]
        private string m_IPV6UriBase;
        public string IPV6UriBase
        {
            get => m_IPV6UriBase;
            set => m_IPV6UriBase = value;
        }

        [SerializeField]
        private int m_Version;
        public int Version
        {
            get => m_Version;
            set => m_Version = value;
        }

        [SerializeField]
        private AssetBundleInfo[] m_RecordInfos;
        public AssetBundleInfo[] RecordInfos
        {
            get => m_RecordInfos;
            set => m_RecordInfos = value;
        }

        [SerializeField]
        private int m_UpdateAssetBundlesCount;
        public int UpdateAssetBundlesCount
        {
            get => m_UpdateAssetBundlesCount;
            set => m_UpdateAssetBundlesCount = value;
        }

        [SerializeField]
        private int m_UpdateFilesLength;
        public int UpdateFilesLength
        {
            get => m_UpdateFilesLength;
            set => m_UpdateFilesLength = value;
        }

        /// <summary>
        /// 强制储存包集合，在强制更新期间结束后开始下载
        /// </summary>
        public HashSet<string> ForceReserveAssetBundles;
        /// <summary>
        /// 强制更新包集合，在强制更新期间强制下载直至完成
        /// </summary>
        public HashSet<string> ForceUpdateAssetBundles;

        /* inter */

        /* ctor */
        public UpdateSummery() { }
        public static UpdateSummery DeserializeFromGZip(byte[] data)
        {
            using (MemoryStream inputStream = new MemoryStream(data))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    GZip.Decompress(inputStream, memoryStream, false);
                    memoryStream.Position = 0;
                    byte[] buffer = new byte[memoryStream.Length];
                    memoryStream.Read(buffer, 0, (int)memoryStream.Length);
                    return JsonConvert.DeserializeObject<UpdateSummery>(Encoding.UTF8.GetString(buffer));
                }
            }
        }

        /* func */
        /// <summary>
        /// 异步下载更新摘要压缩包
        /// </summary>
        /// <returns>下载任务，如果下载失败则重新下载直至成功</returns>
        public static Task<byte[]> DownloadUpdateSummery()
        {
            return Task.Run(async () =>
            {
                byte[] data = null;
                try
                {
                    StartToDownload:
                    Task<HttpResponseMessage> downloadTask = AssetBundleUpdateTask.HttpClient.Value.GetAsync($"UpdateSummery.gz");
                    data = downloadTask.Result.Content.ReadAsByteArrayAsync().Result;

                    if (!downloadTask.Result.IsSuccessStatusCode)
                    {
                        Debug.LogError("Download UpdateSummery.gz failed.");
                        await Task.Delay(5000);
                        goto StartToDownload;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                return data;
            });
        }

        public string GetUriBase()
        {
            string UriBase = null;
            foreach (IPAddress iPAddress in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                switch (iPAddress.AddressFamily)
                {
                    case AddressFamily.InterNetwork:
                        UriBase = IPV4UriBase;
                        break;
                    case AddressFamily.InterNetworkV6:
                        UriBase = IPV6UriBase;
                        break;
                    default:
                        break;
                }
            }
            UriBase = UriBase ?? IPV4UriBase;
            return UriBase;
        }

        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}