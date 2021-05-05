using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace HotFix.Database
{
    /// <summary>
    /// 多张表组合成表集合
    /// <para>仅仅只是集合而已，不对应数据库的概念</para>
    /// </summary>
    public class TableSet
    {
        /* field */
        /// <summary>
        /// 数据表集名称
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 表集合中表的数量
        /// </summary>
        public int Count => TablesSet.Count;

        /// <summary>
        /// 此表集合是否支持加入或移除表
        /// </summary>
        public bool IsReadOnly { get; set; }

        private readonly Dictionary<string, ITable> TablesSet;

        /* inter */
        public ITable this[string tableName]
        {
            get => TablesSet[tableName];
            set => throw new Exception($"{nameof(TableSet)} not support auto set value operator.");
        }

        /* ctor */
        public TableSet(string name)
        {
            Name = name;
            TablesSet = new Dictionary<string, ITable>();
        }

        /* func */
        public bool Add(ITable table)
        {
            if (table is null)
                throw new ArgumentNullException($"Try to add {nameof(table)} : {table} in {nameof(TableSet)} : {Name}.");
            else if (IsReadOnly)
                throw new ReadOnlyException($"Try to {nameof(Add)} one new table, where {nameof(TableSet)} is ReadOnly.");
            else if (table.TableSet is TableSet anotherSet)
                throw new Exception($"Must {nameof(Remove)} this table from {nameof(TableSet)} before {nameof(Add)} it in another.");

            if (TablesSet.ContainsKey(table.Name))
                return false;
            else
            {
                TablesSet.Add(table.Name, table);
                table.TableSet = this;
                return true;
            }
        }

        public void UnionWith(IEnumerable<ITable> other)
        {
            if (IsReadOnly)
                throw new ReadOnlyException($"Try to {nameof(UnionWith)} other table, where {nameof(TableSet)} is ReadOnly.");

            foreach (ITable table in other)
            {
                if (table.TableSet is TableSet anotherSet
                    && anotherSet != this)
                    throw new Exception($"Must {nameof(Remove)} this table from {nameof(TableSet)} before {nameof(Add)} it in another.");

                if (!TablesSet.ContainsKey(table.Name))
                {
                    TablesSet.Add(table.Name, table);
                    table.TableSet = this;
                }
            }
        }

        //public void IntersectWith(IEnumerable<ITable> other)
        //{
        //    throw new NotImplementedException();
        //}

        public void ExceptWith(IEnumerable<ITable> other)
        {
            if (IsReadOnly)
                throw new ReadOnlyException($"Try to {nameof(ExceptWith)} other table, where {nameof(TableSet)} is ReadOnly.");

            foreach (ITable table in other)
            {
                if (table.TableSet != this)
                    continue;
                if (TablesSet.ContainsKey(table.Name))
                    TablesSet.Remove(table.Name);
                table.TableSet = null;
            }
        }

        //public void SymmetricExceptWith(IEnumerable<ITable> other)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool IsSubsetOf(IEnumerable<ITable> other)
        //{
        //    throw new NotImplementedException();
        //}

        public bool IsSupersetOf(IEnumerable<ITable> other)
        {
            foreach (ITable table in other)
                if (!TablesSet.ContainsKey(table.Name))
                    return false;
            return true;
        }

        //public bool IsProperSupersetOf(IEnumerable<ITable> other)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool IsProperSubsetOf(IEnumerable<ITable> other)
        //{
        //    throw new NotImplementedException();
        //}

        public bool Overlaps(IEnumerable<ITable> other)
        {
            foreach (ITable table in other)
                if (TablesSet.ContainsKey(table.Name))
                    return true;
            return false;
        }

        public bool SetEquals(IEnumerable<ITable> other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 强制清除此数据集内的所有表
        /// </summary>
        public void Clear()
        {
            if (IsReadOnly)
                throw new Exception($"Remove readonly flag, before {nameof(Clear)} {nameof(TableSet)}.");

            foreach (ITable table in TablesSet.Values)
            {
                table.Truncate(true);
                table.TableSet = null;
            }
            TablesSet.Clear();
        }

        public bool Contains(ITable table) => TablesSet.ContainsValue(table);

        public void CopyTo(ITable[] array, int arrayIndex) => TablesSet.Values.CopyTo(array, arrayIndex);

        public bool Remove(ITable table)
        {
            if (IsReadOnly)
                throw new Exception($"Remove readonly flag, before {nameof(Clear)} {nameof(TableSet)}.");

            if (table.TableSet == this
                && TablesSet.Remove(table.Name))
            {
                table.TableSet = null;
                return true;
            }
            else
                return false;
        }
    }
}
