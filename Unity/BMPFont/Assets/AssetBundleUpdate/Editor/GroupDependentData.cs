using System;
using System.Collections.Generic;

namespace AssetBundleUpdate.Editor
{
    public class GroupDependentData
    {
        /* field */
        /// <summary>
        /// 资源包名称
        /// </summary>
        public readonly string AssetBundleName;
        /// <summary>
        /// 资源包实际最终依赖
        /// </summary>
        public readonly string[] AllDependencies;
        /// <summary>
        /// 未知是否允许依赖的资源包
        /// </summary>
        public readonly List<string> UnknownDependencies;
        public readonly List<string> ForbiddenDependencies;

        /* inter */
        /// <summary>
        /// 当前资源包名称中，第一个匹配的组名称
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 当前资源包名称中，第一个匹配的项名称
        /// </summary>
        public string ItemName { get; set; }

        /* ctor */
        public GroupDependentData(string assetBundleName, string[] allDependencies)
        {
            if (string.IsNullOrWhiteSpace(assetBundleName))
                throw new ArgumentException($"{nameof(assetBundleName)} is null or white space.");
            AssetBundleName = assetBundleName;
            AllDependencies = allDependencies ?? throw new ArgumentNullException(nameof(allDependencies));
            UnknownDependencies = new List<string>(allDependencies);
            ForbiddenDependencies = new List<string>();
        }

        /* func */
        public void AnalyticsByGroupRule(GroupDependentRule groupDependentRule)
        {
            if (groupDependentRule is null)
                throw new ArgumentNullException(nameof(groupDependentRule));

            throw new NotImplementedException();
        }
        public void AnalyticsByPieceRule(PieceDependentRule pieceDependentRule)
        {
            if (pieceDependentRule is null)
                throw new ArgumentNullException(nameof(pieceDependentRule));

            if (!pieceDependentRule.MatchName.IsMatch(AssetBundleName))
                return;

            throw new NotImplementedException();
        }
    }
}