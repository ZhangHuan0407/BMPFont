using System;

namespace Encoder
{
    /// <summary>
    /// <see cref="Encoder"/> 所必须的类型，没能在热更新代码中找到指定类型
    /// </summary>
    public class NotFoundBindingTypeException : Exception
    {
        /* ctor */
        public NotFoundBindingTypeException()
        {
        }

        public NotFoundBindingTypeException(string message) : base(message)
        {
        }

        public NotFoundBindingTypeException(string typeName, string message) :
            base($"Not found binding type in Hotfix. TypeName : {typeName}.\n{message}")
        {
        }
    }
}