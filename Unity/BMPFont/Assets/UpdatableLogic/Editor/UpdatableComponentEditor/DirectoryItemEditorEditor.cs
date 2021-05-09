using System;
using System.Collections.Generic;
using UnityEngine;

namespace Encoder.Editor
{
    [UpdatableComponentEditor("HotFix.UI.Component.DirectoryItem")]
    public class DirectoryItemEditor : IUpdatableComponentEditor
    {
        /* field */
        public UpdatableComponent UpdatableComponent { get; set; }

        /* ctor */
        public DirectoryItemEditor()
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