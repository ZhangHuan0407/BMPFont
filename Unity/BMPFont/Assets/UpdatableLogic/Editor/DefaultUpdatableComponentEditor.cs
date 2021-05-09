using System;
using System.Collections.Generic;
using UnityEngine;

namespace Encoder.Editor
{
    public class DefaultUpdatableComponentEditor : IUpdatableComponentEditor
    {
        /* field */
        public UpdatableComponent UpdatableComponent { get; set; }

        /* ctor */
        public DefaultUpdatableComponentEditor()
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