using System;

namespace HotFix.Database
{
    /// <summary>
    /// 抽象表
    /// </summary>
    public interface ITable : IDataItem<string>
    {
        /* field */
        /// <summary>
        /// 表名称，同一表集合中表名称唯一
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 表中数据的数量
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 数据类型
        /// </summary>
        Type DataType { get; }
        /// <summary>
        /// 所属表集合
        /// </summary>
        internal TableSet TableSet { get; set; }

        /* func */
        /// <summary>
        /// 表是否包含此键
        /// </summary>
        /// <param name="primaryKey">主键</param>
        /// <returns>包含此键</returns>
        bool ContainsKey(object primaryKey);
        /// <summary>
        /// 表是否包含此值
        /// </summary>
        /// <param name="value">数据值</param>
        /// <returns>包含此值</returns>
        bool ContainsValue(IDataItem<object> value);

        /// <summary>
        /// 清空表所有数据
        /// </summary>
        /// <param name="ignoreReference">忽略引用计数</param>
        void Truncate(bool ignoreReference);
    }
}