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
    public class ExportCustomComponentFunction
    {
        /// <summary>
        /// 获取所有绑定了 BindingUpdatableComponentAttribute 的自定义组件类
        /// <para>可更新组件模糊了继承的概念，因此每个类型算作完全独立的组件</para>
        /// </summary>
        /// <returns>符合要求的类型，导出的(类型, 特性)数组</returns>
        public static KeyValuePair<Type, BindingUpdatableComponentAttribute>[] GetAllCustomComponentTypes()
        {
            var customComponentTypes = new List<KeyValuePair<Type, BindingUpdatableComponentAttribute>>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (Type type in assembly.GetTypes())
                    if (type.IsAbstract || type.IsGenericType)
                        continue;
                    else if (type.GetCustomAttribute<BindingUpdatableComponentAttribute>(false) is BindingUpdatableComponentAttribute attribute)
                    {
                        customComponentTypes.Add(new KeyValuePair<Type, BindingUpdatableComponentAttribute>(type, attribute));
                    }

            return customComponentTypes.ToArray();
        }

        /// <summary>
        /// 基于类型和特性信息，导出自定义组件信息
        /// </summary>
        /// <param name="customComponentsType">类型和特性</param>
        /// <returns>自定义组件信息</returns>
        public static CustomComponentInfo_Editor[] FromTypeToInfo(KeyValuePair<Type, BindingUpdatableComponentAttribute>[] customComponentsType)
        {
            if (customComponentsType is null)
                throw new ArgumentNullException(nameof(customComponentsType));

            CustomComponentInfo_Editor[] customComponentInfos = new CustomComponentInfo_Editor[customComponentsType.Length];
            for (int index = 0; index < customComponentsType.Length; index++)
            {
                Type type = customComponentsType[index].Key;
                CustomComponentInfo_Editor customComponentInfo = customComponentInfos[index] = new CustomComponentInfo_Editor()
                {
                    BindingComponentName = customComponentsType[index].Value.BindingComponentName,
                    TypeFullName = type.FullName,
                };
                FindSerializeItem(type, customComponentInfo);
                FindMarkingMethod(type, customComponentInfo);

                customComponentInfo.SerializeItems.Sort();
            }

            return customComponentInfos;
        }
        private static void FindSerializeItem(Type type, CustomComponentInfo_Editor customComponentInfo)
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
                customComponentInfo.SerializeItems.Add(fieldItem);
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
                customComponentInfo.SerializeItems.Add(fieldItem);
                defaultOrder++;
            }
        }
        private static void FindMarkingMethod(Type type, CustomComponentInfo_Editor customComponentInfo)
        {
            var markingMethods =
                from methodInfo in type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                where methodInfo.GetParameters().Length == 0
                    && methodInfo.GetCustomAttribute<InvokeActionAttribute>() is InvokeActionAttribute invokeActionAttribute
                    && (invokeActionAttribute.IsEditorAction || invokeActionAttribute.IsRuntimeAction)
                select (methodInfo : methodInfo, invokeActionAttribute : methodInfo.GetCustomAttribute<InvokeActionAttribute>());

            foreach ((MethodInfo methodInfo, InvokeActionAttribute invokeActionAttribute) item in markingMethods)
            {
                MarkingMethod markingMethod = new MarkingMethod()
                {
                    IsEditorAction = item.invokeActionAttribute.IsEditorAction,
                    IsRuntimeAction = item.invokeActionAttribute.IsRuntimeAction,
                    Name = item.methodInfo.Name,
                    Title = item.invokeActionAttribute.Title ?? item.methodInfo.Name,
                };
                customComponentInfo.MarkingMethods.Add(markingMethod);
            }
            customComponentInfo.MarkingMethods.Sort();
        }

        /// <summary>
        /// 基于导出的自定义组件信息，转换为自定义组件运行时信息
        /// </summary>
        /// <param name="customComponentInfos_Editor">自定义组件信息</param>
        /// <returns>运行时信息</returns>
        public static CustomComponentInfo[] FromInfoToRuntimeInfo(CustomComponentInfo_Editor[] customComponentInfos_Editor)
        {
            if (customComponentInfos_Editor is null)
                throw new ArgumentNullException(nameof(customComponentInfos_Editor));

            List<CustomComponentInfo> customComponentInfos = new List<CustomComponentInfo>();
            foreach (CustomComponentInfo_Editor customComponentInfo_Editor in customComponentInfos_Editor)
            {
                CustomComponentInfo customComponentInfo = new CustomComponentInfo()
                {
                    BindingComponentName = customComponentInfo_Editor.BindingComponentName,
                    TypeFullName = customComponentInfo_Editor.TypeFullName,
                };
                var serializeItemName = from serializeItem in customComponentInfo_Editor.SerializeItems
                                         where serializeItem.UseJsonSerialize
                                            || serializeItem.UseUnitySerialize
                                         select serializeItem.Name;
                customComponentInfo.SerializeItems = new HashSet<string>(serializeItemName);
                var markingMethodName = from markingMethod in customComponentInfo_Editor.MarkingMethods
                                         where markingMethod.IsRuntimeAction
                                         select markingMethod.Name;
                customComponentInfo.MarkingMethods = new HashSet<string>(markingMethodName);
                customComponentInfos.Add(customComponentInfo);
            }
            return customComponentInfos.ToArray();
        }
    }
}