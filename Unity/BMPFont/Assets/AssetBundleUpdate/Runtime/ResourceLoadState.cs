namespace AssetBundleUpdate
{
    public enum ResourceLoadState
    {
        /// <summary>
        /// 资源包尚未加载完成
        /// </summary>
        WaitForAssetBundleLoaded,
        /// <summary>
        /// 资源或资源包尚未加载完成
        /// </summary>
        WaitForAssetLoaded,
        /// <summary>
        /// Runtime 模式下找不到资源包
        /// </summary>
        AssetBundleNotFound,
        /// <summary>
        /// Editor 模式下找不到目标资源
        /// </summary>
        AssetNotFound,
        /// <summary>
        /// 资源加载成功
        /// </summary>
        LoadSuccessful,
    }
}