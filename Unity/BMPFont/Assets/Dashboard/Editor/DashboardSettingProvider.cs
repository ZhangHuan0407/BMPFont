using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Encoder
{
    public class DashboardSettingProvider : SettingsProvider
    {
        /* field */
        private SerializedObject m_SerializedDashboardSetting;
        private DashboardSetting m_DashboardSetting;

        /* ctor */
        public DashboardSettingProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            m_DashboardSetting = DashboardSetting.GetOrCreateSettings();
            m_SerializedDashboardSetting = DashboardSetting.SerializedSettings;
        }

        [SettingsProvider]
        public static SettingsProvider CreateDashboardSettingProvider()
        {
            DashboardSettingProvider provider = new DashboardSettingProvider("Project/DashboardSetting", SettingsScope.Project);

            provider.keywords = GetSearchKeywordsFromGUIContentProperties<DashboardSetting>();
            return provider;
        }

        /* inter */

        /* func */
        public override void OnGUI(string searchContext)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
                m_SerializedDashboardSetting.ApplyModifiedProperties();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Cancel"))
                m_SerializedDashboardSetting.UpdateIfRequiredOrScript();
            GUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(m_SerializedDashboardSetting.FindProperty("m_UseEditorAssetSystem"), DashboardSetting.UseEditorAssetSystemStyle);
            
            EditorGUILayout.PropertyField(m_SerializedDashboardSetting.FindProperty("m_EnableTweenMonitoring"), DashboardSetting.EnableTweenMonitoringStyle);
            if (m_DashboardSetting.EnableTweenMonitoring)
            {
                string averageRecord = JsonConvert.SerializeObject(Tween.Editor.TweenMonitoring.AverageRecord, Formatting.Indented);
                GUILayout.Label(averageRecord);
            }

            EditorGUILayout.PropertyField(m_SerializedDashboardSetting.FindProperty("m_WriteLogOnly"), DashboardSetting.WriteLogOnlyStyle);
        }
        public override void OnInspectorUpdate()
        {
            if (!EditorApplication.isPlaying)
                return;

            if (m_DashboardSetting.EnableTweenMonitoring)
                Tween.Editor.TweenMonitoring.UpdateAverage();
        }
    }
}