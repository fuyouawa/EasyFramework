using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Core.Editor
{
    public static class EasyEditorIcons
    {
        private static Texture2D s_plus;
        public static Texture2D Plus
        {
            get
            {
                if (s_plus == null)
                {
                    s_plus = TextureUtility.LoadImage("iVBORw0KGgoAAAANSUhEUgAAACIAAAAiCAYAAAA6RwvCAAAA6klEQVRYCe1UQQ4CIQwUn+DZiz7Et+tH3KdgJ5FkKR2CIMmGlITsdhbaYbZMiDGejjDORyABDk5E/4klFLnKqd4y0e1pIgb+8wgDtwZFb0bFTbC7gVehESK1ex+qVY2PS/SIca5+yBXR2rkirYrAlJ4yk1FZT51rH1vrEwb/uewX4535CDMrvb833mRjZnqMCNjPHpnpebNquZki+IczR5GfEXlMZAESRX7WrC08ag2dNWJLMqZIy96/rnEiWk5XZElFClP6npLhWoQsHukRmJIu+hKsMKusIglGDI2k7INHFOmrSHY5ES3MBy+gN8uMLJH3AAAAAElFTkSuQmCC");
                }

                return s_plus;
            }
        }
        private static Texture2D s_list;
        public static Texture2D List
        {
            get
            {
                if (s_list == null)
                {
                    s_list = TextureUtility.LoadImage("iVBORw0KGgoAAAANSUhEUgAAACIAAAAiCAYAAAA6RwvCAAAAe0lEQVRYCe2UQQqAMAwErf//hQ+NBqGHJdBTmz1MQTCIZJhdOiLicji3A0QyAKJJYAQjakBn6448H23ecruf3DOPjZHBFT9D+V9soqlAKKuk1TNW0bSQWINQ1pZO6FLrjijskbkyQlmPqF8tqaJZ/bPlOyCqFSMYUQM6vyOhd+UiNy0nAAAAAElFTkSuQmCC");
                }

                return s_list;
            }
        }
        
        public static Texture Expand => EditorGUIUtility.IconContent("winbtn_win_max").image;
        public static Texture Collapse => EditorGUIUtility.IconContent("winbtn_win_min").image;
        public static Texture AddDropdown => EditorGUIUtility.IconContent("d_Toolbar Plus More@2x").image;
        public static Texture Remove => EditorGUIUtility.IconContent("CrossIcon").image;
        public static Texture Edit => EditorGUIUtility.IconContent("d_Grid.PaintTool@2x").image;
        public static Texture Warn => EditorGUIUtility.IconContent("d_console.warnicon").image;

        public static Texture UnityPrefabIcon => EditorGUIUtility.FindTexture("Prefab Icon");
    }
}
