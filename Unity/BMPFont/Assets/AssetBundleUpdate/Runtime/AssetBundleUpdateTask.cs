using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace AssetBundleUpdate
{
    /// <summary>
    /// 提供资源包(AssetBundle)本地资源MD5核对、异步下载、失败重下的支持
    /// </summary>
    public class AssetBundleUpdateTask
    {
        /* const */

        /* field */
        /// <summary>
        /// 资源下载 Http 客户端
        /// </summary>
        public static Lazy<HttpClient> HttpClient = new Lazy<HttpClient>(() => new HttpClient(), false);
        internal static MD5CryptoServiceProvider MD5Provider = new MD5CryptoServiceProvider();

        /// <summary>
        /// 弃用标记，AssetBundleUpdateTask 会在下一个任务执行点提前终止任务
        /// </summary>
        public bool Abort { get; set; }
        /// <summary>
        /// 检查指向的 AssetBundle 是否具有一致的 MD5，否则开始下载
        /// </summary>
        public bool CheckAsset { get; set; }
        /// <summary>
        /// 指向的 AssetBundle 必然需要下载更新资源，直接开始下载任务
        /// </summary>
        public bool DownloadFromServer { get; set; }

        public readonly AssetBundleInfo AssetBundleInfo;
        public Task Task { get; private set; }
        public CancellationToken CancellationToken { get; private set; }

        /// <summary>
        /// 完成更新任务或者任务取消后回调
        /// </summary>
        public event Action AfterUpdate_Handle;

        /* inter */

        /* ctor */
        public AssetBundleUpdateTask(AssetBundleInfo assetBundleInfo)
        {
            Abort = false;
            CheckAsset = false;
            DownloadFromServer = false;
            AssetBundleInfo = assetBundleInfo ?? throw new ArgumentNullException(nameof(assetBundleInfo));
        }

        /* func */
        public void RunInTask()
        {
            if (CancellationToken != default)
                throw new ArgumentException("Have another task in back?");

            CancellationToken = new CancellationToken();
            Task.Run(async () =>
            {
                if (DownloadFromServer)
                    goto BeforeDownloadFromServer;
                else if (CheckAsset)
                    goto BeforeCheckAsset;
                else if (Abort)
                    goto AfterUpdate;
                else
                    goto AfterUpdate;

            BeforeDownloadFromServer:
                try
                {
                    if (Abort)
                        goto AfterUpdate;
                    // 请求头加入 Range，可以实现断点续传 + 下载速度监控
                    HttpResponseMessage message = HttpClient.Value.GetAsync(AssetBundleInfo.AssetBundleDownloadPath).Result;
                    if (Abort)
                        goto AfterUpdate;
                    if (!message.IsSuccessStatusCode)
                    {
                        Debug.LogError($"Download AssetBundle failed, {nameof(AssetBundleInfo.AssetBundleDownloadPath)} : {AssetBundleInfo.AssetBundleDownloadPath}");
                        await Task.Delay(5000);
                        goto BeforeDownloadFromServer;
                    }

                    byte[] buffer = message.Content.ReadAsByteArrayAsync().Result;
                    Directory.CreateDirectory(Path.GetDirectoryName(AssetBundleInfo.TempFilePath));
                    File.WriteAllBytes(AssetBundleInfo.TempFilePath, buffer);
                    if (Abort)
                        goto AfterUpdate;

                    // CheckUpdateAssetBundle:
                    string downloadMD5 = BitConverter.ToString(MD5Provider.ComputeHash(buffer)).Replace("-", string.Empty).ToUpper();
                    if (!downloadMD5.Equals(AssetBundleInfo.MD5))
                        goto NeedUpdate;
                    else if (Abort)
                        goto AfterUpdate;
                    Directory.CreateDirectory(Path.GetDirectoryName(AssetBundleInfo.FilePath));
                    File.Move(AssetBundleInfo.TempFilePath, AssetBundleInfo.FilePath);
                    goto AfterUpdate;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    goto NeedUpdate;
                }

            BeforeCheckAsset:
                FileInfo fileInfo = new FileInfo(AssetBundleInfo.FilePath);
                if (!File.Exists(fileInfo.FullName)
                    || fileInfo.Length != AssetBundleInfo.FileLength)
                    goto NeedUpdate;

                byte[] fileBytes = File.ReadAllBytes(fileInfo.FullName);
                string fileMD5 = BitConverter.ToString(MD5Provider.ComputeHash(fileBytes)).Replace("-", string.Empty).ToUpper();
                if (fileMD5.Equals(AssetBundleInfo.MD5))
                    goto AfterUpdate;
                else if (Abort)
                    goto AfterUpdate;
                else
                    goto NeedUpdate;

            NeedUpdate:
                if (File.Exists(AssetBundleInfo.TempFilePath))
                    File.Delete(AssetBundleInfo.TempFilePath);
                DownloadFromServer = true;
                await Task.Delay(50);
                if (Abort)
                    goto AfterUpdate;
                else
                    goto BeforeDownloadFromServer;

            AfterUpdate:
#if UNITY_EDITOR
                Debug.Log($"Update AssetBundle successful, {nameof(CheckAsset)} : {CheckAsset}, {nameof(DownloadFromServer)} : {DownloadFromServer}\n{AssetBundleInfo.FilePath}");
#endif
                AfterUpdate_Handle?.Invoke();
                return;
            }, CancellationToken);
        }
    }
}