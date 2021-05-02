using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace AssetBundleUpdate.Editor
{
    public class FileCopyConfig : ScriptableObject
    {
        /* const */
        [JsonIgnore]
        public const string AssetPath = "Assets/Editor Default Resources/AssetBundleConfig/FileCopyConfig.asset";

        /* field */
        public List<BindingFile> BindingFiles;
        
        /* inter */
        public static Dictionary<string, string> BindingFilesMap
        {
            get
            {
                Dictionary<string, string> map = new Dictionary<string, string>();
                foreach (BindingFile bindingFile in GetOrCreateInsttance().BindingFiles)
                {
                    foreach (KeyValuePair<string, string> filePair in bindingFile.GetFilesPair())
                        map.Add(filePair.Key, filePair.Value);
                }
                return map;
            }
        }

        /* ctor */
        public static FileCopyConfig GetOrCreateInsttance()
        {
            Directory.CreateDirectory(new FileInfo(AssetPath).Directory.FullName);
            FileCopyConfig settings = AssetDatabase.LoadAssetAtPath<FileCopyConfig>(AssetPath);
            if (settings == null)
            {
                settings = CreateInstance<FileCopyConfig>();
                settings.BindingFiles = new List<BindingFile>();
                AssetDatabase.CreateAsset(settings, AssetPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        /* func */
        public void GetInOrder()
        {
            List<BindingFile> buffer = new List<BindingFile>();
            foreach (BindingFile bindingFile in BindingFiles)
            {
                if (bindingFile is null)
                    continue;
                else if (string.IsNullOrWhiteSpace(bindingFile.ImportLocalDirectoryPath)
                    && string.IsNullOrWhiteSpace(bindingFile.ExportLocalDirectoryPath))
                    continue;

                if (string.IsNullOrWhiteSpace(bindingFile.SearchPattern))
                    Debug.LogWarning($"{nameof(BindingFile.SearchPattern)} is NullOrWhiteSpace");

                buffer.Add(bindingFile);
            }
            buffer.Sort();
            BindingFiles = buffer;
            EditorUtility.SetDirty(this);
        }

        [MenuItem("Custom Tool/File Copy/Check Files Equal")]
        public static void CheckFilesEqual()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder warning = new StringBuilder();
            foreach (KeyValuePair<string, string> filePair in BindingFilesMap)
            {
                if (BindingFile.FileIsEqual(filePair.Key, filePair.Value))
                    builder.AppendLine($"{filePair.Key} = {filePair.Value}");
                else
                    warning.AppendLine($"{filePair.Key} != {filePair.Value}");
            }
            Debug.Log(builder);
            if (warning.Length > 0)
                Debug.LogWarning(warning);
        }

        [MenuItem("Custom Tool/File Copy/Ovrride File")]
        public static void OvrrideFiles()
        {
            foreach (KeyValuePair<string, string> filePair in BindingFilesMap)
                BindingFile.OvrrideFile(filePair.Key, filePair.Value);
            AssetDatabase.Refresh();
            Debug.Log("Finish Ovrride Files");
        }
    }
}