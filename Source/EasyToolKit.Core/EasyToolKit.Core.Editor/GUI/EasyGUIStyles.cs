// using Sirenix.Utilities.Editor;
using UnityEngine;

namespace EasyToolKit.Core.Editor
{
    public static class EasyGUIStyles
    {
        // private static GUIStyle s_messageBox;
        //
        // public static GUIStyle MessageBox
        // {
        //     get
        //     {
        //         if (s_messageBox == null)
        //         {
        //             s_messageBox = new GUIStyle(SirenixGUIStyles.MessageBox)
        //             {
        //                 fontSize = 13,
        //                 margin = new RectOffset(4, 4, 6, 6),
        //                 padding = new RectOffset(0, 0, 4, 4),
        //                 // fontStyle = FontStyle.Normal
        //             };
        //         }
        //         return s_messageBox;
        //     }
        // }
        private static GUIStyle s_none;

        public static GUIStyle None
        {
            get
            {
                if (s_none == null)
                {
                    s_none = new GUIStyle() { margin = new RectOffset(0, 0, 0, 0), padding = new RectOffset(0, 0, 0, 0), border = new RectOffset(0, 0, 0, 0) };
                }
                return s_none;
            }
        }
    }
}
