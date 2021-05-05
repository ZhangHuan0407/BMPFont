using System;
using System.Collections.Generic;
using UnityEngine;

namespace HotFix.Database
{
    public class PrefabAsset : IDataItem<string>
    {
        /* field */
        public readonly string PrefabName;
        public readonly GameObject Prefab;

        /* inter */
        public string PrimaryKey => PrefabName;

        /* ctor */
        public PrefabAsset() { }
        public PrefabAsset(string prefabName, GameObject prefab)
        {
            PrefabName = prefabName;
            Prefab = prefab;
        }

        /* func */

        /* IDataItem */
        public bool DataItemIsEqual(IDataItem<object> dataItem)
        {
            if (dataItem is PrefabAsset prefabAsset)
                return PrimaryKey.Equals(prefabAsset.PrimaryKey);
            else
                return false;
        }
    }
}