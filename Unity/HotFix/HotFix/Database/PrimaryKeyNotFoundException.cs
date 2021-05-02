using System;

namespace HotFix.Database
{
    /// <summary>
    /// 表中必要主键没有找到时引发此异常
    /// </summary>
    public class PrimaryKeyNotFoundException : Exception
    {
        /* field */

        /* ctor */
        /// <summary>
        /// 表中必要主键没有找到时引发此异常
        /// </summary>
        /// <param name="message">详细信息</param>
        public PrimaryKeyNotFoundException(string message) : base(message) { }
    }
}