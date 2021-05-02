using AssetBundleUpdate;
using Encoder;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotFix.EncoderExtend
{
    public static class AssetBundlePoolExtend
    {
        /* func */
        /// <summary>
        /// 开启携程加载更新资源包
        /// </summary>
        /// <param name="reference">添加资源包引用计数</param>
        /// <param name="assetBundleNames">资源包名称</param>
        /// <returns>携程</returns>
        public static Coroutine UpdateAssetBundle(this AssetBundlePool instance, bool reference, params string[] assetBundleNames)
        {
            return ILRuntimeService.StartILCoroutine(UpdateAssetBundlesOneByOne().GetEnumerator());
            IEnumerable<object> UpdateAssetBundlesOneByOne()
            {
                foreach (string assetBundleName in assetBundleNames)
                {
                    AssetBundlePool.UpdateAssetBundle(assetBundleName);
                    while (!AssetBundlePool.AssetBundleIsReady(assetBundleName))
                        yield return null;
                }
            }
        }
    }
}