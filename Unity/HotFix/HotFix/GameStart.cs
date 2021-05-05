using AssetBundleUpdate;
using Encoder;
using HotFix.Database;
using HotFix.EncoderExtend;
using HotFix.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Tween;
using UnityEngine;

namespace HotFix
{
    public class GameStart
    {
        /* field */
        private List<IEnumerator<object>> CreateInstances;
        private List<IEnumerator<object>> StartInstance;
        private List<Type> Windows;

        public static int TaskCount { get; set; }

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
            Windows = new List<Type>()
            {
                typeof(FileAndDirectoryWindow),
                typeof(FontSettingWindow),
                typeof(MakeSureWindow),
                typeof(MenuWindow),
                typeof(ProcessWindow),
                typeof(RendererWindow),
            };
        }

        /* func */
        public void Main()
        {
            Debug.Log("Invoke GameStart.Main.");
            Coroutine coroutine = ILRuntimeService.StartILCoroutine(WaitForAll_Coroutine());
        }
        private IEnumerator<object> WaitForAll_Coroutine()
        {
            TaskCount = CreateInstances.Count;
            foreach (IEnumerator<object> enumerator in CreateInstances)
                ILRuntimeService.StartILCoroutine(enumerator);

            while (TaskCount > 0)
                yield return null;
            TaskCount = StartInstance.Count;
            foreach (IEnumerator<object> enumerator in StartInstance)
                ILRuntimeService.StartILCoroutine(enumerator);
            while (TaskCount > 0)
                yield return null;

            TaskCount = Windows.Count;
            foreach (Type window in Windows)
            {
                AssetBundlePool.LoadAsset<GameObject>(
                    "bmpfont_prefab.assetbundle",
                    window.Name,
                    (gameObject) =>
                    {
                        GameSystemData.Instance.PrefabCache.Insert(new PrefabAsset(window.Name, gameObject));
                        TaskCount--;
                    });
            }
            while (TaskCount > 0)
                yield return null;

            FontSettingWindow.OpenWindow();
            MenuWindow.OpenWindow();
            RendererWindow.OpenWindow();
            Debug.Log("return GameStart.WaitForAll_Coroutine.");
        }
    }
}