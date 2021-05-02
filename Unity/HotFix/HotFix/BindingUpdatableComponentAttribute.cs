using Encoder;
using System;
using System.Collections.Generic;

namespace HotFix
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class BindingUpdatableComponentAttribute : Attribute
    {
        /* const */
        /// <summary>
        /// 普通组件类型名称
        /// </summary>
        public const string CommonComponent = "Encoder.CommonUpdatableComponent";
        /// <summary>
        /// 逻辑控制组件类型名称
        /// </summary>
        public const string LogicComponent = "Encoder.LogicUpdatableComponent";
        /// <summary>
        /// 碰撞触发组件类型名称
        /// </summary>
        public const string Collision2DComponent = "Encoder.Collision2DUpdatableComponent";
        /// <summary>
        /// 容器组件类型名称
        /// </summary>
        public const string ContainerComponent = "Encoder.ContainerUpdatableComponent";

        /* field */
        /// <summary>
        /// 绑定的可更新组件类型名称
        /// </summary>
        public readonly string BindingComponentName;

        /* ctor */
        public BindingUpdatableComponentAttribute(string bindingComponentName)
        {
            if (string.IsNullOrWhiteSpace(bindingComponentName))
                throw new ArgumentException($"“{nameof(bindingComponentName)}”不能为 Null 或空白", nameof(bindingComponentName));

            BindingComponentName = bindingComponentName;
        }
    }
}