// using Sirenix.Utilities.Editor;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal.VersionControl;

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
        public static readonly Color BorderColor = EditorGUIUtility.isProSkin ? new Color(0.11f * 1.0f, 0.11f * 1.0f, 0.11f * 1.0f, 0.8f) : new Color(0.38f, 0.38f, 0.38f, 0.6f);
        public static readonly Color ListItemDragBgColor = EditorGUIUtility.isProSkin ? new Color(0.1f, 0.1f, 0.1f, 1f) : new Color(0.338f, 0.338f, 0.338f, 1.000f);
        
        public static readonly Color ListItemColorEven = EditorGUIUtility.isProSkin ? new Color(0.235f, 0.235f, 0.235f, 1f) : new Color(0.838f, 0.838f, 0.838f, 1.000f);
        public static readonly Color ListItemColorOdd = EditorGUIUtility.isProSkin ? new Color(0.216f, 0.216f, 0.216f, 1f) : new Color(0.801f, 0.801f, 0.801f, 1.000f);
        
        public static readonly Color ListItemColorHoverEven = EditorGUIUtility.isProSkin ? new Color(0.279f * 0.8f, 0.279f * 0.8f, 0.279f * 0.8f, 1f) : new Color(0.890f, 0.890f, 0.890f, 1.000f);
        public static readonly Color ListItemColorHoverOdd = EditorGUIUtility.isProSkin ? new Color(0.309f * 0.8f, 0.309f * 0.8f, 0.309f * 0.8f, 1f) : new Color(0.904f, 0.904f, 0.904f, 1.000f);

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
        
        private static GUIStyle s_iconButton;
        public static GUIStyle IconButton
        {
            get
            {
                if (s_iconButton == null)
                {
                    s_iconButton = new GUIStyle(GUIStyle.none) { padding = new RectOffset(1, 1, 1, 1), };
                }
                return s_iconButton;
            }
        }

        private static GUIStyle s_toolbarButton;
        
        public static GUIStyle ToolbarButton
        {
            get
            {
                if (s_toolbarButton == null)
                {
                    //toolbarButton = new GUIStyle("OL Title TextRight") { stretchHeight = true, stretchWidth = false, fixedHeight = 0f, alignment = TextAnchor.MiddleCenter, font = EditorStyles.toolbarButton.font, fontSize = EditorStyles.toolbarButton.fontSize, fontStyle = EditorStyles.toolbarButton.fontStyle, overflow = new RectOffset(1, 0, 0, 0), };
                    s_toolbarButton = new GUIStyle(EditorStyles.toolbarButton)
                    {
                        fixedHeight = 0,
                        alignment = TextAnchor.MiddleCenter,
                        stretchHeight = true,
                        stretchWidth = false,
                    };
                }
                return s_toolbarButton;
            }
        }
        
        private static GUIStyle s_toolbarButtonSelected;
        public static GUIStyle ToolbarButtonSelected
        {
            get
            {
                if (s_toolbarButtonSelected == null)
                {
                    s_toolbarButtonSelected = new GUIStyle(ToolbarButton)
                    {
                        normal = new GUIStyle(ToolbarButton).onNormal
                    };
                }

                return s_toolbarButtonSelected;
            }
        }

        private static GUIStyle s_toolbarBackground;

        public static GUIStyle ToolbarBackground
        {
            get
            {
                if (s_toolbarBackground == null)
                {
                    //toolbarBackground = new GUIStyle("OL title") { fixedHeight = 0, fixedWidth = 0, stretchHeight = true, stretchWidth = true, padding = new RectOffset(0, 0, 0, 0), margin = new RectOffset(0, 0, 0, 0), overflow = new RectOffset(0, 0, 0, 0), };
                    s_toolbarBackground = new GUIStyle(EditorStyles.toolbar)
                    {
                        padding = new RectOffset(0, 1, 0, 0),
                        stretchHeight = true,
                        stretchWidth = true,
                        fixedHeight = 0,
                    };
                }
                return s_toolbarBackground;
            }
        }

        private static GUIStyle s_listItem;
        public static GUIStyle ListItem
        {
            get
            {
                if (s_listItem == null)
                {
                    s_listItem = new GUIStyle(None) { padding = new RectOffset(0, 0, 3, 3) };
                }

                return s_listItem;
            }
        }
    }
}
