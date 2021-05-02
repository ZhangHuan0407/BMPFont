using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix.Database
{
    /// <summary>
    /// 一张强类型数据表
    /// <para>基于主键的访问倾向于O(1)操作，基于数据的访问倾向于O(n)操作</para>
    /// </summary>
    /// <typeparam name="TKey">主键类型</typeparam>
    /// <typeparam name="TData">数据类型</typeparam>
    public class Table<TKey, TData> : ITable where TData : class, IDataItem<TKey>, new()
    {
        /* field */
        /// <summary>
        /// 数据集合
        /// </summary>
        protected Dictionary<TKey, TData> DataSet;
        private string m_Name;
        /// <summary>
        /// 表名称，同一表集合中表名称唯一
        /// </summary>
        public string Name
        {
            get => m_Name;
            private set 
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException($"{nameof(value)} can not be null or white space.");
                m_Name = value;
            }
        }
        /// <summary>
        /// 数据类型
        /// </summary>
        public Type DataType { get; }
        /// <summary>
        /// 所属表集合
        /// </summary>
        TableSet ITable.TableSet { get; set; }
        /// <summary>
        /// 当前表处于只读状态
        /// </summary>
        public bool ReadOnly { get; private set; }

        /* inter */
        /// <summary>
        /// 表中数据的数量
        /// </summary>
        public int Count => DataSet.Count;

        public TData this[TKey primartKey]
        {
            get
            {
                if (ContainsKey(primartKey))
                    return Select(primartKey);
                else
                    return null;
            }
            set
            {
                if (value is null)
                {
                    if (ContainsKey(primartKey))
                        Delete(primartKey);
                }
                else
                {
                    if (ContainsKey(primartKey))
                        Update(value);
                    else
                        Insert(value);
                }
            }
        }

        /* ctor */
        public Table()
        {
            DataSet = new Dictionary<TKey, TData>();
            DataType = typeof(TData);
        }

        /* func */
        /// <summary>
        /// 表是否包含此键值对
        /// </summary>
        /// <param name="value">键值对</param>
        /// <returns>包含此键值对</returns>
        public bool Contains(KeyValuePair<TKey, TData> value) => DataSet.Contains(value);
        /// <summary>
        /// 表是否包含此键
        /// </summary>
        /// <param name="primaryKey">主键</param>
        /// <returns>包含此键</returns>
        public bool ContainsKey(TKey primaryKey) => DataSet.ContainsKey(primaryKey);
        /// <summary>
        /// 表是否包含此值
        /// </summary>
        /// <param name="value">数据值</param>
        /// <returns>包含此值</returns>
        public bool ContainsValue(IDataItem<TKey> value) => value is TData t && DataSet.ContainsValue(t);

        public virtual void Insert(params TData[] dataItems)
        {
            if (dataItems is null)
                throw new ArgumentNullException(nameof(dataItems));
            if (ReadOnly)
                throw new ReadOnlyException($"Try to {nameof(Insert)} data from table {Name}, but it is ReadOnly.");

            foreach (TData dataItem in dataItems)
            {
                if (dataItem is null)
                    throw new NullReferenceException(nameof(dataItem));
                DataSet.Add(dataItem.PrimaryKey, dataItem);
            }
        }
        public virtual TData Select(TKey primaryKey)
        {
            DataSet.TryGetValue(primaryKey, out TData dataItem);
            return dataItem;
        }
        public virtual IEnumerable<TData> Select(Predicate<TData> selectedFunction)
        {
            if (selectedFunction is null)
                throw new ArgumentNullException(nameof(selectedFunction));

            var selectedDataItems = from dataItem in DataSet.Values
                                    where selectedFunction(dataItem)
                                    select dataItem;
            return selectedDataItems;
        }
        public virtual void Update(params TData[] newDataItems)
        {
            if (newDataItems is null)
                throw new ArgumentNullException(nameof(newDataItems));
            else if (ReadOnly)
                throw new ReadOnlyException($"Try to {nameof(Update)} data from table {Name}, but it is ReadOnly.");

            foreach (TData newdataItem in newDataItems)
            {
                if (newdataItem is null)
                    throw new NullReferenceException(nameof(newdataItem));

                DataSet[newdataItem.PrimaryKey] = newdataItem;
            }
        }
        public virtual void Delete(params TKey[] primaryKeys)
        {
            if (primaryKeys is null)
                throw new ArgumentNullException(nameof(primaryKeys));
            else if (ReadOnly)
                throw new ReadOnlyException($"Try to {nameof(Delete)} data from table {Name}, but it is ReadOnly.");

            // TKey 无约束，无法检测 null
            foreach (TKey primaryKey in primaryKeys)
            {
                DataSet.Remove(primaryKey);
            }
        }
        public virtual void Delete(params TData[] dataItems)
        {
            if (dataItems is null)
                throw new ArgumentNullException(nameof(dataItems));
            else if (ReadOnly)
                throw new ReadOnlyException($"Try to {nameof(Delete)} data from table {Name}, but it is ReadOnly.");

            foreach (TData dataItem in dataItems)
            {
                if (dataItem is null)
                    throw new NullReferenceException(nameof(dataItem));

                DataSet.Remove(dataItem.PrimaryKey);
            }
        }

        public static Task<Table<TKey, TData>> LoadTableFromCSV(string name, TextAsset textAsset)
        {
            throw new NotImplementedException("ILRuntime MultiTask Exception.");
            //string text = textAsset.text;
            //return Task.Run(() =>
            //{
            //    return LoadTableFromCSV<TData>(text);
            //});
        }
        public static Table<TKey, TData> LoadTableFromCSV(string name, string textContent)
        {
            if (textContent is null)
                throw new ArgumentNullException(nameof(textContent));

            Table<TKey, TData> table = new Table<TKey, TData>();
            table.Name = name;

            // 获取首行
            string[] lines = textContent.Split('\n');
            if (lines.Length == 0)
                throw new ArgumentException(nameof(lines));
            string[] memberNames = lines[0].Split(',');
            if (memberNames.Length == 0)
                throw new ArgumentException(nameof(memberNames));
            
            // 获取赋值缓冲函数
            SetDataFunctionBuffer[] functionBuffers = new SetDataFunctionBuffer[memberNames.Length];
            Type dataItemType = typeof(TData);
            for (int columnIndex = 0; columnIndex < memberNames.Length; columnIndex++)
            {
                string memberName = memberNames[columnIndex] ?? throw new NullReferenceException($"{dataItemType.Name}'s table [{columnIndex}] is null in first row.");
                memberName = memberName.Trim();
                if (dataItemType.GetField(
                    memberName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) is FieldInfo fieldInfo)
                    functionBuffers[columnIndex] = new SetDataFunctionBuffer(fieldInfo.FieldType, fieldInfo.SetValue);
                else if (dataItemType.GetProperty(
                    memberName,
                    BindingFlags.Public | BindingFlags.Instance) is PropertyInfo propertyInfo)
                    functionBuffers[columnIndex] = new SetDataFunctionBuffer(propertyInfo.PropertyType, propertyInfo.SetValue);
                else
                    throw new ArgumentNullException($"Not found field or property named {memberName}");
            }

            // 解析数据
            for (int lineIndex = 1; lineIndex < lines.Length; lineIndex++)
            {
                string[] itemString = lines[lineIndex].Split(',');
                TData data = new TData();
                for (int itemIndex = 0; itemIndex < itemString.Length; itemIndex++)
                {
                    SetDataFunctionBuffer functionBuffer = functionBuffers[itemIndex];
                    object value = functionBuffer.ParseFunction(itemString[itemIndex]);
                    functionBuffer.SetValueFunction(data, value);
                }
                table.Insert(data);
            }

            return table;
        }
        
        private class JsonTable<_TData> where _TData : class, IDataItem<TKey>, new()
        {
            internal _TData[] DataSet;
        }
        public static Task<Table<TKey, TData>> LoadTableFromJson(string name, TextAsset textAsset)
        {
            throw new NotImplementedException("ILRuntime MultiTask Exception.");
            //string text = textAsset.text;
            //return Task.Run(() =>
            //{
            //    return LoadTableFromJson<TData>(text);
            //});
        }
        public static Table<TKey, TData> LoadTableFromJson(string name, string textContent)
        {
            if (textContent is null)
                throw new ArgumentNullException(nameof(textContent));

            Table<TKey, TData> table = new Table<TKey, TData>();
            table.Name = name;

            JsonTable<TData> jsonTable = JsonConvert.DeserializeObject<JsonTable<TData>>(textContent);
            table.Insert(jsonTable.DataSet);
            return table;
        }

        /* IEnumerable */
        public IEnumerator<TData> GetEnumerator() => DataSet.Values.GetEnumerator();
        public IEnumerable<TData> GetEnumerable() => DataSet.Values;

        /* IDataItem */
        string IDataItem<string>.PrimaryKey => Name;
        bool IDataItem<string>.DataItemIsEqual(IDataItem<object> dataItem)
        {
            ITable table = dataItem as ITable;
            if (table is null)
                return false;
            else if ((this as ITable).TableSet is null)
                return false;
            else if ((this as ITable).TableSet != table.TableSet)
                return false;
            else
                return Name.Equals(table.Name);
        }

        /* ITable */
        void ITable.Truncate(bool ignoreReference)
        {
            if (ReadOnly)
                throw new ReadOnlyException($"Try to {nameof(ITable.Truncate)} data from table {Name}, but it is ReadOnly.");

            DataSet.Clear();
        }
        bool ITable.ContainsKey(object primaryKey)
        {
            if (primaryKey is TKey key)
                return ContainsKey(key);
            else
                return false;
        }
        bool ITable.ContainsValue(IDataItem<object> value)
        {
            if (value is IDataItem<TKey> data)
                return ContainsValue(data);
            else
                return false;
        }
    }
}
