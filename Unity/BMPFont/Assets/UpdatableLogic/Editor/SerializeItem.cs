/*
 * ExportUpdatableComponent 和 UnityEditor 公用代码
 * 请确保两个项目中的 SerializeItem 一致性
 */
using System;

#if UNITY_EDITOR
namespace Encoder.Editor
#else
namespace ExportUpdatableComponent
#endif
{
    [Serializable]
    public class SerializeItem : IComparable<SerializeItem>
    {
        /* field */
        /// <summary>
        /// 对象是一个字段
        /// </summary>
        public bool IsField { get; set; }
        /// <summary>
        /// 对象是一个公开属性
        /// </summary>
        public bool IsProperty { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 面板及其它描述中对象的代称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 对象内容说明
        /// </summary>
        public string ToolTip { get; set; }
        /// <summary>
        /// 对象声明类型
        /// </summary>
        public string TypeFullName { get; set; }
        public int Order { get; set; }
        /// <summary>
        /// 启用 Unity 序列化
        /// </summary>
        public bool UseUnitySerialize { get; set; }
        /// <summary>
        /// 启用 Json 序列化
        /// </summary>
        public bool UseJsonSerialize { get; set; }

        /* ctor */

        /* func */

        /* IComparable */
        public int CompareTo(SerializeItem other)
        {
            if (other is null)
                return 1;
            else
                return Order.CompareTo(other.Order);
        }
    }
}