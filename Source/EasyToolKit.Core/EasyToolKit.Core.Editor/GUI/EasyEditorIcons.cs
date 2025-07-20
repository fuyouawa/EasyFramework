using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Core.Editor
{
    public static class EasyEditorIcons
    {
        private static EditorIcon s_plus;
        public static EditorIcon Plus
        {
            get
            {
                if (s_plus == null)
                {
                    s_plus = new EditorIcon("iVBORw0KGgoAAAANSUhEUgAAACIAAAAiCAYAAAA6RwvCAAAA6klEQVRYCe1UQQ4CIQwUn+DZiz7Et+tH3KdgJ5FkKR2CIMmGlITsdhbaYbZMiDGejjDORyABDk5E/4klFLnKqd4y0e1pIgb+8wgDtwZFb0bFTbC7gVehESK1ex+qVY2PS/SIca5+yBXR2rkirYrAlJ4yk1FZT51rH1vrEwb/uewX4535CDMrvb833mRjZnqMCNjPHpnpebNquZki+IczR5GfEXlMZAESRX7WrC08ag2dNWJLMqZIy96/rnEiWk5XZElFClP6npLhWoQsHukRmJIu+hKsMKusIglGDI2k7INHFOmrSHY5ES3MBy+gN8uMLJH3AAAAAElFTkSuQmCC");
                }

                return s_plus;
            }
        }

        private static EditorIcon s_list;
        public static EditorIcon List
        {
            get
            {
                if (s_list == null)
                {
                    s_list = new EditorIcon("iVBORw0KGgoAAAANSUhEUgAAACIAAAAiCAYAAAA6RwvCAAAAe0lEQVRYCe2UQQqAMAwErf//hQ+NBqGHJdBTmz1MQTCIZJhdOiLicji3A0QyAKJJYAQjakBn6448H23ecruf3DOPjZHBFT9D+V9soqlAKKuk1TNW0bSQWINQ1pZO6FLrjijskbkyQlmPqF8tqaJZ/bPlOyCqFSMYUQM6vyOhd+UiNy0nAAAAAElFTkSuQmCC");
                }

                return s_list;
            }
        }

        private static EditorIcon s_x;
        public static EditorIcon X
        {
            get
            {
                if (s_x == null)
                {
                    s_x = new EditorIcon("iVBORw0KGgoAAAANSUhEUgAAACIAAAAiCAYAAAA6RwvCAAABTklEQVRYCe2WUQ7CIAyGnaeaR9CbeY7dwee96a1mf0IT0rVd2RaDCU0I0vKXj7I5hmVZLi3YtQUIMHQQeRK9InsqMpBooobXa5QJnDHmQgMtcviG19dpA8VmaqXdaeBpEMOc0pADuUydGchCJNDMg5EQrHdhPJCJMxi9BmNBcIon/VDXVJ15Mou9voTZguA86prp3IynCA/b24iV7kcevEqn8ftG/o8ay7tXKSkW3SXv1uvL6q3WWzkUsDNgXAisGQHBnCMwmxA1IHthQhC1ILUwYQiANPPRiz4jtdWgTSYLVyUKgoR7LQQTATkCwfCbMFsgZ0CEYP7iL5534vVlyaPVU09BddLK8DdzDdBuZ1ydshJyM1ZlZhKbtzSZRI41GA+C9RLGhcAJsNDrAcPHNEIUbJgLg9asBMVSvjRBvaj82NnMt6aDyJPvFZEV+QLtPDH5milIBAAAAABJRU5ErkJggg==");
                }

                return s_x;
            }
        }
        
        // public static Texture Expand => EditorGUIUtility.IconContent("winbtn_win_max").image;
        // public static Texture Collapse => EditorGUIUtility.IconContent("winbtn_win_min").image;
        // public static Texture AddDropdown => EditorGUIUtility.IconContent("d_Toolbar Plus More@2x").image;
        // public static Texture Remove => EditorGUIUtility.IconContent("CrossIcon").image;
        // public static Texture Edit => EditorGUIUtility.IconContent("d_Grid.PaintTool@2x").image;
        // public static Texture Warn => EditorGUIUtility.IconContent("d_console.warnicon").image;
        //
        // public static Texture UnityPrefabIcon => EditorGUIUtility.FindTexture("Prefab Icon");
    }
}
