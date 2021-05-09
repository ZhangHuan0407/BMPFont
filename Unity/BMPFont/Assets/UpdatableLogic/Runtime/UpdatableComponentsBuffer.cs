using ILRuntime.CLR.Method;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Encoder
{
    public static class UpdatableComponentsBuffer
    {
        /* const */
        public static readonly Dictionary<string, Type> AllUpdatableComponents = new Dictionary<string, Type>()
        {
            { typeof(ContainerUpdatableComponent).FullName,     typeof(ContainerUpdatableComponent) },
            { typeof(CommonUpdatableComponent).FullName,        typeof(CommonUpdatableComponent) },
            { typeof(Collision2DUpdatableComponent).FullName,   typeof(Collision2DUpdatableComponent) },
            { typeof(LogicUpdatableComponent).FullName,         typeof(LogicUpdatableComponent) },
            { typeof(ReferenceUpdatableComponent).FullName,     typeof(ReferenceUpdatableComponent) },
        };

        /* field */
        private static Dictionary<string, Dictionary<string, IMethod>> m_ComponentMethodsCache;
        public static Dictionary<string, Dictionary<string, IMethod>> ComponentMethodsCache
        {
            get => m_ComponentMethodsCache;
            set => m_ComponentMethodsCache = value;
        }

        private static Dictionary<string, Type> m_ComponentBindingType;
        public static Dictionary<string, Type> ComponentBindingType
        {
            get => m_ComponentBindingType;
            set => m_ComponentBindingType = value;
        }

        /* inter */

        /* ctor */

        /* func */
        /// <summary>
        /// 清除现有的缓存，并使用指定的 Json 字符串反序列化为组件方法缓存
        /// </summary>
        /// <param name="jsonData">Json 序列化的组件方法信息</param>
        public static void ClearAndRecreateCache(string jsonData)
        {
            if (string.IsNullOrWhiteSpace(jsonData))
                throw new ArgumentException($"“{nameof(jsonData)}”不能为 null 或空白。", nameof(jsonData));

            if (ComponentMethodsCache is null)
                ComponentMethodsCache = new Dictionary<string, Dictionary<string, IMethod>>();
            else
                ComponentMethodsCache.Clear();

            if (ComponentBindingType is null)
                ComponentBindingType = new Dictionary<string, Type>();
            else
                ComponentBindingType.Clear();

            if (ILRuntimeService.Instance is null)
            {
                Debug.LogError($"{nameof(ILRuntimeService)} is not ready.");
                return;
            }

            UpdatableComponentInfo[] updatableComponentInfos = JsonConvert.DeserializeObject<UpdatableComponentInfo[]>(jsonData);
            Dictionary<string, Dictionary<string, IMethod>> componentMethodsCache = ComponentMethodsCache;
            Dictionary<string, Type> componentBindingType = ComponentBindingType;

            for (int index = 0; index < updatableComponentInfos.Length; index++)
            {
                UpdatableComponentInfo updatableComponentInfo = updatableComponentInfos[index];
                string typeFullName = updatableComponentInfo.TypeFullName;
                if (componentMethodsCache.ContainsKey(typeFullName))
                {
                    Debug.LogError($"Have same UpdatableComponent in cache.{nameof(UpdatableComponentInfo.TypeFullName)} : {typeFullName}");
                    continue;
                }

                Dictionary<string, IMethod> methodsCache = new Dictionary<string, IMethod>();
                componentMethodsCache.Add(typeFullName, methodsCache);
                foreach (string methodName in updatableComponentInfo.MarkingMethods)
                {
                    if (ILRuntimeService.TryFindMethod(typeFullName, methodName, 0, out IMethod method))
                        methodsCache.Add(methodName, method);
                    else
                    {
                        methodsCache.Add(methodName, null);
                        Debug.LogError($"One of UpdatableComponent's method is not exists.Type : {typeFullName}, Method : {methodName}");
                    }
                }

                if (AllUpdatableComponents.TryGetValue(updatableComponentInfo.BindingComponentName, out Type type))
                    componentBindingType.Add(typeFullName, type);
                else
                    Debug.LogError($"Class : {typeFullName} try to binding {nameof(UpdatableComponent)} : {updatableComponentInfo.BindingComponentName}, but it's not exists.");
            }
        }
    }
}