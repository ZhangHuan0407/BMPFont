/*
 * ExportUpdatableComponent 和 UnityEditor 公用代码
 * 请确保两个项目中的 CustomComponentInfo_Editor 一致性
 */
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
namespace Encoder.Editor
#else
namespace ExportUpdatableComponent
#endif
{
    /// <summary>
    /// 通过反射导出的自定义组件信息
    /// </summary>
    [Serializable]
    public class CustomComponentInfo_Editor
    {
        /* field */
        /// <summary>
        /// 绑定的 UpdatableComponent 子类型的具体名称
        /// </summary>
        public string BindingComponentName { get; set; }
        /// <summary>
        /// HotFix 命名空间下具体类型的名称
        /// </summary>
        public string TypeFullName { get; set; }
        /// <summary>
        /// 进行序列化的字段和属性信息
        /// </summary>
        public List<SerializeItem> SerializeItems { get; set; }
        /// <summary>
        /// 进行标记的可调用行为
        /// </summary>
        public List<MarkingMethod> MarkingMethods { get; set; }

        /* ctor */
        public CustomComponentInfo_Editor()
        {
            SerializeItems = new List<SerializeItem>();
            MarkingMethods = new List<MarkingMethod>();
        }

        /* func */

    }
}