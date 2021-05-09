using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace BMPFont
{
    public class BMPFontChar
    {
        /* const */

        /* field */
        public static readonly Regex CharIDRegex = new Regex("(?<=id=)[0-9]+");
        private int m_ID;
        public int ID { get => m_ID; }

        public static readonly Regex PositionRegex = new Regex("(?<=x=)[0-9]+ y=[0-9]+");
        private Vector2Int m_Position;
        public Vector2Int Position { get => m_Position; }

        public static readonly Regex SizeRegex = new Regex("(?<=width=)[0-9]+ height=[0-9]+");
        private Vector2Int m_Size;
        public Vector2Int Size { get => m_Size; }

        public static readonly Regex OffsetRegex = new Regex("(?<=xoffset=)[0-9]+ yoffset=[0-9]+");
        private Vector2Int m_Offset;
        public Vector2Int Offset { get => m_Offset; }

        public static readonly Regex XAdvanceRegex = new Regex("(?<=xadvance=)[0-9]+");
        private int m_XAdvance;
        public int XAdvance { get => m_XAdvance; }

        public static readonly Regex PageIndexRegex = new Regex("(?<=page=)[0-9]+");
        private int m_PageIndex;
        public int PageIndex { get => m_PageIndex; }

        public static readonly Regex ChnlRegex = new Regex("(?<=chnl=)[0-9]");
        private bool m_Chnl;
        public bool Chnl { get => m_Chnl; }

        public static readonly Regex LetterRegex = new Regex("(?<=letter=\").+(?=\")");
        public string Letter { get; set; }

        internal Sprite Sprite;

        /// <summary>
        /// 存在错误，无法使用
        /// </summary>
        public bool HaveError { get; private set; }

        /* inter */
        public override int GetHashCode() => ID;

        /* ctor */
        public BMPFontChar()
        {
            HaveError = true;
        }

        /* func */
        internal void SetStringValue(string line)
        {
            HaveError = true;
            if (string.IsNullOrWhiteSpace(line))
                throw new ArgumentException($"“{nameof(line)}”不能为 null 或空白。", nameof(line));

            Match charIDMatch = CharIDRegex.Match(line);
            if (!charIDMatch.Success
                || !int.TryParse(charIDMatch.Value, out m_ID))
            {
                MessageBox.Show("Page ID Error");
                return;
            }

            Match positionMatch = PositionRegex.Match(line);
            if (positionMatch.Success)
            {
                string positionStr = positionMatch.Value;
                int index = positionStr.IndexOf(" y=");
                int.TryParse(positionStr.Substring(0, index), out int positionX);
                m_Position.x = positionX;
                int.TryParse(positionStr.Substring(index + " y=".Length), out int positionY);
                m_Position.y = positionY;
            }
            else
            {
                MessageBox.Show("Char Position Error");
                return;
            }

            Match sizeMatch = SizeRegex.Match(line);
            if (sizeMatch.Success)
            {
                string sizeStr = sizeMatch.Value;
                int index = sizeStr.IndexOf(" height=");
                int.TryParse(sizeStr.Substring(0, index), out int weight);
                m_Size.x = weight;
                int.TryParse(sizeStr.Substring(index + " height=".Length), out int height);
                m_Size.y = height;
            }
            else
            {
                MessageBox.Show("Char Size Error");
                return;
            }
            // 空白字符可以宽高为 0
            if (Size.x < 0 || Size.y < 0)
            {
                MessageBox.Show("Char Size Error");
                return;
            }

            Match offsetMatch = OffsetRegex.Match(line);
            if (offsetMatch.Success)
            {
                string offsetStr = offsetMatch.Value;
                int index = offsetStr.IndexOf(" yoffset=");
                int.TryParse(offsetStr.Substring(0, index), out int xOffset);
                m_Offset.x = xOffset;
                int.TryParse(offsetStr.Substring(index + " yoffset=".Length), out int yOffset);
                m_Offset.y = yOffset;
            }
            else
            {
                MessageBox.Show("Char Offset Error");
                return;
            }

            Match xAdvanceMatch = XAdvanceRegex.Match(line);
            if (!xAdvanceMatch.Success
                || !int.TryParse(xAdvanceMatch.Value, out m_XAdvance))
            {
                MessageBox.Show("Char XAdvance Error");
                return;
            }

            Match pageIndexMatch = PageIndexRegex.Match(line);
            if (!pageIndexMatch.Success
                || !int.TryParse(pageIndexMatch.Value, out m_PageIndex))
            {
                MessageBox.Show("Char PageIndex Error");
                return;
            }

            Match chnlMatch = ChnlRegex.Match(line);
            if (!chnlMatch.Success
                || !int.TryParse(chnlMatch.Value, out int chnlValue))
            {
                MessageBox.Show("Char Chnl Error");
                return;
            }
            else
                m_Chnl = chnlValue > 0;

            Match letterMatch = LetterRegex.Match(line);
            if (letterMatch.Success)
                Letter = letterMatch.Value;
            else
            {
                MessageBox.Show("Char Letter Error");
                return;
            }

            HaveError = false;
        }

        internal void LoadSprite(BMPFont bMPFont)
        {
            HaveError = true;

            if (PageIndex < 0
                || PageIndex >= bMPFont.Pages.Count)
            {
                MessageBox.Show($"Char PageIndex Error.\nPageIndex = {PageIndex}, Pages.Count = {bMPFont.Pages.Count}");
                return;
            }
            BMPFontPage bMPFontPage = bMPFont.Pages[PageIndex];
            if (bMPFontPage.HaveError)
                return;

            Sprite = Sprite.Create(bMPFontPage.PageImage, new Rect(Position, Size), new Vector2(0.5f, 0.5f));

            HaveError = false;
        }

        //public Action<Bitmap> CreateDrawCall(int vernierX, int vernierY)
        //{
        //    return (Bitmap bitmap) =>
        //    {
        //        for (int indexY = 0; indexY < Size.Y; indexY++)
        //            for (int indexX = 0; indexX < Size.X; indexX++)
        //            {
        //                Color fontColor = Colors[indexX, indexY];
        //                int bitmapIndexX = vernierX + indexX + Offset.X;
        //                int bitmapIndexY = vernierY + indexY + Offset.Y;
        //                Color backColor = bitmap.GetPixel(bitmapIndexX, bitmapIndexY);
        //                if (fontColor.A == 0)
        //                    continue;
        //                else if (backColor.A == 0)
        //                    bitmap.SetPixel(bitmapIndexX, bitmapIndexY, fontColor);
        //                else
        //                {
        //                    Color resultColor = BMPFontRenderer.ColorPlusColor(backColor, fontColor);
        //                    bitmap.SetPixel(bitmapIndexX, bitmapIndexY, resultColor);
        //                }
        //            }
        //    };
        //}
    }
}