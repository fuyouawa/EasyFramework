using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyFramework.Editor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EasyFramework.Editor.Drawer
{
    public class ViewModelInfoDrawer : OdinValueDrawer<ViewModelInfo>
    {
        private Component _component;
        private bool _isBuild;
        private ViewModelEditorInfo _editorInfo;
        private ViewModelInfo _info;

        private FoldoutGroupConfig _foldoutGroupConfig;
        private readonly ViewModelBuilder _builder = new ViewModelBuilder();

        private Type _classType;
        private ViewModelSettings _settings;

        protected override void Initialize()
        {
            base.Initialize();

            _component = Property.Parent.WeakSmartValue<Component>();
            _isBuild = _component as ViewModel == null;

            _info = ValueEntry.SmartValue;
            _editorInfo = _info.EditorData.Get<ViewModelEditorInfo>() ?? new ViewModelEditorInfo();
            if (_editorInfo == null)
            {
                _editorInfo = new ViewModelEditorInfo();
                _info.EditorData.Set(_editorInfo);
            }

            _foldoutGroupConfig = new FoldoutGroupConfig(
                UniqueDrawerKey.Create(Property, this),
                new GUIContent("视图模型"), true, OnContentGUI);

            _classType = _editorInfo.GetClassType();

            _settings = ViewModelSettings.Instance;
            if (!_editorInfo.IsInitialized)
            {
                _editorInfo.ClassName = _component.gameObject.name;
                _editorInfo.GenerateDirectory = _settings.DefaultGenerateDirectory;
                _editorInfo.Namespace = _settings.DefaultNamespace;
                _editorInfo.BaseClass = typeof(MonoBehaviour);

                _editorInfo.IsInitialized = true;
                ValueChanged();
            }
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            _foldoutGroupConfig.Expand = Property.State.Expanded;
            Property.State.Expanded = EasyEditorGUI.FoldoutGroup(_foldoutGroupConfig);
        }

        private void OnContentGUI(Rect headerRect)
        {
            if (_editorInfo.ClassNameSameAsGameObjectName)
            {
                if (_editorInfo.ClassName != _component.gameObject.name)
                {
                    _editorInfo.ClassName = _component.gameObject.name;
                    ValueChanged();
                }
            }

            EditorGUILayout.LabelField("状态", _isBuild ? "已构建" : "未构建");

            EditorGUI.BeginDisabledGroup(_isBuild);
            EditorGUI.BeginChangeCheck();

            EditorGUI.BeginChangeCheck();
            _editorInfo.GenerateDirectory = SirenixEditorFields.FolderPathField(
                new GUIContent("代码生成目录"),
                _editorInfo.GenerateDirectory, "Assets", false, false);
            if (EditorGUI.EndChangeCheck())
            {
                GUIHelper.ExitGUI(false);
            }

            _editorInfo.Namespace =
                EditorGUILayout.TextField("命名空间", _editorInfo.Namespace);

            _editorInfo.ClassNameSameAsGameObjectName =
                EditorGUILayout.Toggle("类名与游戏对象名称相同",
                    _editorInfo.ClassNameSameAsGameObjectName);

            ViewModelHelper.CheckIdentifier("类名", _editorInfo.ClassName);
            using (new EditorGUI.DisabledScope(_editorInfo.ClassNameSameAsGameObjectName))
            {
                _editorInfo.ClassName =
                    EditorGUILayout.TextField("类名", _editorInfo.ClassName);
            }

            var lbl = _editorInfo.BaseClass == null
                ? "<None>"
                : _editorInfo.BaseClass.FullName;

            EasyEditorGUI.DrawSelectorDropdown(
                new SelectorDropdownConfig<Type>(
                    EditorHelper.TempContent("父级"),
                    EditorHelper.TempContent2(lbl),
                    ViewModelHelper.BaseTypes,
                    t => _editorInfo.BaseClass = t)
                {
                    MenuItemNameGetter = t => t.FullName
                });

            if (EditorGUI.EndChangeCheck())
            {
                ValueChanged();
            }

            EditorGUI.EndDisabledGroup();

            if (SirenixEditorGUI.SDFIconButton("构建", EditorGUIUtility.singleLineHeight,
                    SdfIconType.PencilFill))
            {
                _builder.Setup(_editorInfo, _component);
                if (!_builder.Check())
                {
                    return;
                }

                _builder.Build();
            }

            if (_classType != null)
            {
                if (SirenixEditorGUI.SDFIconButton("绑定", EditorGUIUtility.singleLineHeight,
                        SdfIconType.Bezier))
                {
                    Bind();
                }
            }
        }

        private void ValueChanged(bool withUpdateEditorData = true)
        {
            if (withUpdateEditorData)
            {
                _info.EditorData.Set(_editorInfo);
            }

            _classType = _editorInfo.GetClassType();
            EditorUtility.SetDirty(_component);
        }
        

        private void Bind()
        {
            var classType = _editorInfo.GetClassType();
            Debug.Assert(classType != null);

            Component comp;
            if (_component.GetType() != classType)
            {
                comp = _component.GetComponent(classType);
                if (comp == null)
                {
                    comp = _component.gameObject.AddComponent(classType);
                    ((IViewModel)comp).Info = _info;
                    ValueChanged(false);
                }
            }
            else
            {
                comp = _component;
            }

            InternalBind(comp);

            if (comp != _component)
            {
                Object.DestroyImmediate(_component);
                GUIHelper.ExitGUI(true);
            }
        }

        private static void InternalBind(Component component)
        {
            var children = ViewModelHelper.GetChildren(component.transform);

            var fields = component.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.GetCustomAttribute<ByViewBinderAttribute>() != null)
                .ToArray();

            foreach (var child in children)
            {
                var comp = (Component)child;
                var binderEditorInfo = child.Info.EditorData.Get<ViewBinderEditorInfo>();
                var f = fields.FirstOrDefault(f => GetOriginName(f) == binderEditorInfo.Name);
                if (f == null)
                {
                    EditorUtility.DisplayDialog("错误",
                        $"绑定GameObject（{comp.gameObject.name}）失败，" +
                        $"视图模型中没有“{binderEditorInfo.Name}”，" +
                        $"可能需要重新生成视图模型！", "确认");
                    return;
                }

                var value = comp.GetComponent(f.FieldType);
                if (value == null)
                {
                    EditorUtility.DisplayDialog("错误",
                        $"绑定GameObject（{comp.gameObject.name}）失败，" +
                        $"没有组件“{f.FieldType}”", "确认");
                    return;
                }

                f.SetValue(component, value);
            }

            return;

            string GetOriginName(FieldInfo field) =>
                field.GetCustomAttribute<ByViewBinderAttribute>().OriginName;
        }
    }
}
