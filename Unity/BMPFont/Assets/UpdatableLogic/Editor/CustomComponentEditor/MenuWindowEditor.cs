using System;
using System.Collections.Generic;
using UnityEngine;

namespace Encoder.Editor.CustomComponentEditor
{
    [CustomComponentEditor("HotFix.UI.MenuWindow")]
    public class MenuWindowEditor : IUpdatableComponentEditor
    {
        /* field */
        public UpdatableComponent UpdatableComponent { get; set; }

        /* ctor */
        public MenuWindowEditor()
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