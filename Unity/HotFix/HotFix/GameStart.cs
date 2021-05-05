using AssetBundleUpdate;
using Encoder;
using HotFix.Database;
using HotFix.EncoderExtend;
using HotFix.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace HotFix
{
    public class GameStart
    {
        /* field */
        private List<IEnumerator<object>> CreateInstances;
        private List<IEnumerator<object>> StartInstance;

        /* ctor */
        /// <summary>
        /// 返回游戏启动实例，此类型必须包含无参公开构造函数
        /// </summary>
        public GameStart()
        {
            CreateInstances = new List<IEnumerator<object>>()
            {
                Setting.CreateInstance(),
                GameSystemData.CreateInstance(),
            };

            StartInstance = new List<IEnumerator<object>>()
            {
                Setting.StartInstance(),
                GameSystemData.StartInstance(),
            };

            Coroutine coroutine = ILRuntimeService.StartILCoroutine(WaitForAll_Coroutine());
        }
        private IEnumerator<object> WaitForAll_Coroutine()
        {
            foreach (IEnumerator<object> enumerator in CreateInstances)
                ILRuntimeService.StartILCoroutine(enumerator);
            foreach (IEnumerator<object> enumerator in CreateInstances)
            {
                while (enumerator.MoveNext())
                    yield return enumerator.Current;
            }

            foreach (IEnumerator<object> enumerator in StartInstance)
                ILRuntimeService.StartILCoroutine(enumerator);
            foreach (IEnumerator<object> enumerator in StartInstance)
            {
                while (enumerator.MoveNext())
                    yield return enumerator.Current;
            }
        }

        /* func */
        public void Main()
        {
            Debug.Log("Invoke GameStart.Main.");
            int task = 0;
            List<Type> windows = new List<Type>()
            {
                typeof(FileAndDirectoryWindow),
                typeof(FontSettingWindow),
                typeof(MakeSureWindow),
                typeof(MenuWindow),
                typeof(ProcessWindow),
                typeof(RendererWindow),
            };

            Table<string, PrefabAsset> prefabCache = new Table<string, PrefabAsset>()
            {
                Name = "PrefabCache",
            };
            GameSystemData.Instance.Add(prefabCache);
            foreach (Type window in windows)
            {
                task++;
                AssetBundlePool.LoadAsset<GameObject>(
                    "bmpfont_prefab.assetbundle",
                    window.Name,
                    (gameObject) =>
                    {
                        prefabCache.Insert(new PrefabAsset(window.Name, gameObject));
                        task--;
                    });
            }

            ILRuntimeService.StartILCoroutine(WaitAllLoaded());
            IEnumerator<object> WaitAllLoaded()
            {
                while (task > 0)
                    yield return null;
            }
        }
    }
}