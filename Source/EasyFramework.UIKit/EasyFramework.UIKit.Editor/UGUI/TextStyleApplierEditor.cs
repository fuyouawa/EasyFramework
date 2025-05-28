using System.Linq;
using EasyFramework.Core;
using EasyFramework.Editor;
using EasyFramework.ToolKit.Editor;
using Sirenix.OdinInspector.Editor;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.UIKit.Editor
{
    [CustomEditor(typeof(TextStyleApplier))]
    [CanEditMultipleObjects]
    public class TextStyleApplierEditor : OdinEditor
    {
        private InspectorProperty _styleNameProperty;
        private InspectorProperty _ignoreFontAssetProperty;
        private InspectorProperty _ignoreFontMaterialProperty;
        private InspectorProperty _ignoreFontSizeProperty;
        private InspectorProperty _ignoreFontColorProperty;

        private TextStyle _style;

        protected override void OnEnable()
        {
            base.OnEnable();
            _styleNameProperty = Tree.RootProperty.Children["_styleName"];
            _ignoreFontAssetProperty = Tree.RootProperty.Children["_ignoreFontAsset"];
            _ignoreFontMaterialProperty = Tree.RootProperty.Children["_ignoreFontMaterial"];
            _ignoreFontSizeProperty = Tree.RootProperty.Children["_ignoreFontSize"];
            _ignoreFontColorProperty = Tree.RootProperty.Children["_ignoreFontColor"];

            _styleNameProperty.ValueEntry.OnValueChanged += _ => RefreshStyle();
            RefreshStyle();
        }

        private void RefreshStyle()
        {
            _style = TextStyleLibrary.Instance.GetStyleByName(_styleNameProperty.ValueEntry.WeakSmartValueT<string>());
        }

        protected override void DrawTree()
        {
            Tree.BeginDraw(true);

            EditorGUI.BeginChangeCheck();
            EasyEditorGUI.DrawSelectorDropdown(
                () => TextStyleLibrary.Instance.Styles.Select(style => style.Name),
                EditorHelper.TempContent("样式"),
                _styleNameProperty.GetSmartContent(),
                styleName => _styleNameProperty.ValueEntry.SetAllWeakValues(styleName));
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var o in targets)
                {
                    EditorUtility.SetDirty(o);
                }
            }

            _ignoreFontAssetProperty.DrawEx("忽略字体资产");
            _ignoreFontMaterialProperty.DrawEx("忽略字体材质");
            _ignoreFontSizeProperty.DrawEx("忽略字体大小");
            _ignoreFontColorProperty.DrawEx("忽略字体颜色");

            if (_style != null && _styleNameProperty.ValueEntry.WeakValues.Cast<string>().AllSame())
            {
                _styleNameProperty.State.Expanded = EasyEditorGUI.FoldoutGroup(
                    _styleNameProperty,
                    "样式查看",
                    _styleNameProperty.State.Expanded,
                    rect =>
                    {
                        EditorGUI.BeginDisabledGroup(true);

                        EditorGUILayout.ObjectField(
                            EditorHelper.TempContent("字体资产"),
                            _style.FontAsset, typeof(TMP_FontAsset), false);

                        EditorGUILayout.ObjectField(
                            EditorHelper.TempContent("字体材质"),
                            _style.FontMaterial, typeof(Material), false);

                        EditorGUILayout.FloatField(
                            EditorHelper.TempContent("字体大小"),
                            _style.FontSize);

                        EditorGUILayout.ColorField(
                            EditorHelper.TempContent("字体颜色"),
                            _style.FontColor);

                        EditorGUI.EndDisabledGroup();
                    });
            }

            if (GUILayout.Button("打开设置面板"))
            {
                SettingsWindow.ShowWindow();
            }

            Tree.EndDraw();
        }
    }
}
