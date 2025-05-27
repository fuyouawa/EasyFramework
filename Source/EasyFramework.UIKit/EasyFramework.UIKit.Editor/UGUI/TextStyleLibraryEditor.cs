using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace EasyFramework.UIKit.Editor
{
    [CustomEditor(typeof(TextStyleLibrary))]
    public class TextStyleLibraryEditor : OdinEditor
    {
        private InspectorProperty _stylesProperty;
        private InspectorProperty _defaultStyleNameProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            _stylesProperty = Tree.RootProperty.Children["_styles"];
            _defaultStyleNameProperty = Tree.RootProperty.Children["_defaultStyleName"];
        }

        protected override void DrawTree()
        {
            Tree.BeginDraw(true);

            _stylesProperty.DrawEx("样式列表");
            _defaultStyleNameProperty.DrawEx("默认样式");

            Tree.EndDraw();
        }
    }
}
