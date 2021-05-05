using System;
using System.Collections.Generic;
using UnityEngine;

namespace HotFix.EncoderExtend
{
    public static class Vector4Extend
    {
        /* func */
        public static bool TryParse(string str, out Vector4 vector4)
        {
            vector4 = default;
            if (string.IsNullOrEmpty(str))
                return false;

            string[] clips = str.Split(',');
            if (clips.Length == 4
                && int.TryParse(clips[0], out int x)
                && int.TryParse(clips[1], out int y)
                && int.TryParse(clips[1], out int z)
                && int.TryParse(clips[1], out int w))
            {
                vector4 = new Vector4(x, y, z, w);
                return true;
            }
            else
                return false;
        }
    }
}