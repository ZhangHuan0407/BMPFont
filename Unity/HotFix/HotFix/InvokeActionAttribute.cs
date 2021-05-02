using System;

namespace HotFix
{
    /// <summary>
    /// 公开实例行为
    /// <para>仅 Public Instance Action 的标记为有效标记</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class InvokeActionAttribute : Attribute
    {
        /* field */
        /// <summary>
        /// 此字段或属性在面板上显示的名称
        /// <para>默认为方法声明名称</para>
        /// <para>实际序列化保存的名称是字段或属性的声明名称</para>
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 是 Editor 下的公开实例行为，代表可以在面板上直接调用此方法
        /// <para>默认值 false</para>
        /// </summary>
        public bool IsEditorAction { get; set; }
        /// <summary>
        /// 是 Runtime 下的公开实例行为
        /// <para>默认值 false</para>
        /// </summary>
        public bool IsRuntimeAction { get; set; }

        /* ctor */
        /// <summary>
        /// 声明一个公开实例行为实例，它具有默认值
        /// </summary>
        public InvokeActionAttribute()
        {
            IsEditorAction = false;
            IsRuntimeAction = false;
        }
    }
}