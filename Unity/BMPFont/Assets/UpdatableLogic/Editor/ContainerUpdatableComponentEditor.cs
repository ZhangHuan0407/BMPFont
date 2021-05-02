using System;
using System.Collections.Generic;
using UnityEditor;

namespace Encoder.Editor
{
    [CustomEditor(typeof(ContainerUpdatableComponent))]
    public class ContainerUpdatableComponentEditor : UpdatableComponentEditor
    {
        /* func */
        /// <summary>
        /// 重新绘制面板
        /// <para>在基类<see cref="UpdatableComponentEditor"/>中实现</para>
        /// </summary>
        public override void OnInspectorGUI() => base.OnInspectorGUI();
    }
}