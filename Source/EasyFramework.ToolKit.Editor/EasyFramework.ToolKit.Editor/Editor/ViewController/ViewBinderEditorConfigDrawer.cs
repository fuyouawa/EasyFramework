using System.Collections.Generic;
using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public class ViewBinderEditorConfigDrawer : FoldoutValueDrawer<ViewBinderEditorConfig>
    {
        private InspectorProperty _propertyOfBindGameObject;
        private InspectorProperty _propertyOfBindComponentType;
        private InspectorProperty _propertyOfSpecificBindType;
        private InspectorProperty _propertyOfBindAccess;
        private InspectorProperty _propertyOfAutoBindName;
        private InspectorProperty _propertyOfProcessBindName;
        private InspectorProperty _propertyOfBindName;
        private InspectorProperty _propertyOfUseDocumentComment;
        private InspectorProperty _propertyOfAutoAddParaToComment;
        private InspectorProperty _propertyOfComment;

        protected override void Initialize()
        {
            base.Initialize();
            _propertyOfBindGameObject = Property.Children[nameof(ViewBinderEditorConfig.BindGameObject)];
            _propertyOfBindComponentType = Property.Children[nameof(ViewBinderEditorConfig.BindComponentType)];
            _propertyOfSpecificBindType = Property.Children[nameof(ViewBinderEditorConfig.SpecificBindType)];
            _propertyOfBindAccess = Property.Children[nameof(ViewBinderEditorConfig.BindAccess)];
            _propertyOfAutoBindName = Property.Children[nameof(ViewBinderEditorConfig.AutoBindName)];
            _propertyOfProcessBindName = Property.Children[nameof(ViewBinderEditorConfig.ProcessBindName)];
            _propertyOfBindName = Property.Children[nameof(ViewBinderEditorConfig.BindName)];
            _propertyOfUseDocumentComment = Property.Children[nameof(ViewBinderEditorConfig.UseDocumentComment)];
            _propertyOfAutoAddParaToComment = Property.Children[nameof(ViewBinderEditorConfig.AutoAddParaToComment)];
            _propertyOfComment = Property.Children[nameof(ViewBinderEditorConfig.Comment)];
        }

        protected override GUIContent GetLabel(GUIContent label)
        {
            return EditorHelper.TempContent("绑定配置");
        }

        protected override void OnContentGUI(Rect headerRect)
        {
            foreach (var component in Property.Components)
            {
                EnsureInitialize(component.Property);
            }

            var val = ValueEntry.SmartValue;
            var comp = GetTargetComponent(Property);
            
            _propertyOfBindGameObject.Draw(EditorHelper.TempContent("绑定游戏对象"));
            _propertyOfBindComponentType.Draw(EditorHelper.TempContent("绑定组件"));
            _propertyOfSpecificBindType.Draw(EditorHelper.TempContent("指定要绑定的类型"));
            _propertyOfBindAccess.Draw(EditorHelper.TempContent("绑定访问权限"));
            _propertyOfAutoBindName.Draw(EditorHelper.TempContent("自动绑定名称", "绑定名称与游戏对象名称相同"));
            _propertyOfBindName.Draw(EditorHelper.TempContent("绑定名称"));

            _propertyOfProcessBindName.Draw(EditorHelper.TempContent("处理绑定命名"));

            if (val.ProcessBindName)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField("实际变量名称", ViewBinderHelper.GetBindName((IViewBinder)comp));
                EditorGUI.EndDisabledGroup();
            }
            
            EasyEditorGUI.Title("注释设置");
            _propertyOfUseDocumentComment.Draw(EditorHelper.TempContent("使用文档注释"));
            _propertyOfAutoAddParaToComment.Draw(EditorHelper.TempContent("自动添加注释段落"));
            _propertyOfComment.Draw(EditorHelper.TempContent("注释"));
        }

        private void EnsureInitialize(InspectorProperty property)
        {
            var cfg = property.WeakSmartValue<ViewBinderEditorConfig>();
            cfg.SetTargetComponent(GetTargetComponent(property));

            if (!cfg.IsInitialized)
            {
                UseDefault(property);
            }
        }

        private void UseDefault(InspectorProperty property)
        {
        }

        private void ValueChanged(Object target)
        {
            EditorUtility.SetDirty(target);
        }

        private static Component GetTargetComponent(InspectorProperty property)
        {
            if (property.Parent.ValueEntry.WeakSmartValue is OtherViewBinderConfig config)
            {
                return config.Target?.transform;
            }

            return property.Parent.Parent.WeakSmartValue<Component>();
        }
    }
}
