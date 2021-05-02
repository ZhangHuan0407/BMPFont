using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Encoder.Editor
{
    [CustomEditor(typeof(UpdatableComponent))]
    public class UpdatableComponentEditor : UnityEditor.Editor
    {
        /* const */
        /// <summary>
        /// 可更新组件类型没有选中具体选项时的默认值
        /// </summary>
        public const int NoneSelection = -1;

        /// <summary>
        /// 所有导出的自定义组件名称
        /// </summary>
        private static GUIContent[] CustomComponentsSelection = new GUIContent[0];
        /// <summary>
        /// 所有导出的自定义组件对应编号
        /// </summary>
        private static int[] CustomComponentsIndex = new int[0];
        /// <summary>
        /// 自定义组件的编辑器实例的构造函数缓存，<see cref="Encoder"/> 在必要时刻重新定向编辑器实例
        /// </summary>
        public static Lazy<Dictionary<string, ConstructorInfo>> CustomComponentEditor =
            new Lazy<Dictionary<string, ConstructorInfo>>(FindCustomComponentEditorFromAssemblies);
        
        /* field */
        /// <summary>
        /// 可更新组件的编辑器实例，用于添加个性化展示
        /// </summary>
        private IUpdatableComponentEditor EditorInstance;

        /// <summary>
        /// 选择的 HotFix 类型名称的缓存，同时也是用户的输入缓存
        /// </summary>
        private string SelectedILTypeName;
        private int SelectionIndex;
        private GUIContent[] PatternSelections;
        private int[] PatternIndex;

        private List<InspectorObject> InspectorObjects;

        /* inter */
        private UpdatableComponent updatableComponent => target as UpdatableComponent;

        /* ctor */
        private void OnEnable()
        {
            SelectedILTypeName = string.Empty;
            PatternSelections = new GUIContent[0];
            PatternIndex = new int[0];
            SelectionIndex = NoneSelection;
            InspectorObjects = new List<InspectorObject>();

            updatableComponent.OnBeforeSerialize_Handle += OnBeforeSerialize_Callback;

            // 还没有选择
            if (string.IsNullOrWhiteSpace(updatableComponent.ILTypeFullName))
            {
                // 强制 CustomComponentEditor.Value 初始化
                _ = CustomComponentEditor.Value;
                EditorApplication.delayCall += RefreshSelectionsPopup;
                return;
            }
            // 通过组件名称找到对应的编辑器实例，这个组件可能就没有编辑器实例
            if (CustomComponentEditor.Value.TryGetValue(updatableComponent.ILTypeFullName, out ConstructorInfo info))
            {
                EditorInstance = info.Invoke(new object[0]) as IUpdatableComponentEditor;
                EditorInstance.UpdatableComponent = updatableComponent;
                EditorInstance.OnEnable();
            }
            // 通过组件名称找到对应的自定义组件序列化信息，这个组件可能没有导出
            if (!DeserializeInspectorObject())
                Debug.LogError($"Not Found CustomComponentInfo in Export File.ILTypeFullName : {updatableComponent.ILTypeFullName}");

            SelectedILTypeName = updatableComponent.ILTypeFullName;
        }

        private void OnDisable()
        {
            // 释放可能存在的编辑器实例
            if (EditorInstance != null)
            {
                EditorInstance.OnDiable();
                EditorInstance.UpdatableComponent = null;
            }

            if (InspectorObjects != null)
            {
                SerializeInspectorObject();
                
                InspectorObjects.Clear();
                InspectorObjects = null;
            }
            updatableComponent.OnBeforeSerialize_Handle = null;

            // 匹配记录清除
            SelectedILTypeName = string.Empty;
            PatternSelections = null;
            PatternIndex = null;
        }

        /* func */
        private static Dictionary<string, ConstructorInfo> FindCustomComponentEditorFromAssemblies()
        {
            Dictionary<string, ConstructorInfo> allCustomUpdatableComponentEditorType = new Dictionary<string, ConstructorInfo>();
            Type[] ZeroTypes = new Type[0];
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var selectedSet =
                    from type in assembly.GetTypes()
                    where type.GetCustomAttribute<CustomComponentEditorAttribute>() != null
                    select
                    (
                        name: type.GetCustomAttribute<CustomComponentEditorAttribute>().UpdatableComponentTypeName,
                        ctor: type.GetConstructor(ZeroTypes)
                    );
                foreach ((string name, ConstructorInfo ctor) selectedType in selectedSet)
                {
                    if (allCustomUpdatableComponentEditorType.ContainsKey(selectedType.name))
                        Debug.LogError($"There are multi-Custom Updatable Component Editor Binding type : {selectedType.name}");
                    else if (selectedType.ctor is null)
                        Debug.LogError($"Not Found Non-parameters ctor in Binding type : {selectedType.name}");
                    else
                        allCustomUpdatableComponentEditorType.Add(selectedType.name, selectedType.ctor);
                }
            }
            string[] componentsName = allCustomUpdatableComponentEditorType.Keys.ToArray();
            CustomComponentsSelection = new GUIContent[componentsName.Length];
            CustomComponentsIndex = new int[componentsName.Length];
            for (int index = 0; index < componentsName.Length; index++)
            {
                CustomComponentsSelection[index] = new GUIContent(componentsName[index]);
                CustomComponentsIndex[index] = index;
            }

            return allCustomUpdatableComponentEditorType;
        }
        /// <summary>
        /// 清除当前的自定义组件编辑器缓存
        /// <para>变更 Unity 主工程代码会触发静态变量全部清除，大多数情况下不需要主动调用</para>
        /// </summary>
        [MenuItem("Custom Tool/Clear Custom Component/Inspector Editor Class Cache")]
        public static void ClearCustomComponentCache()
        {
            CustomComponentEditor.Value?.Clear();
            CustomComponentEditor = new Lazy<Dictionary<string, ConstructorInfo>>(FindCustomComponentEditorFromAssemblies);
        }

        /// <summary>
        /// 确认变更编辑器实例，立即退出当前的实例并尝试切换至目标的编辑器实例
        /// </summary>
        public void SwitchAnotherEditorInstance()
        {
            // 退出上一个编辑器实例
            if (EditorInstance != null)
            {
                EditorInstance.OnDiable();
                EditorInstance.UpdatableComponent = null;
            }
            EditorUtility.SetDirty(target);
            InspectorObjects.Clear();

            // 变更为无组件，旧数据不清除，以模拟“子继承组件”替换
            if (string.IsNullOrWhiteSpace(SelectedILTypeName))
            {
                updatableComponent.ILTypeFullName = string.Empty;
                return;
            }
            // 用户觉得能切换到目标编辑器实例，实际上不行
            // 例如使用 UI. 作为模糊搜索
            if (!CustomComponentEditor.Value.TryGetValue(SelectedILTypeName, out ConstructorInfo info))
                return;
            else
            {
                EditorInstance = info.Invoke(new object[0]) as IUpdatableComponentEditor;
                EditorInstance.UpdatableComponent = updatableComponent;
                updatableComponent.ILTypeFullName = SelectedILTypeName;
                EditorInstance.OnEnable();
            }

            // 通过组件名称找到对应的自定义组件序列化信息，这个组件可能没有导出
            if (!DeserializeInspectorObject())
                Debug.LogError($"Not Found CustomComponentInfo in Export File.ILTypeFullName : {updatableComponent.ILTypeFullName}");
        }
        /// <summary>
        /// 将当前内容视作关键字，匹配者出现在选项中
        /// </summary>
        public void RefreshSelectionsPopup()
        {
            if (string.IsNullOrWhiteSpace(SelectedILTypeName))
            {
                PatternSelections = CustomComponentsSelection;
                PatternIndex = CustomComponentsIndex;
                return;
            }
            List<GUIContent> patternSelectionsList = new List<GUIContent>();
            List<int> patternIndexList = new List<int>();
            for (int index = 0; index < CustomComponentsSelection.Length; index++)
            {
                GUIContent content = CustomComponentsSelection[index];
                if (!content.text.Contains(content.text))
                    continue;

                patternSelectionsList.Add(content);
                patternIndexList.Add(CustomComponentsIndex[index]);
            }
            PatternSelections = patternSelectionsList.ToArray();
            PatternIndex = patternIndexList.ToArray();
        }

        /// <summary>
        /// 序列化准备回调
        /// </summary>
        public void OnBeforeSerialize_Callback()
        {
            if (EditorApplication.isPlaying)
            {
                if (!updatableComponent.BeforeInit)
                    ;
                // reget value.
            }

            SerializeInspectorObject();
            if (!string.IsNullOrWhiteSpace(updatableComponent.ILTypeFullName))
                DeserializeInspectorObject();
        }
        /// <summary>
        /// 序列化面板值到组件
        /// </summary>
        public void SerializeInspectorObject()
        {
            if (!EditorUtility.IsDirty(target))
                return;

            Dictionary<string, UnityEngine.Object> unityDictionary = new Dictionary<string, UnityEngine.Object>();
            Dictionary<string, string> jsonDictionary = new Dictionary<string, string>();
            foreach (InspectorObject inspectorObject in InspectorObjects)
            {
                if (inspectorObject.UseJsonSerialize)
                    jsonDictionary.Add(inspectorObject.Name, (inspectorObject.NewValue as string) ?? string.Empty);
                if (inspectorObject.UseUnitySerialize)
                    unityDictionary.Add(inspectorObject.Name, inspectorObject.NewValue as UnityEngine.Object);
            }
            updatableComponent.SerializableObjectName = unityDictionary.Keys.ToArray();
            updatableComponent.SerializableObject = unityDictionary.Values.ToArray();
            updatableComponent.StructData = JsonConvert.SerializeObject(jsonDictionary);
        }
        /// <summary>
        /// 序列化面板值到组件
        /// <para>是否成功找到自定义组件信息以完成反序列化任务</para>
        /// </summary>
        public bool DeserializeInspectorObject()
        {
            InspectorObjects.Clear();

            // 暂不支持运行时反序列化
            if (EditorApplication.isPlaying)
                return true;
            
            if (CustomComponentBuffer_Editor.ComponentInfoCache.Value.TryGetValue(
                            updatableComponent.ILTypeFullName,
                            out CustomComponentInfo_Editor ComponentInfo_Editor))
            {
                // 反序列化已有的数据
                Dictionary<string, object> deserializeDictionary = new Dictionary<string, object>();
                for (int index = 0; index < updatableComponent.SerializableObjectName.Length; index++)
                    deserializeDictionary.Add(updatableComponent.SerializableObjectName[index], updatableComponent.SerializableObject[index]);
                foreach (var pair in JsonConvert.DeserializeObject<Dictionary<string, string>>(updatableComponent.StructData))
                    deserializeDictionary.Add(pair.Key, pair.Value);
                deserializeDictionary.Add(nameof(UpdatableComponent), this);

                foreach (SerializeItem serializeItem in ComponentInfo_Editor.SerializeItems)
                    InspectorObjects.Add(new InspectorObject(this, deserializeDictionary, serializeItem));
                InspectorObjects.Sort();
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 渲染 Inspector 面板
        /// </summary>
        public override void OnInspectorGUI()
        {
            // 选择绑定的 HotFix 导出实例类型
            GUILayout.BeginHorizontal();
            SelectedILTypeName = GUILayout.TextField(SelectedILTypeName);
            if (GUILayout.Button("Switch Editor", GUILayout.MaxWidth(120f))
                && !SelectedILTypeName.Equals(updatableComponent.ILTypeFullName))
            {
                EditorApplication.delayCall += SwitchAnotherEditorInstance;
                EditorApplication.delayCall += RefreshSelectionsPopup;
                SelectionIndex = NoneSelection;
            }
            GUILayout.EndHorizontal();

            int lastSelectionIndex = SelectionIndex;
            SelectionIndex = EditorGUILayout.IntPopup(SelectionIndex, PatternSelections, PatternIndex);
            if (SelectionIndex != lastSelectionIndex && SelectionIndex != NoneSelection)
            {
                SelectedILTypeName = CustomComponentsSelection[SelectionIndex].text;
                EditorApplication.delayCall += SwitchAnotherEditorInstance;
            }

            EditorGUILayout.Space();
            GUILayout.Label($"{nameof(UpdatableComponent.ILTypeFullName)} : {updatableComponent.ILTypeFullName}");
            EditorInstance?.OnInspectorGUI();

            // 类型方法摘要

            // 序列化字段/属性
            foreach (InspectorObject inspectorObject in InspectorObjects)
                inspectorObject.OnInspectorGUI();
        }
    }
}