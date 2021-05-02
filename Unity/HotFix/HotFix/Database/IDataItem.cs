namespace HotFix.Database
{
    /// <summary>
    /// 一个存在主键的数据项
    /// </summary>
    /// <typeparam name="TKey">主键</typeparam>
    public interface IDataItem<TKey>
    {
        /* field */
        /// <summary>
        /// 此数据表中唯一主键
        /// </summary>
        TKey PrimaryKey { get; }

        /* func */
        /// <summary>
        /// 两数据项主键相同，且在同一表中
        /// </summary>
        /// <param name="dataItem">另一个数据项</param>
        /// <returns>主键相同，且在同一表中</returns>
        bool DataItemIsEqual(IDataItem<object> dataItem);
    }
}