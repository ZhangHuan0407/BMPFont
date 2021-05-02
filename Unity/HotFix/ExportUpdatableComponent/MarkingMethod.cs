/*
 * ExportUpdatableComponent 和 UnityEditor 公用代码
 * 请确保两个项目中的 MarkingMethod 一致性
 */
using System;

#if UNITY_EDITOR
namespace Encoder.Editor
#else
namespace ExportUpdatableComponent
#endif
{
    /// <summary>
    /// 受到标记的方法
    /// </summary>
    [Serializable]
    public class MarkingMethod : IComparable<MarkingMethod>
    {
        /* field */
        /// <summary>
        /// 编辑器可调用行为
        /// </summary>
        public bool IsEditorAction { get; set; }
        /// <summary>
        /// 运行时可调用行为
        /// </summary>
        public bool IsRuntimeAction { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 面板及其它描述中对象的代称
        /// </summary>
        public string Title { get; set; }

        /* ctor */

        /* func */

        /* IComparable */
        public int CompareTo(MarkingMethod other)
        {
            if (other is null)
                return 1;
            else
                return Name.CompareTo(other.Name);
        }
    }
}