using System;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class InlineEditorAttributeDrawer<T> : EasyAttributeDrawer<InlineEditorAttribute, T>
        where T : UnityEngine.Object
    {
        private UnityEditor.Editor _editor;

        protected override void DrawProperty(GUIContent label)
        {
            switch (Attribute.Style)
            {
                case InlineEditorStyle.Place:
                    DrawObjectField(label);
                    DrawEditor();
                    break;
                case InlineEditorStyle.PlaceWithHide:
                    if (ValueEntry.SmartValue == null)
                    {
                        DrawObjectField(label);
                    }
                    DrawEditor();
                    break;
                case InlineEditorStyle.Box:
                    DrawObjectField(label);
                    break;
                case InlineEditorStyle.Foldout:
                    if (ValueEntry.SmartValue != null)
                    {
                        Property.State.Expanded = EasyEditorGUI.Foldout(
                            Property.State.Expanded, label, out var valueRect);
                        DrawObjectField(valueRect, GUIContent.none);

                        EditorGUI.indentLevel++;
                        if (Property.State.Expanded)
                        {
                            DrawEditor();
                        }
                        EditorGUI.indentLevel--;
                    }
                    else
                    {
                        DrawObjectField(label);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DrawObjectField(GUIContent label)
        {
            var rect = EditorGUILayout.GetControlRect(label != null);
            DrawObjectField(rect, label);
        }

        private void DrawObjectField(Rect rect, GUIContent label)
        {
            var value = ValueEntry.SmartValue;
            EditorGUI.BeginChangeCheck();

            bool assetsOnly = Property.GetAttribute<AssetsOnlyAttribute>() == null;
            if (label == null)
            {
                value = (T)EditorGUI.ObjectField(rect, value, typeof(T), assetsOnly);
            }
            else
            {
                value = (T)EditorGUI.ObjectField(rect, label, value, typeof(T), assetsOnly);
            }

            if (EditorGUI.EndChangeCheck())
            {
                DestroyEditor();
                ValueEntry.SmartValue = value;
            }
        }

        private void DrawEditor()
        {
            if (ValueEntry.SmartValue == null)
            {
                return;
            }

            if (ValueEntry.IsConflicted())
            {
                EditorGUILayout.HelpBox("This object is in conflict state and cannot be edited.", MessageType.Warning);
                return;
            }

            if (_editor == null)
            {
                _editor = UnityEditor.Editor.CreateEditor(ValueEntry.SmartValue);

                if (_editor is EasyEditor easyEditor)
                {
                    easyEditor.IsInlineEditor = true;
                }
            }

            var prev = InspectorConfigAsset.Instance.DrawMonoScriptInEditor;
            InspectorConfigAsset.Instance.DrawMonoScriptInEditor = false;
            _editor.OnInspectorGUI();
            InspectorConfigAsset.Instance.DrawMonoScriptInEditor = prev;
        }

        private void DestroyEditor()
        {
            if (_editor != null)
            {
                UnityEngine.Object.DestroyImmediate(_editor);
                _editor = null;
            }
        }
    }
}
