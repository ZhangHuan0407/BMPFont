using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Encoder.Editor
{
    /// <summary>
    /// 缓存反序列化的<see cref="CustomComponentInfo_Editor"/>信息
    /// </summary>
    public static class CustomComponentBuffer_Editor
    {
        /* const */
        /// <summary>
        /// Editor 自定义组件导出数据路径
        /// </summary>
        public const string CustomComponentInfo_EditorAssetPath = "Assets/UpdatableLogic/Editor/CustomComponentInfo_Editor.json";

        /* field */
        public static Lazy<Dictionary<string, CustomComponentInfo_Editor>> ComponentInfoCache =
            new Lazy<Dictionary<string, CustomComponentInfo_Editor>>(GetComponentInfoCache);

        /* inter */

        /* ctor */

        /* func */
        private static Dictionary<string, CustomComponentInfo_Editor> GetComponentInfoCache()
        {
            Dictionary<string, CustomComponentInfo_Editor> result = new Dictionary<string, CustomComponentInfo_Editor>();
            if (!File.Exists(CustomComponentInfo_EditorAssetPath))
            {
                Debug.LogError("Not Found CustomComponentInfo_Editor.json.");
                return result;
            }

            string content = File.ReadAllText(CustomComponentInfo_EditorAssetPath);
            CustomComponentInfo_Editor[] customComponentInfos_Editor = JsonConvert.DeserializeObject<CustomComponentInfo_Editor[]>(content);
            foreach (CustomComponentInfo_Editor customComponentInfo_Editor in customComponentInfos_Editor)
                result.Add(customComponentInfo_Editor.TypeFullName, customComponentInfo_Editor);
            return result;
        }
        /// <summary>
        /// 清除当前的自定义组件导出数据的实例缓存
        /// <para>变更 HotFix 代码不会触发数据清除，默认还在使用上一次读入的导出数据，大多数情况下需要主动调用</para>
        /// </summary>
        [MenuItem("Custom Tool/Clear Custom Component/Serialize Item And Marking Method")]
        public static void ClearCache()
        {
            if (ComponentInfoCache.IsValueCreated)
                ComponentInfoCache = new Lazy<Dictionary<string, CustomComponentInfo_Editor>>(GetComponentInfoCache, false);
        }


    }
}