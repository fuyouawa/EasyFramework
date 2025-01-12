using JetBrains.Annotations;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace EasyGameFramework.Editor
{
    public class EasyGameFrameworkSettingsWindow : OdinMenuEditorWindow
    {
        [MenuItem("Tools/EasyGameFramework/EasyGameFrameworkSettings Window")]
        [UsedImplicitly]
        public static void ShowWindow()
        {
            GetWindow<EasyGameFrameworkSettingsWindow>("EasyGameFrameworkSettings Window");
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(false)
            {
                { "EasyControl", EasyGameFrameworkSettings.Instance.EasyControlSettings }
            };
            return tree;
        }
    }
}
