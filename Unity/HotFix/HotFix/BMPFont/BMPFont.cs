using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace HotFix
{
    /// <summary>
    /// 一个位图字体类型，包含位图字体的配置信息和纹理切割素材
    /// <para>读取资源后，访问 <see cref="HaveError"/> 以查看自检结果/para>
    /// </summary>
    public class BMPFont
    {
        /* const */
        public static readonly Regex CharsCountRegex = new Regex("(?<=count=)[0-9]{1,}");

        /// <summary>
        /// 加载新的位图字体事件
        /// </summary>
        public static event Action LoadedNewBMPFont_Handle;
        /// <summary>
        /// 释放位图字体事件
        /// </summary>
        public static event Action DisposeBMPFont_Handle;

        /* field */
        /// <summary>
        /// BMP 字体 Info 行具有的信息
        /// </summary>
        public BMPFontInfo Info { get; private set; }
        /// <summary>
        /// BMP 字体 Common 行具有的信息
        /// </summary>
        public BMPFontCommon Common { get; private set; }
        public List<BMPFontPage> Pages { get; }
        public Dictionary<char, BMPFontChar> Chars { get; }
        /// <summary>
        /// 最宽字符，额外空白行宽，以防止索引越界。这并不是有效的处理方法，但是最简单的处理方法。
        /// </summary>
        public int CharMaxWidth;
        /// <summary>
        /// 是否已经尝试着为此实例进行赋值操作
        /// <para>BMPFont 只允许进行一次赋值操作</para>
        /// </summary>
        public bool HaveSetValue { get; private set; }
        /// <summary>
        /// 当前实例是否存在错误
        /// </summary>
        public bool HaveError { get; private set; }

        /* inter */
        
        /* ctor */
        public BMPFont()
        {
            Info = new BMPFontInfo();
            Common = new BMPFontCommon();
            Pages = new List<BMPFontPage>();
            Chars = new Dictionary<char, BMPFontChar>();
            HaveSetValue = false;
            HaveError = false;
        }

        /* func */
        /// <summary>
        /// 从目标配置文件读取字体配置
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void LoadFontFromFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                MessageBox.Show("路径不能为空白");
                return;
            }
            else if (!File.Exists(filePath))
            {
                MessageBox.Show($"没有找到文件。\n{filePath}");
                return;
            }

            string[] lines = File.ReadAllLines(filePath);
            string directoryPath = new FileInfo(filePath).Directory.FullName;
            if (HaveSetValue)
                throw new ArgumentException("Use new instance!");
            else
                HaveSetValue = true;

            HaveError = true;
            int charCounts = 0;

            if (lines.Length < 4)
            {
                MessageBox.Show("字体配置行数过少，停止读入");
                return;
            }

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                int spaceIndex = line.IndexOf(" ");
                if (spaceIndex == -1)
                    continue;
                string label = line.Substring(0, spaceIndex);
                switch (label)
                {
                    case "char":
                        BMPFontChar oneChar = new BMPFontChar();
                        oneChar.SetStringValue(line);
                        char @char = (char)oneChar.ID;
                        if (Chars.ContainsKey(@char))
                            MessageBox.Show($"有相同的 ID 值。\nID={oneChar.ID}, Char={@char}");
                        else
                            Chars.Add(@char, oneChar);
                        break;
                    case "info":
                        Info.SetStringValue(line);
                        break;
                    case "common":
                        Common.SetStringValue(line);
                        break;
                    case "page":
                        BMPFontPage page = new BMPFontPage();
                        page.SetStringValue(line, directoryPath);
                        Pages.Add(page);
                        break;
                    case "chars":
                        Match match = CharsCountRegex.Match(line);
                        if (!match.Success
                            || !int.TryParse(match.Value, out charCounts))
                            MessageBox.Show($"这一行可能有问题。{line}");
                        break;
                    default:
                        // 意外的字段
                        break;
                }
            }

            if (charCounts != Chars.Count)
                MessageBox.Show($"字符的数量与声明的数量不一致。");
            HaveError = false;
            HaveError = Info.HaveError || Common.HaveError;
            foreach (BMPFontPage bMPFontPage in Pages)
                HaveError |= bMPFontPage.HaveError;
            foreach (BMPFontChar bMPFontChar in Chars.Values)
                HaveError |= bMPFontChar.HaveError;
            if (HaveError)
            {
                MessageBox.Show($"读取字体配置时出现错误，停止读入(155)\n\t{nameof(BMPFontCommon)} : {Common.HaveError}\n\t{nameof(BMPFontInfo)} : {Info.HaveError}");
                return;
            }

            Pages.Sort();
            foreach (BMPFontPage page in Pages)
                page.LoadImage();
            CharMaxWidth = 0;
            foreach (BMPFontChar bmpFontChar in Chars.Values)
            {
                CharMaxWidth = CharMaxWidth < bmpFontChar.Size.x ? bmpFontChar.Size.x : CharMaxWidth;
                bmpFontChar.LoadSprite(this);
            }
            HaveError = false;
            HaveError = Info.HaveError || Common.HaveError;
            foreach (BMPFontPage bMPFontPage in Pages)
                HaveError |= bMPFontPage.HaveError;
            foreach (BMPFontChar bMPFontChar in Chars.Values)
                HaveError |= bMPFontChar.HaveError;
            if (HaveError)
            {
                MessageBox.Show($"加载字体配置时出现错误，停止读入(176)\n\t{nameof(BMPFontCommon)} : {Common.HaveError}\n\t{nameof(BMPFontInfo)} : {Info.HaveError}");
                return;
            }
        }
        public void UseIt()
        {
            Debug.Log($"{nameof(BMPFont)}.{nameof(UseIt)}");
            GameSystemData.Instance.Font = this;
            LoadedNewBMPFont_Handle?.Invoke();
        }
        
        /* IDisposable */
        public void Dispose()
        {
            if (GameSystemData.Instance.Font == this)
                GameSystemData.Instance.Font = null;
            DisposeBMPFont_Handle?.Invoke();

            if (Info != null)
                Info = null;
            if (Common != null)
                Common = null;
            foreach (BMPFontPage bMPFontPage in Pages)
                bMPFontPage.PageImage = null;
            foreach (BMPFontChar bMPFontChar in Chars.Values)
                bMPFontChar.Sprite = null;
        }
    }
}
