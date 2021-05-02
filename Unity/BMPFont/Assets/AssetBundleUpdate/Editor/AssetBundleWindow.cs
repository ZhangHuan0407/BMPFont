using System.IO;
using UnityEditor;
using UnityEngine;

namespace AssetBundleUpdate.Editor
{
    public class AssetBundleWindow : EditorWindow
    {
        /* const */
        public const string AssetPath = "Assets/Editor Default Resources/AssetBundleConfig";

        /* field */
        private string configName;

        /* ctor */
        [MenuItem("Window/Asset Bundle")]
        public static void OpenWindow()
        {
            AssetBundleWindow window = GetWindow<AssetBundleWindow>();
            window.titleContent = new GUIContent(nameof(AssetBundleWindow));
        }

        /* func */

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Asset Bundle Config Name :");
            configName = GUILayout.TextField(configName, GUILayout.MinWidth(50f));
            if (GUILayout.Button("Create New Config"))
            {
                if (File.Exists($"{AssetPath}/{configName}.asset"))
                    Debug.LogWarning($"Have asset at path : {AssetPath}/{configName}.asset");
                else
                {
                    AssetBundleConfig assetBundleConfig = CreateInstance<AssetBundleConfig>();
                    assetBundleConfig.ConfigName = configName;
                    assetBundleConfig.GetParameters();
                    Directory.CreateDirectory(AssetPath);
                    AssetDatabase.CreateAsset(assetBundleConfig, $"{AssetPath}/{configName}.asset");
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(15f);




        }


    }
}