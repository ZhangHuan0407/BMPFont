using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

namespace AssetBundleUpdate
{
    /// <summary>
    /// 描述一个更新资源包的详细信息，这些信息在打包完成后记录更新
    /// </summary>
    [Serializable]
    public class AssetBundleInfo
    {
        /* const */
        private static string m_AssetBundleDirectory;
        /// <summary>
        /// 游戏运行时，资源包存储根文件夹
        /// </summary>
        [JsonIgnore]
        public static string AssetBundleDirectory => m_AssetBundleDirectory = m_AssetBundleDirectory ?? $"{Application.persistentDataPath}/AssetBundle";
        private static string m_AssetBundleTempDirectory;
        /// <summary>
        /// 游戏运行时，下载缓存根文件夹
        /// </summary>
        [JsonIgnore]
        public static string AssetBundleTempDirectory => m_AssetBundleTempDirectory = m_AssetBundleTempDirectory ?? $"{Application.persistentDataPath}/AssetBundleTemp";

        /* field */
        [SerializeField]
        private string m_Name;
        /// <summary>
        /// 资源包完整名称，例如: "updatablelogic/hotfix.assetbundle" 小写锁定，以 .assetbundle 作为扩展名
        /// </summary>
        public string Name
        {
            get => m_Name;
            set => m_Name = value;
        }

        [SerializeField]
        private int m_Version;
        /// <summary>
        /// 资源包跟随版本号
        /// </summary>
        public int Version
        {
            get => m_Version;
            set => m_Version = value;
        }

        [SerializeField]
        private string m_MD5;
        /// <summary>
        /// 资源包 MD5 结果，锁定大写去除'-'
        /// </summary>
        public string MD5
        {
            get => m_MD5;
            set => m_MD5 = value;
        }

        [SerializeField]
        private int m_FileLength;
        /// <summary>
        /// 资源包文件大小，单位 byte
        /// </summary>
        public int FileLength
        {
            get => m_FileLength;
            set => m_FileLength = value;
        }

        [SerializeField]
        private DateTime m_LastCreateTime;
        /// <summary>
        /// 最后一次打包时间
        /// </summary>
        [JsonIgnore]
        public DateTime LastCreateTime 
        { 
            get => m_LastCreateTime; 
            set => m_LastCreateTime = value;
        }

        [SerializeField]
        private string[] m_AllDependencies;
        /// <summary>
        /// 列举当前资源包依赖的所有资源包
        /// </summary>
        public string[] AllDependencies
        {
            get => m_AllDependencies;
            set => m_AllDependencies = value;
        }

        [SerializeField]
        private string[] m_DirectDependencies;
        [JsonIgnore]
        public string[] DirectDependencies
        {
            get => m_DirectDependencies;
            set => m_DirectDependencies = value;
        }

        /// <summary>
        /// 运行时计算的资源包直接引用计数
        /// </summary>
        public int DirectReferenceCount { get; internal set; }
        /// <summary>
        /// 运行时计算的资源包总计(直接+被动)引用计数
        /// </summary>
        public int AllReferenceCount { get; internal set; }

        [JsonIgnore]
        public bool HaveUpdate { get; internal set; }
        [JsonIgnore]
        public bool HaveLoaded { get; internal set; }

        /* inter */
        /// <summary>
        /// 资源服务器资源包下载相对路径
        /// </summary>
        [JsonIgnore]
        public string AssetBundleDownloadPath => $"{Version}/{Name}";
        /// <summary>
        /// 本地外存存储文件夹路径
        /// </summary>
        [JsonIgnore]
        public string FilePath => $"{AssetBundleDirectory}/{Version}/{Name}";
        /// <summary>
        /// 本地外存缓存文件夹路径
        /// </summary>
        [JsonIgnore]
        public string TempFilePath => $"{AssetBundleTempDirectory}/{Name}";

        /* ctor */
        public AssetBundleInfo()
        {
            Name = string.Empty;
            Version = 0;
            MD5 = string.Empty;
            FileLength = 0;
            LastCreateTime = new DateTime();
            AllDependencies = new string[0];
            DirectDependencies = new string[0];
            DirectReferenceCount = 0;
            AllReferenceCount = 0;
            HaveUpdate = false;
            HaveLoaded = false;
        }

        /* func */
        /// <summary>
        /// 从打包清单中获取当前资源包的摘要信息
        /// </summary>
        /// <param name="assetBundleManifest">打包清单</param>
        public void UpdateFromManifest(AssetBundleManifest assetBundleManifest)
        {
            LastCreateTime = DateTime.Now;
            AllDependencies = assetBundleManifest.GetAllDependencies(Name);
            DirectDependencies = assetBundleManifest.GetDirectDependencies(Name);
        }
        /// <summary>
        /// 从生成资源包中获取当前资源包的摘要信息
        /// </summary>
        /// <param name="fileInfo">生成资源包文件</param>
        public void UpdateFromFile(FileInfo fileInfo)
        {
            if (!File.Exists(fileInfo.FullName))
                throw new FileNotFoundException($"Not found assetbundle at path : {fileInfo.FullName}");

            if (fileInfo.Length > 20 * 1024 * 1024)
                throw new Exception("AssetBundle is too huge");
            FileLength = (int)fileInfo.Length;

            MD5CryptoServiceProvider MD5Provider = new MD5CryptoServiceProvider();
            byte[] buffer = File.ReadAllBytes(fileInfo.FullName);
            MD5 = BitConverter.ToString(MD5Provider.ComputeHash(buffer)).Replace("-", string.Empty).ToUpper();
        }

        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);
        public override int GetHashCode() => Name.GetHashCode();
    }
}