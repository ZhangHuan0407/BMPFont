using ILRuntime.CLR.Method;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Encoder.Editor
{
    public class InspectorMethod
    {
        /* const */

        /* field */
        /// <summary>
        /// 展示对象实际序列化名称
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// 面板及其它描述中对象的代称
        /// </summary>
        public readonly GUIContent Title;


        public readonly int Order;

        /// <summary>
        /// 编辑器可调用行为
        /// </summary>
        public readonly bool IsEditorAction;
        /// <summary>
        /// 运行时可调用行为
        /// </summary>
        public readonly bool IsRuntimeAction;

        public UpdatableComponentEditor Editor { get; }

        /* inter */

        /* ctor */
        public InspectorMethod(UpdatableComponentEditor editor, MarkingMethod markingMethod)
        {
            if (editor is null)
                throw new ArgumentNullException(nameof(editor));
            if (markingMethod is null)
                throw new ArgumentNullException(nameof(markingMethod));
            Editor = editor ?? throw new ArgumentNullException(nameof(editor));

            Name = markingMethod.Name;
            Title = new GUIContent(markingMethod.Title);
            IsEditorAction = markingMethod.IsEditorAction;
            IsRuntimeAction = markingMethod.IsRuntimeAction;
        }

        /* func */
        public void OnInspectorGUI()
        {
            if (!EditorApplication.isPlaying)
                return;

            if (GUILayout.Button(Title))
            {
                EditorApplication.delayCall += () =>
                {
                    // 组件或者对象已经退出
                    if (!Editor || !Editor.updatableComponent)
                        return;

                    if (!ILRuntimeService.TryFindMethod(Editor.updatableComponent.ILTypeFullName, Name, 0, out IMethod method))
                        Debug.LogError($"Try to invoke {Editor.updatableComponent.ILTypeFullName}.{Name}, but it's not exists.");
                    ILRuntimeService.InvokeMethod(method, Editor.updatableComponent.InstanceObject, new object[0]);
                };
            }
        }
    }
}