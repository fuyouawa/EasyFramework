using System;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Editor.Drawer
{
    public class ViewBinderInfoDrawer : OdinValueDrawer<ViewBinderInfo>
    {
        private Component _component;
        private ViewBinderEditorInfo _editorInfo;
        private ViewBinderInfo _info;
        private Transform[] _parents;
        private FoldoutGroupConfig _foldoutGroupConfig;

        private ViewBinderSettings _settings;

        protected override void Initialize()
        {
            base.Initialize();

            _component = Property.Parent.WeakSmartValue<Component>();
            _info = ValueEntry.SmartValue;
            _editorInfo = _info.EditorData.Get<ViewBinderEditorInfo>();
            if (_editorInfo == null)
            {
                _editorInfo = new ViewBinderEditorInfo();
                _info.EditorData.Set(_editorInfo);
            }

            _settings = ViewBinderSettings.Instance;

            _foldoutGroupConfig = new FoldoutGroupConfig(
                UniqueDrawerKey.Create(Property, this),
                new GUIContent("ViewBinder"), true, OnContentGUI);

            _parents = _component.transform.FindObjectsByTypeInParents<IViewModel>()
                .Select(x => ((Component)x).transform).ToArray();

            ViewBinderHelper.InitializeBinder((IViewBinder)_component);
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
                if (_editorInfo.Name != _component.gameObject.name)
                {
                    _editorInfo.Name = _component.gameObject.name;
                    ValueChanged();
                }
            }

            EditorGUI.BeginChangeCheck();

            EasyEditorField.Value(
                EditorHelper.TempContent("绑定游戏对象"),
                ref _editorInfo.BindGameObject);

            if (!_editorInfo.BindGameObject)
            {
                if (_editorInfo.BindComponent == null)
                {
                    EasyEditorGUI.MessageBox("必须得有一个要绑定的组件", MessageType.Error);
                }

                GUIContent bindComponentBtnLabel;
                if (_editorInfo.BindComponent == null)
                {
                    bindComponentBtnLabel = EditorHelper.NoneSelectorBtnLabel;
                }
                else
                {
                    bindComponentBtnLabel = EditorHelper.TempContent2(_editorInfo.BindComponent.GetType().FullName);
                    bindComponentBtnLabel.image =
                        GUIHelper.GetAssetThumbnail(_editorInfo.BindComponent, _editorInfo.BindComponent.GetType(),
                            true);
                }

                EasyEditorGUI.DrawSelectorDropdown(
                    _component.GetComponents<Component>()
                        .Where(c => c != null),
                    EditorHelper.TempContent("要绑定的组件"),
                    bindComponentBtnLabel,
                    t => _editorInfo.BindComponent = t,
                    t => t.GetType().FullName);

                if (_editorInfo.BindComponent != null)
                {
                    var baseTypes = _editorInfo.BindComponent.GetType().GetParentTypes(includeTargetType: true);
                    if (_editorInfo.BindType == null || !Array.Exists(baseTypes, t => t == _editorInfo.BindType))
                    {
                        _editorInfo.BindType = _editorInfo.BindComponent.GetType();
                        ValueChanged();
                    }

                    var btnLabel = _editorInfo.BindType == null
                        ? EditorHelper.NoneSelectorBtnLabel
                        : EditorHelper.TempContent2(_editorInfo.BindType.FullName);

                    EasyEditorGUI.DrawSelectorDropdown(
                        baseTypes,
                        EditorHelper.TempContent("绑定类型"),
                        btnLabel,
                        t => _editorInfo.BindType = t);
                }
            }

            if (_info.Owner == null)
            {
                EasyEditorGUI.MessageBox("必须得有一个父级", MessageType.Error);
            }

            GUIContent parentBtnLabel;
            if (_info.Owner == null)
            {
                parentBtnLabel = EditorHelper.NoneSelectorBtnLabel;
            }
            else
            {
                parentBtnLabel = EditorHelper.TempContent2(_info.Owner.gameObject.name);
            }

            EasyEditorGUI.DrawSelectorDropdown(
                _parents,
                EditorHelper.TempContent("父级"),
                parentBtnLabel,
                c => _info.Owner = c,
                c => c.gameObject.name);

            _editorInfo.NameSameAsGameObjectName = EditorGUILayout.Toggle(
                "变量名称与游戏对象名称相同",
                _editorInfo.NameSameAsGameObjectName);

            if (!_editorInfo.NameSameAsGameObjectName)
            {
                ViewModelHelper.CheckIdentifierWithMessage("变量名称", _editorInfo.Name);
                _editorInfo.Name =
                    EditorGUILayout.TextField("变量名称", _editorInfo.Name);
            }

            _editorInfo.AutoNamingNotations = EditorGUILayout.Toggle(
                "自动命名规范",
                _editorInfo.AutoNamingNotations);

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.TextField("实际变量名称", ((IViewBinder)_component).GetBindName());
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
                ValueChanged();
            }
        }

        private void ValueChanged()
        {
            _info.EditorData.Set(_editorInfo);
            EditorUtility.SetDirty(_component);
        }
    }
}
