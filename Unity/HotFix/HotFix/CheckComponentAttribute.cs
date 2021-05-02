using System;

namespace HotFix
{
    /// <summary>
    /// 检查组件特性
    /// <para>当通过检查点时会检查此 GameObject 上挂载的组件是否满足预定</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class CheckComponentAttribute : Attribute
    {
        /* field */
        public Type[] RequiredComponents { get; private set; }

        /* ctor */
        public CheckComponentAttribute(params Type[] types)
        {
            if (types.Length == 0)
                throw new ArgumentException(nameof(types));
            RequiredComponents = types;
        }

        /* func */
    }
}