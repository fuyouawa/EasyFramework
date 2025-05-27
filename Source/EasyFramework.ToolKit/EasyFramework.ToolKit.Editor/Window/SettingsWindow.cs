using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public class SettingsWindow : OdinMenuEditorWindow
    {
        [MenuItem("Tools/EasyFramework/Settings/ToolKit")]
        public static void ShowWindow()
        {
            var isNew = HasOpenInstances<SettingsWindow>();
            var window = GetWindow<SettingsWindow>("ToolKit");
            if (!isNew)
            {
                window.CenterWindowWithRadio(0.4f, 0.45f);
            }
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(false)
            {
                { "Editor/Binder", BinderSettings.Instance },
                { "Editor/Builder", BuilderSettings.Instance }
            };
            return tree;
        }
    }
}
