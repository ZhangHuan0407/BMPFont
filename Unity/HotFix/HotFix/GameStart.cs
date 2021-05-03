using AssetBundleUpdate;
using Encoder;
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
            
            task++;
            AssetBundlePool.LoadAsset<GameObject>(
                "bmpfont_prefab.assetbundle",
                nameof(MakeSureWindow),
                (gameObject) =>
                {
                    MakeSureWindow.Prefab = gameObject;
                    task--;
                });

            task++;
            AssetBundlePool.LoadAsset<GameObject>(
                "bmpfont_prefab.assetbundle",
                nameof(FileAndDirectoryWindow),
                (gameObject) =>
                {
                    FileAndDirectoryWindow.Prefab = gameObject;
                    task--;
                    FileAndDirectoryWindow.OpenWindow(
                        new System.IO.DirectoryInfo("A:/BMPFont/"),
                        (FileAndDirectoryWindow window) => Debug.Log($"OK {window.SelectedFileSystemInfo}\n{window.SelectedFileSystemInfo?.FullName}"),
                        (FileAndDirectoryWindow window) => Debug.Log($"Cancle {window.SelectedFileSystemInfo}\n{window.SelectedFileSystemInfo?.FullName}")
                        );
                });
        }
    }
}