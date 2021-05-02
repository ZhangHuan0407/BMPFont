using AssetBundleUpdate;
using HotFix.Database;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotFix
{
    [Serializable]
    public class Setting
    {
        /* field */
        public static Setting Instance { get; private set; }

        /// <summary>
        /// 当前使用的语言包
        /// </summary>
        public string Language { get; set; }
        /// <summary>
        /// 启用字幕
        /// </summary>
        public bool Subtitles { get; set; }
        /// <summary>
        /// 跳过新手引导
        /// </summary>
        public bool SkipNewGuide { get; set; }

        /* ctor */
        private Setting() { }
        internal static IEnumerator<object> CreateInstance()
        {
            Instance = LoadFromPlayerPrefs();
            yield break;
        }
        internal static IEnumerator<object> StartInstance()
        {
            Table<string, LanguageAssetBundle> languageABTable = GameSystemData.Instance[nameof(LanguageAssetBundle)] as Table<string, LanguageAssetBundle> ?? throw new NullReferenceException(nameof(languageABTable));
            LanguageAssetBundle languageAssetBundle = languageABTable[Instance.Language] ?? languageABTable["ChineseSimplified"] ?? throw new NullReferenceException(nameof(languageAssetBundle));
            AssetBundlePool.LoadAssetBundle(languageAssetBundle.InterfaceLanguageAssetBundleName, true);
            while (!AssetBundlePool.AssetBundleIsReady(languageAssetBundle.InterfaceLanguageAssetBundleName))
                yield return null;
        }

        /* func */
        public static Setting LoadFromPlayerPrefs()
        {
            string content = PlayerPrefs.GetString(nameof(Setting));
            Setting setting = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(content))
                {
                    setting = JsonConvert.DeserializeObject<Setting>(content);
                }
            }
            // 解析出现错误或者没有数据，生成默认数据
            catch (Exception e)
            {
                setting = null;
            }
            finally
            {
                setting = setting ?? new Setting()
                {
                    Language = Application.systemLanguage.ToString(),
                };
            }
            return setting;
        }
        public void SaveOutPlayerPrefs()
        {
            if (Instance == null)
                throw new NullReferenceException(nameof(Instance));

            string content = JsonConvert.SerializeObject(Instance);
            PlayerPrefs.SetString(nameof(Setting), content);
        }
    }
}