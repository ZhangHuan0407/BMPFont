using System;

namespace Encoder
{
    /// <summary>
    /// <see cref="Encoder"/> 所必须的方法，没能在热更新代码指定类型中找到
    /// </summary>
    public class NotFoundBindingMethodException : Exception
    {
        /* ctor */
        public NotFoundBindingMethodException()
        {
        }

        public NotFoundBindingMethodException(string message) : base(message)
        {
        }

        public NotFoundBindingMethodException(string typeName, string methodName) :
            base($"Not found binding method in Hotfix. Method : {typeName}.{methodName}")
        {
        }
    }
}