using ILRuntime.CLR.Method;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Encoder
{
    public static class CustomComponentMethodBuffer
    {
        /* field */
        private static Dictionary<string, Dictionary<string, IMethod>> m_ComponentMethodsCache;
        public static Dictionary<string, Dictionary<string, IMethod>> ComponentMethodsCache
        {
            get => m_ComponentMethodsCache;
            set => m_ComponentMethodsCache = value;
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

            if (ILRuntimeService.Instance is null)
            {
                Debug.LogError($"{nameof(ILRuntimeService)} is not ready.");
                return;
            }

            CustomComponentInfo[] customComponentInfos = JsonConvert.DeserializeObject<CustomComponentInfo[]>(jsonData);
            Dictionary<string, Dictionary<string, IMethod>> componentMethodsCache = ComponentMethodsCache;

            for (int index = 0; index < customComponentInfos.Length; index++)
            {
                CustomComponentInfo customComponentInfo = customComponentInfos[index];
                string typeFullName = customComponentInfo.TypeFullName;
                if (componentMethodsCache.ContainsKey(typeFullName))
                {
                    Debug.LogError($"Have same Custom Component in cache.{nameof(CustomComponentInfo.TypeFullName)} : {typeFullName}");
                    continue;
                }

                Dictionary<string, IMethod> methodsCache = new Dictionary<string, IMethod>();
                componentMethodsCache.Add(typeFullName, methodsCache);
                foreach (string methodName in customComponentInfo.MarkingMethods)
                {
                    if (ILRuntimeService.TryFindMethod(typeFullName, methodName, 0, out IMethod method))
                        methodsCache.Add(methodName, method);
                    else
                    {
                        methodsCache.Add(methodName, null);
                        Debug.LogError($"One of Custom Component's method is not exists.Type : {typeFullName}, Method : {methodName}");
                    }
                }
            }
        }
    }
}