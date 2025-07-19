using System;
using UnityEngine;

namespace EasyToolKit.Core
{
    public static class TextureUtility
    {
        public static Texture2D LoadImage(byte[] data)
        {
            var texture2D = new Texture2D(34, 34, TextureFormat.ARGB32, false, true);
            texture2D.LoadImage(data);
            return texture2D;
        }

        public static Texture2D LoadImage(string base64)
        {
            return LoadImage(Convert.FromBase64String(base64));
        }
    }
}
