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
        public string RawFilePath { get; set; }

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

        internal void SetStringValue(string line, string directoryPath)
        {
            HaveError = true;
            if (string.IsNullOrWhiteSpace(line))
                throw new ArgumentException($"“{nameof(line)}”不能为 null 或空白。", nameof(line));
            if (string.IsNullOrWhiteSpace(directoryPath))
                throw new ArgumentException($"“{nameof(directoryPath)}”不能为 null 或空白。", nameof(directoryPath));

            Match pageIDMatch = PageIDRegex.Match(line);
            if (!pageIDMatch.Success
                || !int.TryParse(pageIDMatch.Value, out m_ID))
            {
                MessageBox.Show("Page ID Error");
                return;
            }

            Match filePathMatch = FilePathRegex.Match(line);
            if (filePathMatch.Success)
            {
                FilePath = filePathMatch.Value;
                RawFilePath = filePathMatch.Value;
            }
            else
            {
                MessageBox.Show("Page File Error");
                return;
            }
            if (!Path.IsPathRooted(FilePath))
            {
                FilePath = Path.Combine(directoryPath, FilePath);
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
            if (RawFilePath.Contains("/"))
            {
                MessageBox.Show("使用相对文件路径，而非绝对文件路径");
                return;
            }

            PageImage = new Texture2D(0, 0);
            PageImage.LoadImage(File.ReadAllBytes(FilePath));
            HaveError = false;
        }
    }
}
