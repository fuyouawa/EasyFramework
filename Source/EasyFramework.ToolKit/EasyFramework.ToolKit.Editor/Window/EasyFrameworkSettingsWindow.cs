using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public class EasyFrameworkSettingsWindow : OdinMenuEditorWindow
    {
        [MenuItem("Tools/EasyFramework/EasyFramework Settings")]
        public static void ShowWindow()
        {
            var isNew = HasOpenInstances<EasyFrameworkSettingsWindow>();
            var window = GetWindow<EasyFrameworkSettingsWindow>("EasyFramework Settings");
            if (!isNew)
            {
                window.CenterWindowWithSizeRadio(new Vector2(0.4f, 0.45f));
            }
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(false)
            {
                // { "Ui/UiTextPresets", UiTextPresetsSettings.Instance },
                { "Editor/Binder", BinderSettings.Instance },
                { "Editor/Builder", BuilderSettings.Instance }
            };
            return tree;
        }
    }
}
