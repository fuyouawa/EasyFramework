using UnityEditor;
using UnityEngine;

namespace EasyFramework.Editor
{
    public static class EasyEditorIcons
    {
        private static Texture s_expand;

        public static Texture Expand
        {
            get
            {
                if (s_expand == null)
                {
                    var t = new Texture2D(32, 32, TextureFormat.RGBA32, false);
                    t.LoadRawTextureData(RawDataManager.ExpandIcon);
                    t.Apply();
                    s_expand = t;
                }

                return s_expand;
            }
        }

        public static Texture Collapse => EditorGUIUtility.IconContent("winbtn_win_min").image;
        public static Texture AddDropdown => EditorGUIUtility.IconContent("d_Toolbar Plus More@2x").image;
        public static Texture Remove => EditorGUIUtility.IconContent("CrossIcon").image;
        public static Texture Edit => EditorGUIUtility.IconContent("d_Grid.PaintTool@2x").image;
        public static Texture Warn => EditorGUIUtility.IconContent("d_console.warnicon").image;

        public static Texture UnityPrefabIcon => EditorGUIUtility.FindTexture("Prefab Icon");
    }
}
