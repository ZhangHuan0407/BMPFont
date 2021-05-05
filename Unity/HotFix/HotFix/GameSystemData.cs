using AssetBundleUpdate;
using HotFix.Database;
using HotFix.EncoderExtend;
using System;
using System.Collections;
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

        /* inter */
        public Table<string, LanguageAssetBundle> LanguageAB => this[nameof(LanguageAssetBundle)] as Table<string, LanguageAssetBundle>;
        public Table<string, LanguageTranslate> InterfaceString => this[nameof(InterfaceString)] as Table<string, LanguageTranslate>;
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
            AssetBundlePool.LoadAsset<TextAsset>(
                "updatablelogic/hotfix.assetbundle",
                "LanguageAssetBundle.csv",
                (textAsset) =>
                {
                    ITable languageAB_Table = Table<string, LanguageAssetBundle>.LoadTableFromCSV(nameof(LanguageAssetBundle), textAsset.text);
                    instance.Add(languageAB_Table);
                    task--;
                });

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

            // 加载语言包
            LanguageAssetBundle languageAB = Instance.LanguageAB[Setting.Instance.Language];
            AssetBundlePool.LoadAssetBundle(languageAB.InterfaceLanguageAssetBundleName, true);
            AssetBundlePool.LoadAsset<TextAsset>(
                languageAB.InterfaceLanguageAssetBundleName,
                "Interface.json",
                (textAsset) =>
                {
                    // Newston 无法获取 Table 数据类型
                    //ITable interfaceString_Table = Table<string, LanguageTranslate>.LoadTableFromJson(nameof(InterfaceString), textAsset.text);
                    //Instance.Add(interfaceString_Table);
                    task--;
                });

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