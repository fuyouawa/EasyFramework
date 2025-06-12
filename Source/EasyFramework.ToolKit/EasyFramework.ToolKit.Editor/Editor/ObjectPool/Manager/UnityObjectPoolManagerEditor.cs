using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace EasyFramework.ToolKit.Editor
{
    [CustomEditor(typeof(UnityObjectPoolManager))]
    public class UnityObjectPoolManagerEditor : OdinEditor
    {
        private InspectorProperty _settingsProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            _settingsProperty = Tree.RootProperty.Children["_settings"];
        }

        protected override void DrawTree()
        {
            Tree.BeginDraw(true);

            EasyEditorGUI.Title("设置");
            _settingsProperty.Draw();

            Tree.EndDraw();
        }
    }
}
