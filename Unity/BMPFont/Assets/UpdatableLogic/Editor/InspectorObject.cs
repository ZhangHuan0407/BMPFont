using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Encoder.Editor
{
    /// <summary>
    /// <see cref="UnityEditor"/> 下，一个面板渲染项目
    /// </summary>
    public class InspectorObject : IComparable<InspectorObject>
    {
        /* const */

        /* field */
        public readonly string Name;
        /// <summary>
        /// 面板及其它描述中对象的代称
        /// </summary>
        public readonly GUIContent Title;
        public readonly int Order;
        /// <summary>
        /// 启用 Unity 序列化
        /// </summary>
        public readonly bool UseUnitySerialize;
        /// <summary>
        /// 启用 Json 序列化
        /// </summary>
        public readonly bool UseJsonSerialize;

        public Type ObjectType;
        public readonly object OldValue;
        public object NewValue;

        public UpdatableComponentEditor Editor { get; }

        /* inter */

        /* ctor */
        public InspectorObject(UpdatableComponentEditor editor, Dictionary<string, object> deserializeDictionary, SerializeItem serializeItem)
        {
            if (deserializeDictionary is null)
                throw new ArgumentNullException(nameof(deserializeDictionary));
            if (serializeItem is null)
                throw new ArgumentNullException(nameof(serializeItem));

            Editor = editor ?? throw new ArgumentNullException(nameof(editor));

            Name = serializeItem.Name;
            Title = new GUIContent(serializeItem.Title, serializeItem.ToolTip);
            Order = serializeItem.Order;
            UseUnitySerialize = serializeItem.UseUnitySerialize;
            UseJsonSerialize = serializeItem.UseJsonSerialize;
            ObjectType = Type.GetType(serializeItem.TypeFullName, false);
            if (ObjectType is null)
                Debug.LogError($"Not found type : {serializeItem.TypeFullName}");

            deserializeDictionary.TryGetValue(Name, out OldValue);
            NewValue = OldValue;
        }

        /* func */

        /* IComparable */
        public int CompareTo(InspectorObject other) => Order.CompareTo(other?.Order);

        internal void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Title);
            if (ObjectType is null)
                return;
            bool isUnityObject = ObjectType.IsAssignableFrom(typeof(UnityEngine.Object));
            // Unity Object, 展示完成后可能序列化
            if (UseUnitySerialize || isUnityObject)
            {
                object input = EditorGUILayout.ObjectField(NewValue as UnityEngine.Object, ObjectType, true);
                NewValue = UseUnitySerialize ? input : OldValue;
            }
            // Json 序列化，尽量按照具体数据类型做展示
            if (UseJsonSerialize || !isUnityObject)
            {
                string input = string.Empty;
                if (ObjectType.Equals(typeof(int)))
                {
                    int.TryParse(NewValue as string, out int result);
                    input = EditorGUILayout.IntField("Int", result, GUILayout.MinWidth(30f)).ToString();
                }
                else if (ObjectType.Equals(typeof(float)))
                {
                    float.TryParse(NewValue as string, out float result);
                    input = EditorGUILayout.FloatField("Float", result, GUILayout.MinWidth(30f)).ToString();
                }
                else if (ObjectType.Equals(typeof(bool)))
                {
                    bool.TryParse(NewValue as string, out bool result);
                    input = EditorGUILayout.Toggle("Boolean", result, GUILayout.MinWidth(30)).ToString();
                }
                else if (ObjectType.Equals(typeof(Vector2)))
                {
                    Vector2 vector2 = Vector3.zero;
                    input = EditorGUILayout.Vector3Field("Vector2", vector2, GUILayout.MinWidth(30f)).ToString();
                }
                else if (ObjectType.Equals(typeof(Vector3)))
                {
                    Vector3 vector3 = Vector3.zero;
                    input = EditorGUILayout.Vector3Field("Vector3", vector3, GUILayout.MinWidth(30f)).ToString();
                }
                else if (ObjectType.Equals(typeof(string)))
                {
                    input = EditorGUILayout.TextField("String", NewValue as string, GUILayout.MinWidth(30f));
                }
                // 字符串直接展示和编辑
                else
                    input = GUILayout.TextField(NewValue as string, GUILayout.MinWidth(30f));
                NewValue = UseJsonSerialize ? input : OldValue;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // 同时为空或无效数据，无法判断
            if (NewValue is null && OldValue is null)
                return;
            // 对象被置空
            else if (NewValue is null
                || !NewValue.Equals(OldValue))
                EditorUtility.SetDirty(Editor.target);
        }

        // 运行时重新获取状态值，从而实时显示状态值
        //internal void RumtimeUpdate()
        //{

        //}
    }
}