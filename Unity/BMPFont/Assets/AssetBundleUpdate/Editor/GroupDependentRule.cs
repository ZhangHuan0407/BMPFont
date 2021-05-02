using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AssetBundleUpdate.Editor
{
    [Serializable]
    public class GroupDependentRule
    {
        /* field */
        /// <summary>
        /// 资源包组正则名称
        /// </summary>
        public string MatchGroupName_RegexString;
        /// <summary>
        /// 资源包组项目正则名称
        /// </summary>
        public string MatchItemName_RegexString;
        /// <summary>
        /// 允许依赖资源包转义正则
        /// </summary>
        public string[] DependentOnName_EscapeRegexString;

        private Regex m_MatchGroupName;
        [JsonIgnore]
        public Regex MatchGroupName => m_MatchGroupName = m_MatchGroupName ?? new Regex(MatchGroupName_RegexString, RegexOptions.IgnoreCase);
        private Regex m_MatchItemName;
        [JsonIgnore]
        public Regex MatchItemName => m_MatchItemName = m_MatchItemName ?? new Regex(MatchItemName_RegexString, RegexOptions.IgnoreCase);

        /* inter */
        /// <summary>
        /// 自检数据完整性
        /// </summary>
        [JsonIgnore]
        public bool Check => !string.IsNullOrWhiteSpace(MatchGroupName_RegexString)
                            && !string.IsNullOrWhiteSpace(MatchItemName_RegexString)
                            && DependentOnName_EscapeRegexString != null && DependentOnName_EscapeRegexString.Length > 0;

        /* ctor */
        public GroupDependentRule()
        {
        }

        /* func */
    }
}