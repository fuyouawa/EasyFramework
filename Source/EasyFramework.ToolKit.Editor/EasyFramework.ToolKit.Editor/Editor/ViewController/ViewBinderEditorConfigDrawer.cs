using System;
using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

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
        private ViewBinderSettings _settings;

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

            _settings = ViewBinderSettings.Instance;
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
            _propertyOfBindAccess.Draw(EditorHelper.TempContent("绑定权限"));
            _propertyOfBindName.Draw(EditorHelper.TempContent("绑定名称"));
            
            _propertyOfAutoBindName.Draw(EditorHelper.TempContent("自动绑定名称", "绑定名称与游戏对象名称相同"));
            _propertyOfProcessBindName.Draw(EditorHelper.TempContent("处理绑定命名"));
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("实际变量名称", ((IViewBinder)comp).GetBindName());
            EditorGUI.EndDisabledGroup();
            
            EasyEditorGUI.Title("注释设置");
            _propertyOfUseDocumentComment.Draw(EditorHelper.TempContent("使用文档注释"));
            _propertyOfAutoAddParaToComment.Draw(EditorHelper.TempContent("自动添加注释段落"));
            _propertyOfComment.Draw(EditorHelper.TempContent("注释"));

            if (GUILayout.Button("恢复默认值"))
            {
                UseDefault(Property);
            }
            
            _propertyOfBindComponentType.ValueEntry.OnValueChanged += i =>
            {
                if (_propertyOfBindComponentType.ValueEntry.WeakSmartValue is Type type)
                {
                    var specialType = ViewBinderEditorUtility.GetDefaultSpecialType(
                        ViewBinderUtility.GetSpecficableBindTypes(type));
                    _propertyOfSpecificBindType.ValueEntry.WeakSmartValue = specialType;
                }
                else
                {
                    _propertyOfSpecificBindType.ValueEntry.WeakSmartValue = null;
                }
            };

        }

        private void EnsureInitialize(InspectorProperty property)
        {
            var comp = GetTargetComponent(property);
            var cfg = property.WeakSmartValue<ViewBinderEditorConfig>();
            cfg.SetTargetComponent(comp);

            if (!cfg.IsInitialized)
            {
                UseDefault(property);
                cfg.IsInitialized = true;

                ValueChanged(comp);
            }
        }

        private void UseDefault(InspectorProperty property)
        {
            var comp = GetTargetComponent(property);
            var cfg = property.WeakSmartValue<ViewBinderEditorConfig>();

            cfg.BindName = comp.gameObject.name;
            var bindableComps = ((IViewBinder)comp).GetBindableComponentTypes();
            cfg.BindComponentType = bindableComps[0];

            cfg.SpecificBindType = ViewBinderEditorUtility.GetDefaultSpecialType(
                    ViewBinderUtility.GetSpecficableBindTypes(cfg.BindComponentType));

            cfg.BindGameObject = _settings.Default.BindGameObject;
            cfg.BindAccess = _settings.Default.BindAccess;
            cfg.AutoBindName = _settings.Default.AutoBindName;
            cfg.ProcessBindName = _settings.Default.ProcessBindName;
            cfg.UseDocumentComment = _settings.Default.UseDocumentComment;
            cfg.AutoAddParaToComment = _settings.Default.AutoAddParaToComment;
            cfg.Comment = _settings.Default.Comment;
        }

        private void ValueChanged(Object target)
        {
            EditorUtility.SetDirty(target);
        }

        private static Component GetTargetComponent(InspectorProperty property)
        {
            return property.Parent.Parent.WeakSmartValue<Component>();
        }
    }
}
