using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.UIKit.Editor
{
    public class SettingsWindow : OdinMenuEditorWindow
    {
        [MenuItem("Tools/EasyFramework/Settings/UIKit")]
        public static SettingsWindow ShowWindow()
        {
            var isNew = HasOpenInstances<SettingsWindow>();
            var window = GetWindow<SettingsWindow>("UIKit");
            if (!isNew)
            {
                window.CenterWindowWithRadio(0.4f, 0.45f);
            }
            return window;
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(false)
            {
                { "TextStyleLibrary", TextStyleLibrary.Instance },
            };
            return tree;
        }
    }
}
