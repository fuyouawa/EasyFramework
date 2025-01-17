using Sirenix.Utilities.Editor;
using UnityEngine;

namespace EasyFramework.Inspector
{
    public static class EasyGUIStyles
    {
        private static GUIStyle _messageBox;

        public static GUIStyle MessageBox
        {
            get
            {
                if (_messageBox == null)
                {
                    _messageBox = new GUIStyle(SirenixGUIStyles.MessageBox)
                    {
                        fontSize = 13,
                        margin = new RectOffset(4, 4, 6, 6),
                        padding = new RectOffset(0, 0, 4, 4),
                        // fontStyle = FontStyle.Normal
                    };
                }
                return _messageBox;
            }
        }
    }
}
