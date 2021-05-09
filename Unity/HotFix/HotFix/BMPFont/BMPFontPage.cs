using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace HotFix
{
    public class BMPFontPage
    {
        /* const */

        /* field */
        public static readonly Regex PageIDRegex = new Regex("(?<=id=)[0-9]+");
        private int m_ID;
        public int ID { get => m_ID; }

        public static readonly Regex FilePathRegex = new Regex("(?<=file=\")\\w+\\.png(?=\")");
        public string FilePath { get; set; }

        /// <summary>
        /// 存在错误，无法使用
        /// </summary>
        public bool HaveError { get; private set; }
        internal Texture2D PageImage;

        /* inter */

        /* ctor */
        public BMPFontPage()
        {
            HaveError = true;
        }

        /* func */
        public override int GetHashCode() => ID;

        internal void SetStringValue(string line)
        {
            HaveError = true;
            if (string.IsNullOrWhiteSpace(line))
                throw new ArgumentException($"“{nameof(line)}”不能为 null 或空白。", nameof(line));

            Match pageIDMatch = PageIDRegex.Match(line);
            if (!pageIDMatch.Success
                || !int.TryParse(pageIDMatch.Value, out m_ID))
            {
                MessageBox.Show("Page ID Error");
                return;
            }

            Match filePathMatch = FilePathRegex.Match(line);
            if (filePathMatch.Success)
                FilePath = filePathMatch.Value;
            else
            {
                MessageBox.Show("Page File Error");
                return;
            }

            HaveError = false;
        }

        /* IComparable */
        public int CompareTo(BMPFontPage other)
        {
            if (other is null)
                return 1;
            else
                return other.ID.CompareTo(ID);
        }

        internal void LoadImage()
        {
            HaveError = true;
            if (!File.Exists(FilePath))
            {
                MessageBox.Show($"没有找到图片文件。\n{FilePath}");
                return;
            }
            if (FilePath.Contains("/"))
            {
                MessageBox.Show("使用相对文件路径，而非绝对文件路径");
                return;
            }

            PageImage = new Texture2D(0, 0);
            PageImage.LoadImage(File.ReadAllBytes(FilePath));
            Debug.Log($"load image, size : {PageImage.width}, {PageImage.height}");
            HaveError = false;
        }
    }
}
