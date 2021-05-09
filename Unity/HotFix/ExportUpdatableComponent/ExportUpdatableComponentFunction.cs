using HotFix;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using Encoder;

namespace ExportUpdatableComponent
{
    /// <summary>
    /// 从当前域中导出自定义组件标记的方法
    /// </summary>
    public class ExportUpdatableComponentFunction
    {
        /// <summary>
        /// 获取所有绑定了 BindingUpdatableComponentAttribute 的自定义组件类
        /// <para>可更新组件模糊了继承的概念，因此每个类型算作完全独立的组件</para>
        /// </summary>
        /// <returns>符合要求的类型，导出的(类型, 特性)数组</returns>
        public static KeyValuePair<Type, BindingUpdatableComponentAttribute>[] GetAllUpdatableComponentTypes()
        {
            var updatableComponentTypes = new List<KeyValuePair<Type, BindingUpdatableComponentAttribute>>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (Type type in assembly.GetTypes())
                    if (type.IsAbstract || type.IsGenericType)
                        continue;
                    else if (type.GetCustomAttribute<BindingUpdatableComponentAttribute>(false) is BindingUpdatableComponentAttribute attribute)
                    {
                        updatableComponentTypes.Add(new KeyValuePair<Type, BindingUpdatableComponentAttribute>(type, attribute));
                    }

            return updatableComponentTypes.ToArray();
        }

        /// <summary>
        /// 基于类型和特性信息，导出自定义组件信息
        /// </summary>
        /// <param name="updatableComponentsType">类型和特性</param>
        /// <returns>自定义组件信息</returns>
        public static UpdatableComponentInfo_Editor[] FromTypeToInfo(KeyValuePair<Type, BindingUpdatableComponentAttribute>[] updatableComponentsType)
        {
            if (updatableComponentsType is null)
                throw new ArgumentNullException(nameof(updatableComponentsType));

            UpdatableComponentInfo_Editor[] customComponentInfos = new UpdatableComponentInfo_Editor[updatableComponentsType.Length];
            for (int index = 0; index < updatableComponentsType.Length; index++)
            {
                Type type = updatableComponentsType[index].Key;
                UpdatableComponentInfo_Editor updatableComponentInfo = customComponentInfos[index] = new UpdatableComponentInfo_Editor()
                {
                    BindingComponentName = updatableComponentsType[index].Value.BindingComponentName,
                    TypeFullName = type.FullName,
                };
                FindSerializeItem(type, updatableComponentInfo);
                FindMarkingMethod(type, updatableComponentInfo);

                updatableComponentInfo.SerializeItems.Sort();
            }

            return customComponentInfos;
        }
        private static void FindSerializeItem(Type type, UpdatableComponentInfo_Editor updatableComponentInfo)
        {
            int defaultOrder = 0;
            foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!(fieldInfo.GetCustomAttribute<InspectorInfoAttribute>() is InspectorInfoAttribute inspectorInfo))
                    continue;
                string title = inspectorInfo.Title ?? fieldInfo.Name;
                if (title.StartsWith("m_") && title.Length > 2)
                    title = title.Substring(2);
                bool isUnityObject = fieldInfo.FieldType.IsSubclassOf(typeof(UnityEngine.Object));
                bool isSerizlableType = fieldInfo.FieldType.IsValueType
                    || (fieldInfo.FieldType.IsClass && fieldInfo.FieldType.GetCustomAttribute<SerializableAttribute>(true) != null);
                bool needSerialize;
                if (inspectorInfo.State == ItemSerializableState.Default)
                    needSerialize = fieldInfo.IsPublic;
                else
                    needSerialize = inspectorInfo.State == ItemSerializableState.SerializeIt;

                SerializeItem fieldItem = new SerializeItem()
                {
                    Name = fieldInfo.Name,
                    Order = inspectorInfo.Order < 0 ? defaultOrder : inspectorInfo.Order,
                    TypeFullName = fieldInfo.FieldType.FullName,
                    Title = title,
                    ToolTip = inspectorInfo.ToolTip ?? string.Empty,
                    IsField = true,
                    IsProperty = false,
                    UseUnitySerialize = needSerialize && isUnityObject,
                    UseJsonSerialize = needSerialize && isSerizlableType,
                };
                updatableComponentInfo.SerializeItems.Add(fieldItem);
                defaultOrder++;
            }

            foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!(propertyInfo.GetCustomAttribute<InspectorInfoAttribute>() is InspectorInfoAttribute inspectorInfo))
                    continue;

                bool isUnityObject = propertyInfo.PropertyType.IsSubclassOf(typeof(UnityEngine.Object));
                bool isSerizlableType = propertyInfo.PropertyType.IsValueType
                    || (propertyInfo.PropertyType.IsClass && propertyInfo.PropertyType.GetCustomAttribute<SerializableAttribute>(true) != null);
                bool needSerialize;
                if (inspectorInfo.State == ItemSerializableState.Default)
                    needSerialize = true;
                else
                    needSerialize = inspectorInfo.State == ItemSerializableState.SerializeIt;
                Console.WriteLine($"{propertyInfo.Name}, canSerialize : {needSerialize}, {isSerizlableType}, ");

                SerializeItem fieldItem = new SerializeItem()
                {
                    Name = propertyInfo.Name,
                    Order = inspectorInfo.Order < 0 ? defaultOrder : inspectorInfo.Order,
                    TypeFullName = propertyInfo.PropertyType.FullName,
                    Title = inspectorInfo.Title ?? propertyInfo.Name,
                    IsField = false,
                    IsProperty = true,
                    UseUnitySerialize = needSerialize && isUnityObject,
                    UseJsonSerialize = needSerialize && isSerizlableType,
                };
                updatableComponentInfo.SerializeItems.Add(fieldItem);
                defaultOrder++;
            }
        }
        private static void FindMarkingMethod(Type type, UpdatableComponentInfo_Editor updatableComponentInfo)
        {
            var markingMethods =
                from methodInfo in type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                where methodInfo.GetParameters().Length == 0
                    && methodInfo.GetCustomAttribute<MarkingActionAttribute>() is MarkingActionAttribute invokeActionAttribute
                    && (invokeActionAttribute.IsEditorAction || invokeActionAttribute.IsRuntimeAction)
                select (methodInfo : methodInfo, markingActionAttribute : methodInfo.GetCustomAttribute<MarkingActionAttribute>());

            foreach ((MethodInfo methodInfo, MarkingActionAttribute markingActionAttribute) item in markingMethods)
            {
                MarkingMethod markingMethod = new MarkingMethod()
                {
                    IsEditorAction = item.markingActionAttribute.IsEditorAction,
                    IsRuntimeAction = item.markingActionAttribute.IsRuntimeAction,
                    Name = item.methodInfo.Name,
                    Title = item.markingActionAttribute.Title ?? item.methodInfo.Name,
                };
                updatableComponentInfo.MarkingMethods.Add(markingMethod);
            }
            updatableComponentInfo.MarkingMethods.Sort();
        }

        /// <summary>
        /// 基于导出的自定义组件信息，转换为自定义组件运行时信息
        /// </summary>
        /// <param name="updatableComponentInfos_Editor">自定义组件信息</param>
        /// <returns>运行时信息</returns>
        public static UpdatableComponentInfo[] FromInfoToRuntimeInfo(UpdatableComponentInfo_Editor[] updatableComponentInfos_Editor)
        {
            if (updatableComponentInfos_Editor is null)
                throw new ArgumentNullException(nameof(updatableComponentInfos_Editor));

            List<UpdatableComponentInfo> updatableComponentInfos = new List<UpdatableComponentInfo>();
            foreach (UpdatableComponentInfo_Editor updatableComponentInfo_Editor in updatableComponentInfos_Editor)
            {
                UpdatableComponentInfo updatableComponentInfo = new UpdatableComponentInfo()
                {
                    BindingComponentName = updatableComponentInfo_Editor.BindingComponentName,
                    TypeFullName = updatableComponentInfo_Editor.TypeFullName,
                };
                var serializeItemName = from serializeItem in updatableComponentInfo_Editor.SerializeItems
                                         where serializeItem.UseJsonSerialize
                                            || serializeItem.UseUnitySerialize
                                         select serializeItem.Name;
                updatableComponentInfo.SerializeItems = new HashSet<string>(serializeItemName);
                var markingMethodName = from markingMethod in updatableComponentInfo_Editor.MarkingMethods
                                         where markingMethod.IsRuntimeAction
                                         select markingMethod.Name;
                updatableComponentInfo.MarkingMethods = new HashSet<string>(markingMethodName);
                updatableComponentInfos.Add(updatableComponentInfo);
            }
            return updatableComponentInfos.ToArray();
        }
    }
}