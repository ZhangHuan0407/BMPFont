using System.Collections.Generic;

namespace HotFix.EncoderExtend
{
    public static class DictionaryExtend
    {
        /// <summary>
        /// 从字典中尝试获取并推出数据
        /// </summary>
        /// <param name="dictionary">数据源</param>
        /// <param name="key">查找键</param>
        /// <param name="result">推出值，当字典中的数据无效时使用类型默认值</param>
        public static void TryPushValue<TKey, TValue, TResult>(this Dictionary<TKey, TValue> dictionary, TKey key, out TResult result)
        {
            if (dictionary.TryGetValue(key, out TValue value_object)
                && value_object is TResult value_result)
                result = value_result;
            else
                result = default(TResult);
        }
    }
}