using System;
using System.Collections.Generic;
using UnityEngine;

namespace Encoder.Editor.CustomComponentEditor
{
    [CustomComponentEditor("HotFix.UI.Component.CountdownText")]
    public class CountdownTextEditor : IUpdatableComponentEditor
    {
        /* field */
        public UpdatableComponent UpdatableComponent { get; set; }

        /* ctor */
        public CountdownTextEditor()
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