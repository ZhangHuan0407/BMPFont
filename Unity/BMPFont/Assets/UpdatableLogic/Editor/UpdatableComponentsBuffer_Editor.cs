using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Encoder.Editor
{
    /// <summary>
    /// 缓存反序列化的<see cref="UpdatableComponentInfo_Editor"/>信息
    /// </summary>
    public static class UpdatableComponentsBuffer_Editor
    {
        /* const */
        /// <summary>
        /// Editor 自定义组件导出数据路径
        /// </summary>
        public const string UpdatableComponentInfo_EditorAssetPath = "Assets/UpdatableLogic/Editor/UpdatableComponentInfo_Editor.json";

        public static readonly Dictionary<string, Type> AllUpdatableComponents = new Dictionary<string, Type>()
        {
            { typeof(ContainerUpdatableComponent).FullName,     typeof(ContainerUpdatableComponent) },
            { typeof(CommonUpdatableComponent).FullName,        typeof(CommonUpdatableComponent) },
            { typeof(Collision2DUpdatableComponent).FullName,   typeof(Collision2DUpdatableComponent) },
            { typeof(LogicUpdatableComponent).FullName,         typeof(LogicUpdatableComponent) },
            { typeof(ReferenceUpdatableComponent).FullName,     typeof(ReferenceUpdatableComponent) },
        };

        /* field */
        public static Lazy<Dictionary<string, UpdatableComponentInfo_Editor>> ComponentInfoCache =
            new Lazy<Dictionary<string, UpdatableComponentInfo_Editor>>(GetComponentInfoCache);

        /* inter */

        /* ctor */

        /* func */
        private static Dictionary<string, UpdatableComponentInfo_Editor> GetComponentInfoCache()
        {
            Dictionary<string, UpdatableComponentInfo_Editor> result = new Dictionary<string, UpdatableComponentInfo_Editor>();
            if (!File.Exists(UpdatableComponentInfo_EditorAssetPath))
            {
                Debug.LogError("Not Found UpdatableComponentInfo_Editor.json.");
                return result;
            }

            string content = File.ReadAllText(UpdatableComponentInfo_EditorAssetPath);
            UpdatableComponentInfo_Editor[] updatableComponentInfos_Editor = JsonConvert.DeserializeObject<UpdatableComponentInfo_Editor[]>(content);
            foreach (UpdatableComponentInfo_Editor updatableComponentInfo_Editor in updatableComponentInfos_Editor)
                result.Add(updatableComponentInfo_Editor.TypeFullName, updatableComponentInfo_Editor);
            return result;
        }
        /// <summary>
        /// 清除当前的自定义组件导出数据的实例缓存
        /// <para>变更 HotFix 代码不会触发数据清除，默认还在使用上一次读入的导出数据，大多数情况下需要主动调用</para>
        /// </summary>
        [MenuItem("Custom Tool/Clear Updatable Components Cache/Serialize Item And Marking Method")]
        public static void ClearCache()
        {
            if (ComponentInfoCache.IsValueCreated)
                ComponentInfoCache = new Lazy<Dictionary<string, UpdatableComponentInfo_Editor>>(GetComponentInfoCache, false);
        }

        public static Type GetComponentBindingType(string updatableComponentName)
        {
            if (string.IsNullOrWhiteSpace(updatableComponentName))
                throw new ArgumentException($"{nameof(updatableComponentName)} can't be none or whitespace.", nameof(updatableComponentName));

            if (ComponentInfoCache.Value.TryGetValue(updatableComponentName, out UpdatableComponentInfo_Editor info_Editor)
                && AllUpdatableComponents.TryGetValue(info_Editor.BindingComponentName, out Type bindingType))
                return bindingType;
            else
                return null;
        }

        public static MarkingMethod[] GetEditorMarkingMethods(string updatableComponentName)
        {
            if (string.IsNullOrWhiteSpace(updatableComponentName))
                throw new ArgumentException($"{nameof(updatableComponentName)} can't be none or whitespace.", nameof(updatableComponentName));

            if (ComponentInfoCache.Value.TryGetValue(updatableComponentName, out UpdatableComponentInfo_Editor info_Editor))
            {
                var editorMarkingMethods = from markingMethod in info_Editor.MarkingMethods
                                           where markingMethod.IsEditorAction
                                           select markingMethod;
                return editorMarkingMethods.ToArray();
            }
            else
                return new MarkingMethod[0];
        }
    }
}