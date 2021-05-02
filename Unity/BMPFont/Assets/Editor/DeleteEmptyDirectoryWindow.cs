using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Encoder.Editor
{
    public class DeleteEmptyDirectoryWindow : EditorWindow
    {
        /* field */
        public Queue<DirectoryInfo> EmptyDirectories;
        public bool[] UserSelected;
        private Vector2 m_ScrollPosition;
        private string m_InputDirectory;

        /// <summary>
        /// 删除 MaxOS 下的隐藏样式文件
        /// </summary>
        public static bool DeleteDSStoreFile = true;
        public static bool DeleteThumbsFile = true;

        /* ctor */
        [MenuItem("Window/" + nameof(DeleteEmptyDirectoryWindow))]
        public static void GetDeleteEmptyDirectoryWindow()
        {
            DeleteEmptyDirectoryWindow window = GetWindow<DeleteEmptyDirectoryWindow>();
            window.titleContent = new GUIContent(nameof(DeleteEmptyDirectoryWindow));
            window.minSize = new Vector2(500f, 200f);
        }

        public void OnEnable()
        {
            EmptyDirectories = new Queue<DirectoryInfo>();
            UserSelected = new bool[0];
            m_ScrollPosition = Vector2.zero;
            m_InputDirectory = string.Empty;
        }
        public void OnDisable()
        {
            EmptyDirectories.Clear();
            UserSelected = null;
            m_ScrollPosition = default;
            m_InputDirectory = null;
        }

        /* func */
        private void OnGUI()
        {
            DeleteDSStoreFile = GUILayout.Toggle(DeleteDSStoreFile, "删除 .DS_Store 文件");
            DeleteThumbsFile = GUILayout.Toggle(DeleteThumbsFile, "删除 Thumbs.db 文件");
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            GUILayout.Label("要检查的文件夹");
            m_InputDirectory = GUILayout.TextField(m_InputDirectory, GUILayout.MinWidth(300f));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (GUILayout.Button(nameof(CheckDirectory), GUILayout.Width(300f)))
                EditorApplication.delayCall += () => CheckDirectory(m_InputDirectory);
            EditorGUILayout.Space();

            if (GUILayout.Button(nameof(DeleteEmptyDirectory), GUILayout.Width(300f))
                && !string.IsNullOrWhiteSpace(m_InputDirectory))
                EditorApplication.delayCall += DeleteEmptyDirectory;
            EditorGUILayout.Space();

            m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);
            int index = 0;
            foreach (DirectoryInfo directoryInfo in EmptyDirectories)
            {
                GUILayout.BeginHorizontal();
                UserSelected[index] = GUILayout.Toggle(UserSelected[index], directoryInfo.ToString());
                GUILayout.EndHorizontal();
                index++;
            }
            GUILayout.EndScrollView();
        }

        public void CheckDirectory(string directoryPath)
        {
            if (directoryPath is null
                || !Directory.Exists(directoryPath))
            {
                Debug.LogWarning($"Directory : {directoryPath} is not exists");
                return;
            }

            EmptyDirectories.Clear();
            Stack<DirectoryInfo> directoryInfos = new Stack<DirectoryInfo>();
            directoryInfos.Push(new DirectoryInfo(directoryPath));

            while (directoryInfos.Count > 0)
            {
                DirectoryInfo directoryInfo = directoryInfos.Pop();
                string[] childDirectories = Directory.GetDirectories(directoryInfo.FullName);
                foreach (string childDirectory in childDirectories)
                    directoryInfos.Push(new DirectoryInfo(childDirectory));
                if (DeleteDSStoreFile
                    && File.Exists($"{directoryInfo.FullName}/.DS_Store"))
                    File.Delete($"{directoryInfo.FullName}/.DS_Store");
                if (DeleteThumbsFile
                    && File.Exists($"{directoryInfo.FullName}/Thumbs.db"))
                    File.Delete($"{directoryInfo.FullName}/Thumbs.db");
                int childFilesCount = Directory.GetFiles(directoryInfo.FullName).Length;
                if (childFilesCount > 0)
                    continue;

                if (childDirectories.Length == 0)
                    EmptyDirectories.Enqueue(directoryInfo);
            }
            UserSelected = new bool[EmptyDirectories.Count];
            for (int i = 0; i < UserSelected.Length; i++)
                UserSelected[i] = true;
        }
        public void DeleteEmptyDirectory()
        {
            HashSet<string> parentDirectory = new HashSet<string>();
            int index = 0;
            while (EmptyDirectories.Count > 0)
            {
                DirectoryInfo directoryInfo = EmptyDirectories.Dequeue();
                if (UserSelected[index]
                    && Directory.Exists(directoryInfo.FullName)
                    && Directory.GetFiles(directoryInfo.FullName).Length == 0
                    && Directory.GetDirectories(directoryInfo.FullName).Length == 0)
                {
                    Directory.Delete(directoryInfo.FullName);
                    string parentDirectoryPath = directoryInfo.Parent.FullName;
                    string directoryMetaPath = $"{parentDirectoryPath}/{directoryInfo.Name}.meta";
                    if (File.Exists(directoryMetaPath))
                        File.Delete(directoryMetaPath);
                    parentDirectory.Add(parentDirectoryPath);
                }
                index++;
            }
            EmptyDirectories.Clear();

            AssetDatabase.Refresh();

            foreach (string parentDirectoryPath in parentDirectory)
            {
                if (Directory.GetFiles(parentDirectoryPath).Length == 0
                    && Directory.GetDirectories(parentDirectoryPath).Length == 0)
                    EmptyDirectories.Enqueue(new DirectoryInfo(parentDirectoryPath));
            }
            UserSelected = new bool[EmptyDirectories.Count];
            for (int i = 0; i < UserSelected.Length; i++)
                UserSelected[i] = true;
        }
    }
}