using HotFix.Database;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HotFix
{
    /// <summary>
    /// 游戏系统数据表单缓存
    /// </summary>
    public partial class GameSystemData : TableSet
    {
        /* field */
        public static GameSystemData Instance { get; private set; }

        public BMPFont Font;

        /* inter */
        public Table<string, PrefabAsset> PrefabCache => this[nameof(PrefabCache)] as Table<string, PrefabAsset>;

        /* ctor */
        private GameSystemData() : base(nameof(GameSystemData))
        {
        }
        internal static IEnumerator<object> CreateInstance()
        {
            int task = 0;
            GameSystemData instance = new GameSystemData();

            task++;

            Table<string, PrefabAsset> prefabCache = new Table<string, PrefabAsset>()
            {
                Name = nameof(PrefabCache),
            };
            instance.Add(prefabCache);

            while (task > 0)
                yield return null;
            Instance = instance;
            GameStart.TaskCount--;
        }
        internal static IEnumerator<object> StartInstance()
        {
            int task = 0;

            while (task > 0)
                yield return null;
            GameStart.TaskCount--;
        }

        /* func */
        public GameObject InstantiateGo(string prefabName)
        {
            GameObject go = UnityEngine.Object.Instantiate(PrefabCache[prefabName].Prefab);
            go.transform.SetParent(GameObject.Find("Canvas").transform, false);
            return go;
        }
    }
}