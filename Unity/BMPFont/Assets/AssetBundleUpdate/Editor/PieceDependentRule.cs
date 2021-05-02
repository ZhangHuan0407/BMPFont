using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AssetBundleUpdate.Editor
{
    [Serializable]
    public class PieceDependentRule
    {
        /* field */
        /// <summary>
        /// 资源包名称匹配
        /// </summary>
        public string MatchName_RegexString;
        /// <summary>
        /// 允许依赖资源包转义正则
        /// </summary>
        public string[] DependentOnName_EscapeRegexString;
        /// <summary>
        /// 禁止依赖资源包转义正则
        /// </summary>
        public string[] ForbiddenDependentOnName_EscapeRegexString;

        private Regex m_MatchName;
        /// <summary>
        /// 正则表达式匹配的资源包名称
        /// </summary>
        [JsonIgnore]
        public Regex MatchName => m_MatchName = m_MatchName ?? new Regex(MatchName_RegexString, RegexOptions.IgnoreCase);

        /* inter */
        /// <summary>
        /// 自检数据完整性
        /// </summary>
        [JsonIgnore]
        public bool Check => !string.IsNullOrWhiteSpace(MatchName_RegexString)
                            && DependentOnName_EscapeRegexString != null && DependentOnName_EscapeRegexString.Length > 0
                            && ForbiddenDependentOnName_EscapeRegexString != null;

        /* ctor */
        public PieceDependentRule()
        {
        }

        /* func */
    }
}