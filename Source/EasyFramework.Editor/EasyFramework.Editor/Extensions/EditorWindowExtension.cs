using UnityEditor;
using UnityEngine;

namespace EasyFramework.Editor
{
    public static class EditorWindowExtension
    {
        public static void CenterWindowWithSizeRadio(this EditorWindow window, Vector2 windowSizeRadio)
        {
            var size = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            size.x *= windowSizeRadio.x;
            size.y *= windowSizeRadio.y;
            window.CenterWindow(size);
        }

        public static void CenterWindow(this EditorWindow window, Vector2 windowSize)
        {
            // var screenResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            //
            // var windowPosition = new Vector2(
            //     (screenResolution.x - windowSize.x) / 2,
            //     (screenResolution.y - windowSize.y) / 2
            // );

            window.position.AlignCenter(windowSize.x, windowSize.y);
        }
    }
}
