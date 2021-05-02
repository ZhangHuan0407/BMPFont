using ICSharpCode.SharpZipLib.GZip;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AssetBundleUpdate
{
    [Serializable]
    public class AssetBundleConfig : ScriptableObject
    {
        /* const */
        public const string AssetPath = "../AssetBundles";

        /* field */
        [SerializeField]
        private string m_ConfigName;
        public string ConfigName
        {
            get => m_ConfigName;
            set => m_ConfigName = value;
        }
        public string LastVersionRecordInfosFile;

        [SerializeField]
        private List<AssetBundleInfo> m_RecordInfos;
        public List<AssetBundleInfo> RecordInfos
        { 
            get => m_RecordInfos; 
            set => m_RecordInfos = value;
        }

        [Tooltip("这次打包使用的资源包版本号，打包结束后自动增长")]
        [SerializeField]
        private int m_Version;
        public int Version
        { 
            get => m_Version; 
            set => m_Version = value;
        }

        [Tooltip("资源下载时的前缀 Uri 地址，http://127.0.0.1/")]
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
        private BuildAssetBundleOptions m_BuildAssetBundleOptions;
        public BuildAssetBundleOptions BuildAssetBundleOptions
        { 
            get => m_BuildAssetBundleOptions; 
            set => m_BuildAssetBundleOptions = value;
        }

        [SerializeField]
        private BuildTarget m_BuildTarget;
        public BuildTarget BuildTarget
        {
            get => m_BuildTarget;
            set => m_BuildTarget = value;
        }

        [SerializeField]
        private List<string> m_ForceReserveAssetBundles;
        public List<string> ForceReserveAssetBundles
        {
            get => m_ForceReserveAssetBundles;
            set => m_ForceReserveAssetBundles = value;
        }

        [Tooltip("强制更新包，在更新阶段立即阻塞下载")]
        [SerializeField]
        private List<string> m_ForceUpdateAssetBundles;
        public List<string> ForceUpdateAssetBundles
        {
            get => m_ForceUpdateAssetBundles;
            set => m_ForceUpdateAssetBundles = value;
        }

        /* inter */
        public string OutputDirectory => $"{AssetPath}/{BuildTarget}/{ConfigName}";
        [JsonIgnore]
        public bool CanCreateAssetBundle
        {
            get
            {
                return RecordInfos.Count > 0
                    && Version > 0
                    && BuildTarget != BuildTarget.NoTarget;
            }
        }

        public int UpdateAssetBundlesCount
        {
            get
            {
                int count = 0;
                foreach (AssetBundleInfo assetBundleInfo in RecordInfos)
                    count += Version == assetBundleInfo.Version ? 1 : 0;
                return count;
            }
        }
        public int UpdateFilesLength
        {
            get
            {
                int length = 0;
                foreach (AssetBundleInfo assetBundleInfo in RecordInfos)
                    length += Version == assetBundleInfo.Version ? assetBundleInfo.FileLength : 0;
                return length;
            }
        }

        /* ctor */
        public AssetBundleConfig()
        {
            ConfigName = string.Empty;
            LastVersionRecordInfosFile = string.Empty;
            RecordInfos = new List<AssetBundleInfo>();
            Version = 0;
            BuildAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression;
            BuildTarget = BuildTarget.NoTarget;
        }
        public static AssetBundleConfig ParseJson(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentException($"“{nameof(data)}”不能为 Null 或空白", nameof(data));
            return JsonConvert.DeserializeObject<AssetBundleConfig>(data);
        }

        /* func */
        public void GetParameters()
        {
            RecordInfos.Clear();
            AssetDatabase.RemoveUnusedAssetBundleNames();
            foreach (string abName in AssetDatabase.GetAllAssetBundleNames())
                RecordInfos.Add(new AssetBundleInfo() 
                {
                    Name = abName,
                });
        }
        public void CreateAssetBundles()
        {
            if (!CanCreateAssetBundle)
                throw new ArgumentException("The asset bundle could not be created. Please check the parameters.");
            Directory.CreateDirectory($"{OutputDirectory}/AssetBundles");
            AssetBundleManifest assetBundleManifest = BuildPipeline.BuildAssetBundles($"{OutputDirectory}/AssetBundles", BuildAssetBundleOptions, BuildTarget);
            Debug.Log("Create Asset Bundles finish.");

            RecordInfos.Clear();
            foreach (string abName in assetBundleManifest.GetAllAssetBundles())
            {
                AssetBundleInfo assetBundleInfo = new AssetBundleInfo()
                {
                    Name = abName,
                };
                RecordInfos.Add(assetBundleInfo);
                assetBundleInfo.UpdateFromManifest(assetBundleManifest);
                assetBundleInfo.UpdateFromFile(new FileInfo($"{OutputDirectory}/AssetBundles/{assetBundleInfo.Name}"));
            }

            Directory.CreateDirectory($"{OutputDirectory}/History");
            Directory.CreateDirectory($"{OutputDirectory}/Upload/{Version}");

            AssetBundleInfo[] lastVersionRecordInfos = new AssetBundleInfo[0];
            if (File.Exists(LastVersionRecordInfosFile))
                lastVersionRecordInfos = JsonConvert.DeserializeObject<UpdateSummery>(File.ReadAllText(LastVersionRecordInfosFile)).RecordInfos ?? new AssetBundleInfo[0];
            RedirectAssetBundle(lastVersionRecordInfos, RecordInfos);

            string configString = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText($"{OutputDirectory}/History/AssetBundleConfig_{Version}.json", configString, Encoding.UTF8);
            UpdateSummery updateSummery = GetUpdateSummery(); 
            LastVersionRecordInfosFile = $"{OutputDirectory}/History/UpdateSummery_{Version}.json";
            File.WriteAllText(LastVersionRecordInfosFile, JsonConvert.SerializeObject(updateSummery));
            WriteGZipFile(LastVersionRecordInfosFile, $"{OutputDirectory}/Upload/UpdateSummery.gz");

            foreach (AssetBundleInfo assetBundleInfo in RecordInfos)
            {
                if (assetBundleInfo.Version == Version)
                {
                    string destFilePath = $"{OutputDirectory}/Upload/{Version}/{assetBundleInfo.Name}";
                    Directory.CreateDirectory(Path.GetDirectoryName(destFilePath));
                    File.Copy($"{OutputDirectory}/AssetBundles/{assetBundleInfo.Name}", destFilePath, true);
                }
            }
            Version++;
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            Debug.Log("Update Upload files finish.");

            GC.Collect();
            AssetDatabase.Refresh();
        }

        public UpdateSummery GetUpdateSummery()
        {
            HashSet<string> assetBunbleNames = new HashSet<string>(
                from assetBundleInfo in RecordInfos select assetBundleInfo.Name);

            List<string> buffer = new List<string>();
            foreach (string assetBundleName in ForceReserveAssetBundles)
            {
                if (assetBundleName is null)
                    continue;
                else if (assetBunbleNames.Contains(assetBundleName))
                    buffer.Add(assetBundleName);
                else
                    Debug.LogWarning($"AssetBundle : {assetBundleName} is not found in {nameof(RecordInfos)}, remove it from {nameof(ForceReserveAssetBundles)}.");
            }
            foreach (string assetBundleName in ForceUpdateAssetBundles)
            {
                if (assetBundleName is null)
                    continue;
                else if (assetBunbleNames.Contains(assetBundleName))
                    buffer.Add(assetBundleName);
                else
                    Debug.LogWarning($"AssetBundle : {assetBundleName} is not found in {nameof(RecordInfos)}, remove it from {nameof(ForceUpdateAssetBundles)}.");
            }

            return new UpdateSummery()
            {
                Version = Version,
                IPV4UriBase = IPV4UriBase,
                IPV6UriBase = IPV6UriBase,
                RecordInfos = RecordInfos.ToArray(),
                UpdateAssetBundlesCount = UpdateAssetBundlesCount,
                UpdateFilesLength = UpdateFilesLength,
                ForceReserveAssetBundles = new HashSet<string>(ForceReserveAssetBundles),
                ForceUpdateAssetBundles = new HashSet<string>(ForceUpdateAssetBundles),
            };
        }

        private void RedirectAssetBundle(IEnumerable<AssetBundleInfo> lastVersionInfos, IEnumerable<AssetBundleInfo> newVersionInfos)
        {
            Dictionary<string, AssetBundleInfo> lastVersionInfosDictionary = new Dictionary<string, AssetBundleInfo>();
            foreach (AssetBundleInfo assetBundleInfo in lastVersionInfos)
                lastVersionInfosDictionary.Add(assetBundleInfo.Name, assetBundleInfo);

            foreach (AssetBundleInfo assetBundleInfo in newVersionInfos)
            {
                if (lastVersionInfosDictionary.TryGetValue(assetBundleInfo.Name, out AssetBundleInfo lastVersionInfo)
                    && assetBundleInfo.MD5.Equals(lastVersionInfo.MD5))
                    assetBundleInfo.Version = lastVersionInfo.Version;
                else
                    assetBundleInfo.Version = Version;
            }
        }
        private void WriteGZipFile(string sourceFile, string targetFile)
        {
            using (FileStream sourceFileStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
            {
                using (FileStream targetFileStream = new FileStream(targetFile, FileMode.Create, FileAccess.Write))
                {
                    GZip.Compress(sourceFileStream, targetFileStream, false);
                }
            }
        }

        public override int GetHashCode() => ConfigName.GetHashCode();

        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}