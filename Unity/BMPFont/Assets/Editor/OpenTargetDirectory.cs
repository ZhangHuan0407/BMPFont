using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Encoder.Editor
{
    /// <summary>
    /// 使用资源管理器打开目标文件夹
    /// </summary>
    public static class OpenTargetDirectory
    {
        [MenuItem("Custom Tool/Open Directory/Persistent Data")]
        public static void OpenPersistentDataDirectory()
        {
            Directory.CreateDirectory(Application.persistentDataPath);
            OpenDirectory(Application.persistentDataPath);
        }
        [MenuItem("Custom Tool/Open Directory/Data")]
        public static void OpenDataDirectory()
        {
            Directory.CreateDirectory(Application.dataPath);
            OpenDirectory(Application.dataPath);
        }

        /// <summary>
        /// 打开指向的文件夹
        /// <para>此处分裂为不同平台的具体实现</para>
        /// </summary>
        public static void OpenDirectory(DirectoryInfo directoryInfo)
        {
            if (directoryInfo != null)
                OpenDirectory(directoryInfo.FullName);
        }
        /// <summary>
        /// 打开指向的文件夹
        /// <para>此处分裂为不同平台的具体实现</para>
        /// </summary>
        public static void OpenDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                UnityEngine.Debug.LogWarning($"Directory is not exists, directoryPath : {directoryPath}.");
                return;
            }
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    _ = Process.Start($"explorer", directoryPath.Replace("/", "\\"));
                    break;
                case RuntimePlatform.OSXEditor:
                    _ = Process.Start("open", $"\"{directoryPath.Replace("\\", " / ")}\"");
                    break;
                default:
                    UnityEngine.Debug.LogError($"Invoke OpenDirectory, but platform is {Application.platform}");
                    break;
            }
        }
    }
}