using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Encoder
{
    public class DashboardSetting : ScriptableObject
    {
        /* const */
        public const string AssetPath = "Assets/Editor Default Resources/DashboardSetting.asset";

        /* field */
        public static GUIContent UseEditorAssetSystemStyle = new GUIContent(
            "Use Editor Asset System", "true : Use UnityEditor AssetDatabase.LoadAssetAtPath to load asset.\n" +
            "false : Use AssetBundle.LoadAsset to load asset.");
        [Header(nameof(AssetBundleUpdate))]
        [SerializeField]
        private bool m_UseEditorAssetSystem;
        public bool UseEditorAssetSystem 
        {
            get => m_UseEditorAssetSystem;
            internal set => m_UseEditorAssetSystem = value;
        }

        public static GUIContent EnableTweenMonitoringStyle = new GUIContent(
            "Enable Tween Monitoring", "true : Tween will statistics behaviour in each frame.\n" +
            "false : Do nothing.");
        [Header(nameof(Tween))]
        [SerializeField]
        private bool m_EnableTweenMonitoring;
        public bool EnableTweenMonitoring
        {
            get => m_EnableTweenMonitoring;
            internal set => m_EnableTweenMonitoring = value;
        }

        public static GUIContent WriteLogOnlyStyle = new GUIContent(
            "Print event log only.");
        [SerializeField]
        private bool m_WriteLogOnly;
        public bool WriteLogOnly
        {
            get => m_WriteLogOnly;
            internal set => m_WriteLogOnly = value;
        }

        /* ctor */
        internal static DashboardSetting GetOrCreateSettings()
        {
            DashboardSetting settings = AssetDatabase.LoadAssetAtPath<DashboardSetting>(AssetPath);
            if (settings == null)
            {
                settings = CreateInstance<DashboardSetting>();
                settings.UseEditorAssetSystem = true;
                settings.WriteLogOnly = true;
                AssetDatabase.CreateAsset(settings, AssetPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        /* inter */
        public static SerializedObject SerializedSettings => new SerializedObject(GetOrCreateSettings());

        /* func */
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void DahboardPushParameter_BeforeSceneLoad()
        {
            DashboardSetting setting = GetOrCreateSettings();

            PropertyInfo useEditorAssetSystemPropertyInfo = typeof(AssetBundleUpdate.AssetBundlePool).GetProperty(nameof(UseEditorAssetSystem), BindingFlags.NonPublic | BindingFlags.Static);
            if (useEditorAssetSystemPropertyInfo is null)
                Debug.LogWarning($"Forget to rename property {nameof(UseEditorAssetSystem)} in DashboardSetting?");
            else
                useEditorAssetSystemPropertyInfo.SetValue(null, setting.UseEditorAssetSystem);

            PropertyInfo writeLogOnlyPropertyInfo = typeof(Log).GetProperty(nameof(WriteLogOnly), BindingFlags.NonPublic | BindingFlags.Static);
            if (writeLogOnlyPropertyInfo is null)
                Debug.LogWarning($"Forget to rename property {nameof(WriteLogOnly)} in DashboardSetting?");
            else
                writeLogOnlyPropertyInfo.SetValue(null, setting.WriteLogOnly);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void DahboardPushParameter_AfterSceneLoad()
        {
            DashboardSetting setting = GetOrCreateSettings();

            if (setting.EnableTweenMonitoring)
                Debug.Log($"{nameof(EnableTweenMonitoring)} => {setting.EnableTweenMonitoring}");
        }
    }
}