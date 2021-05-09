using HotFix.Database;
using HotFix.EncoderExtend;
using HotFix.UI;
using HotFix.UI.Component;

namespace HotFix
{
    public class GamePrewarm
    {
        /* const */
        /// <summary>
        /// 预热类型全名称数组，在游戏预热阶段分帧预热对应类型
        /// </summary>
        public readonly string[] PrewarmTypeArray = new string[]
        {
            // EncoderExtend
            typeof(AssetBundlePoolExtend).FullName,
            typeof(EnumerableExtend).FullName,

            // Database
            typeof(IDataItem<int>).FullName,
            typeof(IDataItem<string>).FullName,
            typeof(ITable).FullName,
            typeof(PrimaryKeyNotFoundException).FullName,
            typeof(SetDataFunctionBuffer).FullName,
            typeof(TableSet).FullName,

            // UI

            // UI.Component
            typeof(CountdownText).FullName,
            typeof(UIHorizontalLayout).FullName,
            typeof(UILayout).FullName,
        };

        /// <summary>
        /// 直接加载到内存的资源包，在游戏预热阶段分帧加载。这些资源包可以认为是常驻资源包。
        /// </summary>
        public readonly string[] PreloadAssetBundlesArray = new string[]
        {
            // 资源总体积这么小，
            // 我直接一个滑铲，成为常驻包
            "bmpfont_animator.assetbundle",
            "bmpfont_prefab.assetbundle",
        };

        /* field */

        /* ctor */
        /// <summary>
        /// 返回游戏预热实例，此类型必须包含无参公开构造函数
        /// </summary>
        public GamePrewarm()
        {
        }

        /* func */
        /// <summary>
        /// 返回预热类型全名称数组
        /// </summary>
        public string[] GetPrewarmTypeArray() => PrewarmTypeArray;
        /// <summary>
        /// 返回预加载资源包
        /// </summary>
        public string[] GetPreloadAssetBundlesArray() => PreloadAssetBundlesArray;
    }
}