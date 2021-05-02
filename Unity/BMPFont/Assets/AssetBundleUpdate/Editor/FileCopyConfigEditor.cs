using UnityEditor;
using UnityEngine;

namespace AssetBundleUpdate.Editor
{
    [CustomEditor(typeof(FileCopyConfig))]
    public class FileCopyConfigEditor : UnityEditor.Editor
    {
        /* field */

        /* inter */

        /* func */
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            FileCopyConfig config = target as FileCopyConfig;
            if (!config)
                return;

            EditorGUILayout.Space();
            if (GUILayout.Button(nameof(FileCopyConfig.GetInOrder)))
                EditorApplication.delayCall += config.GetInOrder;
            EditorGUILayout.Space();
            if (GUILayout.Button(nameof(FileCopyConfig.CheckFilesEqual)))
                EditorApplication.delayCall += FileCopyConfig.CheckFilesEqual;
            EditorGUILayout.Space();
            if (GUILayout.Button(nameof(FileCopyConfig.OvrrideFiles)))
                EditorApplication.delayCall += () =>
                {
                    FileCopyConfig.OvrrideFiles();
                    AssetDatabase.Refresh();
                };
        }
    }
}