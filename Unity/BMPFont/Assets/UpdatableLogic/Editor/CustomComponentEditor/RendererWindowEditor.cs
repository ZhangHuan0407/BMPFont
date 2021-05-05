using System;
using System.Collections.Generic;
using UnityEngine;

namespace Encoder.Editor.CustomComponentEditor
{
    [CustomComponentEditor("HotFix.UI.RendererWindow")]
    public class RendererWindowEditor : IUpdatableComponentEditor
    {
        /* field */
        public UpdatableComponent UpdatableComponent { get; set; }

        /* ctor */
        public RendererWindowEditor()
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