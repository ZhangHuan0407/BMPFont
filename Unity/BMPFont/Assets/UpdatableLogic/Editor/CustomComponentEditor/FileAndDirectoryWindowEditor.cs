using System;
using System.Collections.Generic;
using UnityEngine;

namespace Encoder.Editor.CustomComponentEditor
{
    [CustomComponentEditor("HotFix.UI.FileAndDirectoryWindow")]
    public class FileAndDirectoryWindowEditor : IUpdatableComponentEditor
    {
        /* field */
        public UpdatableComponent UpdatableComponent { get; set; }

        /* ctor */
        public FileAndDirectoryWindowEditor()
        {
        }

        /* IUpdatableComponentEditor */
        public void OnEnable()
        {
        }

        public void OnDiable()
        {
        }

        public void OnInspectorGUI()
        {
        }
    }
}