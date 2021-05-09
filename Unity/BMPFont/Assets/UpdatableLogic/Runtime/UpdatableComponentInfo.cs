using System.Collections.Generic;

namespace Encoder
{
    public class UpdatableComponentInfo
    {
        /* field */
        /// <summary>
        /// 绑定的 UpdatableComponent 子类型的具体名称
        /// </summary>
        public string BindingComponentName;
        /// <summary>
        /// HotFix 命名空间下具体类型的名称
        /// </summary>
        public string TypeFullName;
        public HashSet<string> SerializeItems;
        public HashSet<string> MarkingMethods;

        /* ctor */
        public UpdatableComponentInfo()
        {
        }
    }
}