#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace EasyGameFramework
{
    public static class EasyGUIStyles
    {
        public static GUIStyle InfoBoxCN => new GUIStyle(SirenixGUIStyles.MessageBox)
        {
            fontSize = 14,
            margin = new RectOffset(4, 4, 6, 6),
            padding = new RectOffset(0, 0, 4, 4),
            fontStyle = FontStyle.Normal
        };
    }
}

#endif