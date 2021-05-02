using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Encoder.Editor
{
    public class DeletePrefabPrefs : EditorWindow
    {
        /* field */
        private string m_Key;
        private string m_Content;

        /* ctor */
        [MenuItem("Window/" + nameof(DeletePrefabPrefs))]
        public static void GetDeletePrefabPrefsWindow()
        {
            DeletePrefabPrefs window = GetWindow<DeletePrefabPrefs>();
            window.titleContent = new GUIContent(nameof(DeletePrefabPrefs));
            window.minSize = new Vector2(300f, 200f);
        }

        /* func */
        private void OnGUI()
        {
            m_Key = GUILayout.TextField(m_Key);
            m_Content = GUILayout.TextField(m_Content);
            if (GUILayout.Button("Read String"))
            {
                EditorApplication.delayCall += () =>
                {
                    m_Content = string.Empty;
                    m_Content = PlayerPrefs.GetString(m_Key);
                };
            }
            if (GUILayout.Button("Delete String"))
            {
                EditorApplication.delayCall += () =>
                {
                    m_Content = string.Empty;
                    PlayerPrefs.DeleteKey(m_Key);
                };
            }
        }
    }
}