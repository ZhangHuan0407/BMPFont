using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AssetBundleUpdate.Editor
{
    /// <summary>
    /// 依赖规则配置，描述资源包交叉依赖是否合法
    /// </summary>
    [Serializable]
    public class DependentRuleConfig
    {
        /* const */
        /// <summary>
        /// 依赖规则配置默认位置
        /// </summary>
        public const string DefaultDependentRuleConfigAssetPath = "Assets/Editor Default Resources/AssetBundleConfig/DependentRuleConfig.json";

        /* field */
        /// <summary>
        /// 单项依赖规则集合
        /// </summary>
        public HashSet<PieceDependentRule> PieceDependentRules { get; set; }
        /// <summary>
        /// 组依赖规则集合
        /// </summary>
        public HashSet<GroupDependentRule> GroupDependentRules { get; set; }

        /* ctor */

        /* inter */
        /// <summary>
        /// 自检数据完整性
        /// </summary>
        [JsonIgnore]
        public bool Check
        {
            get
            {
                if (PieceDependentRules is null
                    || GroupDependentRules is null || GroupDependentRules.Count < 1)
                    return false;
                foreach (PieceDependentRule pieceDependentRule in PieceDependentRules)
                    if (!pieceDependentRule.Check)
                        return false;
                foreach (GroupDependentRule groupDependentRule in GroupDependentRules)
                    if (!groupDependentRule.Check)
                        return false;
                return true;
            }
        }

        /* func */
        /// <summary>
        /// 读取默认位置的依赖规则配置
        /// </summary>
        /// <returns>依赖规则配置实例</returns>
        public DependentRuleConfig GetDefaultDependentRuleConfig()
        {
            if (TryGetDependentRuleConfig(DefaultDependentRuleConfigAssetPath, out DependentRuleConfig config))
                return config;
            else
                throw new FileNotFoundException($"Not found file at path : {DefaultDependentRuleConfigAssetPath}");
        }
        /// <summary>
        /// 尝试读取目标文件，反序列化为依赖规则配置实例
        /// </summary>
        /// <param name="filePath">资源文件路径</param>
        /// <param name="config">读取获得的依赖规则配置实例</param>
        /// <returns>是否找到文件并反序列化成功</returns>
        public bool TryGetDependentRuleConfig(string filePath, out DependentRuleConfig config)
        {
            config = null;
            if (string.IsNullOrWhiteSpace(filePath))
                return false;
            else if (!File.Exists(filePath))
                return false;

            try
            {
                string content = File.ReadAllText(filePath);
                config = JsonConvert.DeserializeObject<DependentRuleConfig>(content);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                config = null;
                return false;
            }
            return true;
        }
    }
}