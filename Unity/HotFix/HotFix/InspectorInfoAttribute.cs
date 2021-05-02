using System;

namespace HotFix
{
    /// <summary>
    /// 公开面板信息
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class InspectorInfoAttribute : Attribute
    {
        /* field */
        /// <summary>
        /// 此字段或属性在面板上显示的名称
        /// <para>默认为字段或属性的声明名称</para>
        /// <para>实际序列化保存的名称是字段或属性的声明名称</para>
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 当前字段在面板上的排序，默认值-1
        /// <para>小于 0 将被转为反射时获取到的顺序</para>
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// 是否序列化此字段或属性
        /// <para>Field : Public 默认为 SerializeIt, NonPublic 默认为 ShowInInspectorOnly</para>
        /// <para>Property : Public 默认为 SerializeIt, NonPublic 拒绝序列化</para>
        /// </summary>
        public ItemSerializableState State { get; set; }
        /// <summary>
        /// 此字段或属性在面板上显示的提示信息
        /// </summary>
        public string ToolTip { get; set; }

        /* inter */

        /* ctor */
        /// <summary>
        /// 声明一个公开面板信息实例，它具有默认值
        /// </summary>
        public InspectorInfoAttribute()
        {
            Order = -1;
            State = ItemSerializableState.Default;
        }

        /* func */

    }
}
