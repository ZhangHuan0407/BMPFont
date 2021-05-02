using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundleUpdate.Editor
{
    [Serializable]
    public class BindingFile : IComparable<BindingFile>
    {
        private const int BufferLength = 5 * 1024 * 1024;

        /* field */
        public string ImportLocalDirectoryPath;
        public string ExportLocalDirectoryPath;
        public string SearchPattern;
        public string ReplaceName;

        /* ctor */
        public BindingFile()
        {
            ReplaceName = "{FileName}";
        }

        /* inter */

        /* func */
        /// <summary>
        /// 检测两个文件的内容是否相同(字节流)，若一者不存在则不同
        /// </summary>
        /// <param name="sourceFilePath">源文件</param>
        /// <param name="targetFilePath">目标文件</param>
        /// <returns>文件内容是否相同</returns>
        public static bool FileIsEqual(string sourceFilePath, string targetFilePath)
        {
            if (string.IsNullOrWhiteSpace(sourceFilePath))
                throw new ArgumentException($"“{nameof(sourceFilePath)}”不能为 Null 或空白", nameof(sourceFilePath));
            if (string.IsNullOrWhiteSpace(targetFilePath))
                return false;

            FileInfo sourceFileInfo = new FileInfo(sourceFilePath);
            FileInfo targetFileInfo = new FileInfo(targetFilePath);
            if (!sourceFileInfo.Exists)
            {
                Debug.LogWarning($"File Not Found.{sourceFilePath} as {sourceFileInfo.FullName}");
                return false;
            }
            else if (!targetFileInfo.Exists)
            {
                Debug.LogWarning($"File Not Found.{targetFilePath} as {targetFileInfo.FullName}");
                return false;
            }
            else if (sourceFileInfo.Length != targetFileInfo.Length)
                return false;

            byte[] sourceBinData;
            byte[] targetBinData;
            if (sourceFileInfo.Length < BufferLength)
            {
                sourceBinData = File.ReadAllBytes(sourceFileInfo.FullName);
                targetBinData = File.ReadAllBytes(targetFileInfo.FullName);
                for (int index = 0; index < sourceBinData.Length; index++)
                {
                    if (sourceBinData[index] != targetBinData[index])
                        return false;
                }
            }
            else
            {
                sourceBinData = new byte[BufferLength];
                targetBinData = new byte[BufferLength];
                using (Stream sourceStream = new FileStream(sourceFileInfo.FullName, FileMode.Open, FileAccess.Read))
                {
                    using (Stream targetStream = new FileStream(targetFileInfo.FullName, FileMode.Open, FileAccess.Read))
                    {
                        int length = sourceStream.Read(sourceBinData, 0, BufferLength);
                        targetStream.Read(targetBinData, 0, BufferLength);
                        for (int index = 0; index < length; index++)
                            if (sourceBinData[index] != targetBinData[index])
                                return false;
                    }
                }
            }

            return true;
        }
        public static void OvrrideFile(string sourceFilePath, string targetFilePath)
        {
            if (string.IsNullOrWhiteSpace(sourceFilePath))
                throw new ArgumentException($"“{nameof(sourceFilePath)}”不能为 Null 或空白", nameof(sourceFilePath));
            if (string.IsNullOrWhiteSpace(targetFilePath))
                throw new ArgumentException($"“{nameof(targetFilePath)}”不能为 Null 或空白", nameof(targetFilePath));

            FileInfo sourceFileInfo = new FileInfo(sourceFilePath);
            FileInfo targetFileInfo = new FileInfo(targetFilePath);
            if (!sourceFileInfo.Exists)
            {
                Debug.LogWarning($"File Not Found.{sourceFilePath} as {sourceFileInfo.FullName}");
                return;
            }
            Directory.CreateDirectory(targetFileInfo.Directory.FullName);
            File.Copy(sourceFilePath, targetFilePath, true);
        }

        /// <summary>
        /// 获取当前绑定文件中需要进行覆盖的文件对
        /// </summary>
        /// <returns>(复制源文件，目标文件)</returns>
        public IEnumerable<KeyValuePair<string, string>> GetFilesPair()
        {
            if (!Directory.Exists(ImportLocalDirectoryPath))
            {
                Debug.LogWarning($"Directory is not exists.{ImportLocalDirectoryPath}");
                yield break;
            }
            foreach (string sourceFilePath in Directory.GetFiles(ImportLocalDirectoryPath, SearchPattern))
            {
                int nameIndex = sourceFilePath.Replace('\\', '/').LastIndexOf('/');
                if (nameIndex == -1)
                {
                    Debug.LogWarning($"Argument Exception.{sourceFilePath}");
                    yield break;
                }
                string fileName = sourceFilePath.Substring(nameIndex + 1);
                string targetFilePath = $"{ExportLocalDirectoryPath}/{ReplaceName.Replace("{FileName}", fileName)}";
                yield return new KeyValuePair<string, string>(sourceFilePath, targetFilePath);
            }
        }

        /* IComparable */
        public int CompareTo(BindingFile other)
        {
            if (other is null)
                return 0;
            else
                return string.Compare(ImportLocalDirectoryPath, other.ImportLocalDirectoryPath);
        }
    }
}