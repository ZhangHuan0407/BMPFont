using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace HotFix
{
    public class BMPFontCommon
    {
        /* const */

        /* field */
        public static readonly Regex LineHelghtRegex = new Regex("(?<=lineHeight=)[0-9]+");
        private int m_LineHelght;
        public int LineHelght { get => m_LineHelght; }

        public static readonly Regex BaseRegex = new Regex("(?<=base=)[0-9]+");
        private int m_Base;
        public int Base { get => m_Base; }

        public static readonly Regex ScaleRegex = new Regex("(?<=scaleW=)[0-9]+ scaleH=[0-9]+");
        private Vector2Int m_Scale;
        public Vector2Int Scale { get => m_Scale; }

        //...

        /// <summary>
        /// 存在错误，无法使用
        /// </summary>
        public bool HaveError { get; private set; }

        /* inter */

        /* ctor */
        public BMPFontCommon()
        {
            HaveError = true;
        }

        internal void SetStringValue(string line)
        {
            HaveError = true;
            if (string.IsNullOrWhiteSpace(line))
                throw new ArgumentException($"“{nameof(line)}”不能为 null 或空白。", nameof(line));

            Match lineHeightMatch = LineHelghtRegex.Match(line);
            if (!lineHeightMatch.Success
                || !int.TryParse(lineHeightMatch.Value, out m_LineHelght))
            {
                MessageBox.Show("Common LineHeight Error");
                return;
            }

            Match baseMatch = BaseRegex.Match(line);
            if (!baseMatch.Success
                || !int.TryParse(baseMatch.Value, out m_Base))
            {
                MessageBox.Show("Common Base Error");
                return;
            }

            Match scaleMatch = ScaleRegex.Match(line);
            if (scaleMatch.Success)
            {
                string scaleStr = scaleMatch.Value;
                int index = scaleStr.IndexOf(" scaleH=");
                int.TryParse(scaleStr.Substring(0, index), out int scaleW);
                m_Scale.x = scaleW;
                int.TryParse(scaleStr.Substring(index + " scaleH=".Length), out int scaleH);
                m_Scale.y = scaleH;
            }
            else
            {
                MessageBox.Show("Common Scale Error");
                return;
            }

            HaveError = false;
        }

        /* func */

    }
}