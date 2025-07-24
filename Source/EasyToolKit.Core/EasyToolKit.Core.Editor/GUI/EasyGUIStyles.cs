using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Core.Editor
{
    public static class EasyGUIStyles
    {
        public static readonly Color HeaderBoxBackgroundColor =
            EditorGUIUtility.isProSkin ? new Color(1, 1, 1, 0.06f) : new Color(1, 1, 1, 0.26f);

        public static readonly Color BorderColor = EditorGUIUtility.isProSkin
            ? new Color(0.11f * 1.0f, 0.11f * 1.0f, 0.11f * 1.0f, 0.8f)
            : new Color(0.38f, 0.38f, 0.38f, 0.6f);

        public static readonly Color ListItemDragBgColor = EditorGUIUtility.isProSkin
            ? new Color(0.1f, 0.1f, 0.1f, 1f)
            : new Color(0.338f, 0.338f, 0.338f, 1.000f);

        public static readonly Color ListItemColorEven = EditorGUIUtility.isProSkin
            ? new Color(0.235f, 0.235f, 0.235f, 1f)
            : new Color(0.838f, 0.838f, 0.838f, 1.000f);

        public static readonly Color ListItemColorOdd = EditorGUIUtility.isProSkin
            ? new Color(0.216f, 0.216f, 0.216f, 1f)
            : new Color(0.801f, 0.801f, 0.801f, 1.000f);

        public static readonly Color ListItemColorHoverEven = EditorGUIUtility.isProSkin
            ? new Color(0.279f * 0.8f, 0.279f * 0.8f, 0.279f * 0.8f, 1f)
            : new Color(0.890f, 0.890f, 0.890f, 1.000f);

        public static readonly Color ListItemColorHoverOdd = EditorGUIUtility.isProSkin
            ? new Color(0.309f * 0.8f, 0.309f * 0.8f, 0.309f * 0.8f, 1f)
            : new Color(0.904f, 0.904f, 0.904f, 1.000f);

        public static readonly Color InactiveColor = EditorGUIUtility.isProSkin
            ? new Color(0.40f, 0.40f, 0.40f, 1)
            : new Color(0.72f, 0.72f, 0.72f, 1);

        public static readonly Color ActiveColor = EditorGUIUtility.isProSkin
            ? new Color(0.55f, 0.55f, 0.55f, 1)
            : new Color(0.40f, 0.40f, 0.40f, 1);

        public static readonly Color HighlightedColor = EditorGUIUtility.isProSkin
            ? new Color(0.90f, 0.90f, 0.90f, 1)
            : new Color(0.20f, 0.20f, 0.20f, 1);

        public static readonly Color HighlightedTextColor =
            EditorGUIUtility.isProSkin ? new Color(1, 1, 1, 1) : new Color(0, 0, 0, 1);

        private static GUIStyle s_none;

        public static GUIStyle None
        {
            get
            {
                if (s_none == null)
                {
                    s_none = new GUIStyle()
                    {
                        margin = new RectOffset(0, 0, 0, 0), padding = new RectOffset(0, 0, 0, 0),
                        border = new RectOffset(0, 0, 0, 0)
                    };
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

        private static GUIStyle s_boxContainer;

        /// <summary>
        /// Box container style.
        /// </summary>
        public static GUIStyle BoxContainer
        {
            get
            {
                if (s_boxContainer == null)
                {
                    s_boxContainer = new GUIStyle(EditorStyles.helpBox) { margin = new RectOffset(0, 0, 0, 2) };
                }

                return s_boxContainer;
            }
        }

        private static GUIStyle s_boxHeaderStyle;

        /// <summary>
        /// Box header style.
        /// </summary>
        public static GUIStyle BoxHeaderStyle
        {
            get
            {
                if (s_boxHeaderStyle == null)
                {
                    s_boxHeaderStyle = new GUIStyle(None) { margin = new RectOffset(0, 0, 0, 2) };
                }

                return s_boxHeaderStyle;
            }
        }

        private static GUIStyle s_propertyPadding;

        /// <summary>
        /// Property padding.
        /// </summary>
        public static GUIStyle PropertyPadding
        {
            get
            {
                if (s_propertyPadding == null)
                {
                    s_propertyPadding = new GUIStyle(GUIStyle.none)
                        { padding = new RectOffset(0, 0, 0, 3), margin = new RectOffset(0, 0, 0, 0) };
                }

                return s_propertyPadding;
            }
        }

        private static GUIStyle s_foldout;

        public static GUIStyle Foldout
        {
            get
            {
                if (s_foldout == null)
                {
                    s_foldout = new GUIStyle(EditorStyles.foldout)
                    {
                        fixedWidth = 0,
                        fixedHeight = 16,
                        stretchHeight = false,
                        stretchWidth = true,
                    };
                }

                return s_foldout;
            }
        }
        
        private static GUIStyle s_whiteBoxStyle;
        public static GUIStyle WhiteBoxStyle
        {
            get
            {
                if (s_whiteBoxStyle == null)
                {
                    s_whiteBoxStyle = new GUIStyle();
                    s_whiteBoxStyle.normal.background = Texture2D.whiteTexture;
                }
                return s_whiteBoxStyle;
            }
        }
    }
}
