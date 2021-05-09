using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Encoder.Editor
{
    /// <summary>
    /// <see cref="UnityEditor"/> 下，一个面板渲染的数据
    /// </summary>
    public class InspectorObject : IComparable<InspectorObject>
    {
        /* const */
        public static readonly Dictionary<string, Type> TypeDictionary = new Dictionary<string, Type>()
        {
            // UnityEngine
            { "UnityEngine.Animation", typeof(UnityEngine.Animation)},
            { "UnityEngine.Animator", typeof(UnityEngine.Animator)},
            { "UnityEngine.AudioClip", typeof(UnityEngine.AudioClip)},
            { "UnityEngine.GameObject", typeof(UnityEngine.GameObject)},
            { "UnityEngine.Transform", typeof(UnityEngine.Transform)},
            { "UnityEngine.RectTransform", typeof(UnityEngine.RectTransform)},

            // UnityEngine.UI
            { "UnityEngine.UI.Button", typeof(UnityEngine.UI.Button)},
            { "UnityEngine.UI.Graphic", typeof(UnityEngine.UI.Graphic)},
            { "UnityEngine.UI.Image", typeof(UnityEngine.UI.Image)},
            { "UnityEngine.UI.InputField", typeof(UnityEngine.UI.InputField)},
            { "UnityEngine.UI.Mask", typeof(UnityEngine.UI.Mask)},
            { "UnityEngine.UI.RectMask2D", typeof(UnityEngine.UI.RectMask2D)},
            { "UnityEngine.UI.Text", typeof(UnityEngine.UI.Text)},
        };

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
        /// 启用 Unity 序列化
        /// </summary>
        public readonly bool UseUnitySerialize;
        /// <summary>
        /// 启用 Json 序列化
        /// </summary>
        public readonly bool UseJsonSerialize;

        /// <summary>
        /// 展示对象类型
        /// </summary>
        public Type ObjectType;
        /// <summary>
        /// 展示对象原始值
        /// </summary>
        public readonly object OldValue;
        /// <summary>
        /// 展示对象当前值
        /// </summary>
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
            if (ObjectType is null
                && !TypeDictionary.TryGetValue(serializeItem.TypeFullName, out ObjectType))
                Debug.LogError($"Not found type : {serializeItem.TypeFullName}");

            deserializeDictionary.TryGetValue(Name, out OldValue);
            NewValue = OldValue;
        }

        /* func */
        public void OnInspectorGUI()
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
            else if (UseJsonSerialize || !isUnityObject)
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

        /* IComparable */
        public int CompareTo(InspectorObject other) => Order.CompareTo(other?.Order);
    }
}