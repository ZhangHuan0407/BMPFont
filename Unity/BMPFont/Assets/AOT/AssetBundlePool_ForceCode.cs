using System;
using AssetBundleUpdate;
using UnityEngine;

namespace Encoder.AOT
{
    public static class AssetBundlePool_ForceCode
    {
        public static void LoadAsset_GameObject(string assetBundleName, string name, Action<GameObject> callback) =>
            AssetBundlePool.LoadAsset(assetBundleName, name, callback);
        public static void LoadAsset_TextAsset(string assetBundleName, string name, Action<TextAsset> callback) =>
            AssetBundlePool.LoadAsset(assetBundleName, name, callback);
        public static void LoadAsset_Sprite(string assetBundleName, string name, Action<Sprite> callback) =>
            AssetBundlePool.LoadAsset(assetBundleName, name, callback);
        public static void LoadAsset_Material(string assetBundleName, string name, Action<Material> callback) =>
            AssetBundlePool.LoadAsset(assetBundleName, name, callback);
        public static void LoadAsset_Asset(string assetBundleName, string name, Action<ScriptableObject> callback) =>
            AssetBundlePool.LoadAsset(assetBundleName, name, callback);
        public static void LoadAsset_Font(string assetBundleName, string name, Action<Font> callback) =>
            AssetBundlePool.LoadAsset(assetBundleName, name, callback);
    }
}