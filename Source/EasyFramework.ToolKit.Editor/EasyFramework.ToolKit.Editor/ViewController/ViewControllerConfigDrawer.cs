using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public class ViewControllerConfigDrawer : FoldoutValueDrawer<ViewControllerConfig>
    {
        private InspectorProperty _generateDirectoryProperty;
        private InspectorProperty _namespaceProperty;
        private InspectorProperty _autoScriptNameProperty;
        private InspectorProperty _scriptNameProperty;
        private InspectorProperty _baseClassProperty;
        private InspectorProperty _bindersGroupTypeProperty;
        private InspectorProperty _bindersGroupNameProperty;
        private InspectorProperty _isInitializedProperty;

        protected override void Initialize()
        {
            base.Initialize();
            _generateDirectoryProperty = Property.Children["_generateDirectory"];
            _namespaceProperty = Property.Children["_namespace"];
            _autoScriptNameProperty = Property.Children["_autoScriptName"];
            _scriptNameProperty = Property.Children["_scriptName"];
            _baseClassProperty = Property.Children["_baseClass"];
            _bindersGroupTypeProperty = Property.Children["_bindersGroupType"];
            _bindersGroupNameProperty = Property.Children["_bindersGroupName"];
            _isInitializedProperty = Property.Children["_isInitialized"];
        }

        protected override GUIContent GetLabel(GUIContent label)
        {
            return EditorHelper.TempContent("视图控制器配置");
        }

        private void EnsureInitialize()
        {
            var settings = ViewControllerSettings.Instance;
            for (int i = 0; i < Property.ValueEntry.WeakValues.Count; i++)
            {
                
            }
        }

        protected override void OnContentGUI(Rect headerRect)
        {
            EnsureInitialize();

        }

        private static Component GetTargetComponent(InspectorProperty property)
        {
            return property.Parent.ValueEntry.WeakSmartValueT<Component>();
        }
    }
}
