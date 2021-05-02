using System;
using System.Collections.Generic;

namespace HotFix.Database
{
    /// <summary>
    /// 数据反序列化缓冲
    /// </summary>
    internal class SetDataFunctionBuffer
    {
        /* const */
        internal static readonly Dictionary<Type, Func<string, object>> ParseDictionary =
            new Dictionary<Type, Func<string, object>>()
        {
            { typeof(long),        (str) => long.Parse(str) },
            { typeof(int),         (str) => int.Parse(str) },
            { typeof(short),       (str) => short.Parse(str) },
            { typeof(float),       (str) => float.Parse(str) },
            { typeof(double),      (str) => double.Parse(str) },
            { typeof(string),      (str) => str.Trim() },
            { typeof(bool),        (str) => bool.Parse(str) },
            { typeof(decimal),     (str) => decimal.Parse(str) },
        };

        /* field */
        internal readonly Func<string, object> ParseFunction;
        internal readonly Action<object, object> SetValueFunction;

        /* ctor */
        internal SetDataFunctionBuffer(Type valueType, Action<object, object> setValueFunction)
        {
            ParseFunction = ParseDictionary[valueType];
            SetValueFunction = setValueFunction ?? throw new ArgumentNullException(nameof(setValueFunction));
        }
    }
}