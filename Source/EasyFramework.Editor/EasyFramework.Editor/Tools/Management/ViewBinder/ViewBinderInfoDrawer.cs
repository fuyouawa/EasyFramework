using System;
using System.Collections.Generic;
using System.Linq;
using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Editor
{
    public class ViewBinderInfoDrawer : OdinValueDrawer<ViewBinderInfo>
    {
        private Component _component;
        private ViewBinderEditorInfo _editorInfo;
        private ViewBinderInfo _info;
        private List<Transform> _parents;
        private FoldoutGroupConfig _foldoutGroupConfig;

        private ViewBinderSettings _settings;

        protected override void Initialize()
        {
            base.Initialize();

            _component = Property.Parent.WeakSmartValue<Component>();
            _info = ValueEntry.SmartValue;
            _editorInfo = _info.EditorData.Get<ViewBinderEditorInfo>() ?? new ViewBinderEditorInfo();
            _settings = ViewBinderSettings.Instance;

            _foldoutGroupConfig = new FoldoutGroupConfig(
                UniqueDrawerKey.Create(Property, this),
                new GUIContent("ViewBinder"), true, OnContentGUI);

            _parents = _component.transform.FindParents(p =>
                p.gameObject.HasComponent<IViewModel>()).ToList();

            if (!_editorInfo.IsInitialized)
            {
                var candidateComponents = _component.GetComponents<Component>()
                    .Where(c => c != null)
                    .ToArray();

                Component initialComponent;
                if (candidateComponents.Length > 1)
                {
                    var t = candidateComponents[1];
                    if (t.GetType() == typeof(ViewBinder))
                    {
                        initialComponent = candidateComponents.Length > 2
                            ? candidateComponents[2]
                            : candidateComponents[0];
                    }
                    else
                    {
                        initialComponent = t;
                    }
                }
                else
                {
                    initialComponent = candidateComponents[0];
                }

                _info.BindComponent = initialComponent;

                if (_parents.IsNotNullOrEmpty())
                {
                    _info.OwnerViewModel = _parents[0];
                }

                _editorInfo.Name = _component.gameObject.name;
                _editorInfo.Access = _settings.Access;
                _editorInfo.AutoNamingNotations = _settings.AutoNamingNotations;
                _editorInfo.AutoAddCommentPara = _settings.AutoAddCommentPara;
                _editorInfo.Comment = _settings.Comment;

                _editorInfo.IsInitialized = true;
            }
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            _foldoutGroupConfig.Expand = Property.State.Expanded;
            Property.State.Expanded = EasyEditorGUI.FoldoutGroup(_foldoutGroupConfig);
        }

        private void OnContentGUI(Rect headerRect)
        {
            if (_editorInfo.NameSameAsGameObjectName)
            {
                _editorInfo.Name = _component.gameObject.name;
            }

            EditorGUI.BeginChangeCheck();

            if (_info.BindComponent == null)
            {
                EasyEditorGUI.MessageBox("必须得有一个要绑定的组件", MessageType.Error);
            }

            var btnLabel = _info.BindComponent == null
                ? EditorHelper.NoneSelectorBtnLabel.text
                : _info.BindComponent.GetType().FullName;

            EasyEditorGUI.DrawSelectorDropdown(
                new SelectorDropdownConfig<Component>(
                    EditorHelper.TempContent("要绑定的组件"),
                    EditorHelper.TempContent2(btnLabel),
                    _component.GetComponents<Component>()
                        .Where(c => c != null),
                    t => _info.BindComponent = t)
                {
                    // MenuItemNameGetter = t => t.FullName
                });

            if (_info.OwnerViewModel == null)
            {
                EasyEditorGUI.MessageBox("必须得有一个父级", MessageType.Error);
            }

            btnLabel = _info.OwnerViewModel == null
                ? EditorHelper.NoneSelectorBtnLabel.text
                : _info.OwnerViewModel.gameObject.name;

            EasyEditorGUI.DrawSelectorDropdown(
                new SelectorDropdownConfig<Transform>(
                    EditorHelper.TempContent("父级"),
                    EditorHelper.TempContent2(btnLabel),
                    _parents,
                    c => _info.OwnerViewModel = c)
                {
                    MenuItemNameGetter = c => c.gameObject.name
                });

            _editorInfo.NameSameAsGameObjectName = EditorGUILayout.Toggle(
                "变量名称与游戏对象名称相同",
                _editorInfo.NameSameAsGameObjectName);

            ViewModelHelper.CheckIdentifier("变量名称", _editorInfo.Name);
            using (new EditorGUI.DisabledScope(_editorInfo.NameSameAsGameObjectName))
            {
                _editorInfo.Name =
                    EditorGUILayout.TextField("变量名称", _editorInfo.Name);
            }

            _editorInfo.AutoNamingNotations = EditorGUILayout.Toggle(
                "自动命名规范",
                _editorInfo.AutoNamingNotations);

            if (_editorInfo.AutoNamingNotations)
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.TextField("变量名称（自动命名规范）", _editorInfo.GetName());
                }
            }

            _editorInfo.Access = EasyEditorField.Enum(
                new GUIContent("访问标识符"),
                _editorInfo.Access);

            _editorInfo.AutoAddCommentPara = EditorGUILayout.Toggle(
                "注释自动添加段落xml",
                _editorInfo.AutoAddCommentPara);

            EditorGUILayout.PrefixLabel("注释");
            _editorInfo.Comment = EditorGUILayout.TextArea(_editorInfo.Comment);

            if (EditorGUI.EndChangeCheck())
            {
                _info.EditorData.Set(_editorInfo);
                EditorUtility.SetDirty(_component);
            }
        }
    }
}
