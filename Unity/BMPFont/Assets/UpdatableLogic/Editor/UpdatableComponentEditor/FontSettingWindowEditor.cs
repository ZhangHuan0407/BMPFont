using System;
using System.Collections.Generic;
using UnityEngine;

namespace Encoder.Editor
{
    [UpdatableComponentEditor("HotFix.UI.FontSettingWindow")]
    public class FontSettingWindowEditor : IUpdatableComponentEditor
    {
        /* field */
        public UpdatableComponent UpdatableComponent { get; set; }

        /* ctor */
        public FontSettingWindowEditor()
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