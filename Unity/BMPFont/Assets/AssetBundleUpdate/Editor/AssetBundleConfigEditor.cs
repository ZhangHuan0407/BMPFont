using UnityEditor;
using UnityEngine;

namespace AssetBundleUpdate.Editor
{
    [CustomEditor(typeof(AssetBundleConfig))]
    public class AssetBundleConfigEditor : UnityEditor.Editor
    {
        /* inter */
        private AssetBundleConfig m_Target => target as AssetBundleConfig;

        /* func */
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            if (GUILayout.Button(nameof(AssetBundleConfig.GetParameters)))
                EditorApplication.delayCall += m_Target.GetParameters;
            EditorGUILayout.Space();
            if (GUILayout.Button(nameof(AssetBundleConfig.GetUpdateSummery)))
                EditorApplication.delayCall += () => m_Target.GetUpdateSummery();
            EditorGUILayout.Space();
            if (GUILayout.Button(nameof(AssetBundleConfig.CreateAssetBundles)))
                EditorApplication.delayCall += m_Target.CreateAssetBundles;
        }
    }
}