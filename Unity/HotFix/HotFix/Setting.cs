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
        /// 跳过新手引导
        /// </summary>
        public bool SkipNewGuide { get; set; }
        /// <summary>
        /// 最后一次选择的位图字体路径
        /// </summary>
        public string LatestBMPFontPath { get; set; }

        /* ctor */
        private Setting() { }
        internal static IEnumerator<object> CreateInstance()
        {
            Instance = LoadFromPlayerPrefs();
            GameStart.TaskCount--;
            yield break;
        }
        internal static IEnumerator<object> StartInstance()
        {
            GameStart.TaskCount--;
            yield break;
        }

        /* func */
        public static Setting LoadFromPlayerPrefs()
        {
            Setting setting = new Setting()
            {
                Language = Application.systemLanguage.ToString(),
                SkipNewGuide = false,
                LatestBMPFontPath = "",
            };
            if (PlayerPrefs.GetString($"{nameof(Setting)}.{nameof(Language)}") is string language)
                setting.Language = language;
            if (PlayerPrefs.GetInt($"{nameof(Setting)}.{nameof(SkipNewGuide)}") > 0)
                setting.SkipNewGuide = true;
            if (PlayerPrefs.GetString($"{nameof(Setting)}.{nameof(LatestBMPFontPath)}") is string fontPath)
                setting.LatestBMPFontPath = fontPath;
            return setting;
        }
        public void SaveOutPlayerPrefs()
        {
            if (Instance == null)
                throw new NullReferenceException(nameof(Instance));

            PlayerPrefs.SetString($"{nameof(Setting)}.{nameof(Language)}", Language);
            PlayerPrefs.SetInt($"{nameof(Setting)}.{nameof(SkipNewGuide)}", SkipNewGuide ? 1 : 0);
            PlayerPrefs.SetString($"{nameof(Setting)}.{nameof(LatestBMPFontPath)}", LatestBMPFontPath);
        }
    }
}