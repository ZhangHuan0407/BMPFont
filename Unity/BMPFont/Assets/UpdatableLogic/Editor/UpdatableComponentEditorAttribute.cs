using System;

namespace Encoder.Editor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UpdatableComponentEditorAttribute : Attribute
    {
        /* field */
        /// <summary>
        /// 指向一个 <see cref="UpdatableComponent"/> 运行时指向的包装类型名称，
        /// 要求提供命完整限定类名，切程序集内不能有两个类绑定同一个类名。
        /// </summary>
        public readonly string UpdatableComponentTypeName;

        /* ctor */
        public UpdatableComponentEditorAttribute(string updatableComponentTypeName)
        {
            if (string.IsNullOrWhiteSpace(updatableComponentTypeName))
                throw new ArgumentException($"{nameof(updatableComponentTypeName)} can't be null or white space.");
            UpdatableComponentTypeName = updatableComponentTypeName;
        }

        /* func */

    }
}