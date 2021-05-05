using System;
using System.Collections.Generic;
using UnityEngine;

namespace HotFix.EncoderExtend
{
    public static class Vector2IntExtend
    {
        /* func */
        public static bool TryParse(string str, out Vector2Int vector2Int)
        {
            vector2Int = default;
            if (string.IsNullOrEmpty(str))
                return false;

            string[] clips = str.Split(',');
            if (clips.Length == 2
                && int.TryParse(clips[0], out int x)
                && int.TryParse(clips[1], out int y))
            {
                vector2Int = new Vector2Int(x, y);
                return true;
            }
            else
                return false;
        }
    }
}