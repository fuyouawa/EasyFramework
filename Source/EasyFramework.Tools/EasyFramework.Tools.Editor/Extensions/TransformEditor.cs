using System;
using System.Collections.Generic;
using System.Linq;
using EasyFramework.Generic;
using EasyFramework.Inspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace EasyFramework.Tools.Editor
{
    [CustomEditor(typeof(Transform), true)]
    [CanEditMultipleObjects]
    public class TransformEditor : ExtendEditor<Transform>
    {
        protected override string EditorTypeName => "UnityEditor.TransformInspector, UnityEditor";

        class Contents
        {
            public readonly GUIContent PositionContent = EditorGUIUtility.TrTextContent("Position");
            public readonly GUIContent RotationContent = EditorGUIUtility.TrTextContent("Rotation");
            public readonly GUIContent ScaleContent = EditorGUIUtility.TrTextContent("Scale");
        }

        private static Contents _contents;
        public override void OnInspectorGUI()
        {
            _contents ??= new Contents();
            
            _expand = EasyEditorGUI.WindowLikeToolGroup(new WindowLikeToolGroupConfig(this, "组件预览面板", _expand)
            {
                OnContentGUI = () => OnComponentViewPanelGUI(Target),
                OnMaximize = () => OnExpandAllComponents(Target, true),
                OnMinimize = () => OnExpandAllComponents(Target, false),
                OnTitleBarGUI = rect => OnComponentViewPanelTitleBarGUI(Target, rect)
            });

            SirenixEditorGUI.Title("局部空间", string.Empty, TextAlignment.Left, true);
            // EditorGUILayout.LabelField("Local Space", EditorStyles.boldLabel);

            base.OnInspectorGUI();
            
            SirenixEditorGUI.Title("世界空间", string.Empty, TextAlignment.Left, true);
            // EditorGUILayout.LabelField("World", EditorStyles.boldLabel);

            var v = Target.position;
            var w = EditorGUIUtility.singleLineHeight;
            using (new EditorGUILayout.HorizontalScope())
            {
                var v2 = EditorGUILayout.Vector3Field(_contents.PositionContent, v);
                if (GUILayout.Button("Z", GUILayout.Width(w)))
                    v2 = Vector3.zero;
                if (v != v2)
                    Target.position = v2;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                v = Target.rotation.eulerAngles;
                var v2 = EditorGUILayout.Vector3Field(_contents.RotationContent, v);
                if (GUILayout.Button("Z", GUILayout.Width(w)))
                    v2 = Vector3.zero;
                if (v != v2)
                    Target.rotation = Quaternion.Euler(v2);
            }


            v = Target.lossyScale;
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.Vector3Field(_contents.ScaleContent, v);
            }
        }
        
        private bool _expand = true;
        void OnComponentViewPanelTitleBarGUI(Transform transform, Rect rect)
        {
            if (EasyEditorGUI.ToolbarButton(
                    new GUIContent(EasyEditorIcons.AddDropdown.image, "添加组件"),
                    SirenixEditorGUI.currentDrawingToolbarHeight))
            {
                EasyEditorHelper.ShowAddComponentWindow(
                    new Rect(Screen.width / 2f - 230f / 2f, rect.y + rect.height, 230, 0),
                    new GameObject[] { transform.gameObject });
            }
        }
        
        void OnExpandAllComponents(Transform transform, bool b)
        {
            foreach (var comp in transform.GetComponents<Component>())
            {
                var t = comp == null ? null : comp.GetType();
                if (t != typeof(Transform) && t != typeof(RectTransform))
                {
                    InternalEditorUtility.SetIsInspectorExpanded(comp, b);
                    EasyEditorHelper.ForceRebuildInspectors();
                }
            }
        }
        
        void OnComponentViewPanelGUI(Transform transform)
        {
            int i = 0;
            foreach (var comp in transform.GetComponents<Component>())
            {
                var t = comp == null ? null : comp.GetType();
                if (t != typeof(Transform) && t != typeof(RectTransform))
                {
                    OnComponentViewPanelItemGUI(comp, i);
                    ++i;
                }
            }
        }
        
        void OnComponentViewPanelItemGUI(Component component, int index)
        {
            var height = EditorGUIUtility.singleLineHeight - 2;
            var btnWidth = height;
        
            var rect = EditorGUILayout.GetControlRect(false, height);
        
            var drawRect = new Rect(rect);
            drawRect.height += 2;
            EditorGUI.DrawRect(drawRect, index % 2 == 0 ? new Color(0.23f, 0.23f, 0.23f) : new Color(0.2f, 0.2f, 0.2f));
        
            if (component == null)
            {
                rect.x += 14;
                var iconRect = new Rect(rect);
                iconRect.width = iconRect.height = btnWidth;
        
                EditorGUI.LabelField(iconRect, EasyEditorIcons.Warn);
        
                rect.x += (btnWidth + 1) * 2;
                //Name Handler
                GUIStyle guiStyle = new GUIStyle(EditorStyles.boldLabel);
                guiStyle.normal.textColor = guiStyle.onNormal.textColor = new Color32(209, 137, 24, 255);
                EditorGUI.LabelField(rect, "Mono脚本丢失！", guiStyle);
                return;
            }
        
            var compName = component.GetType().Name;
        
            var foldoutRect = new Rect(rect);
        
            var compIconRect = new Rect(rect);
            compIconRect.x += 14;
            compIconRect.width = compIconRect.height = btnWidth;
        
            var enableToggleRect = new Rect(compIconRect);
            enableToggleRect.x += btnWidth + 1;
        
            var componentLabelRect = new Rect(enableToggleRect)
            {
                width = rect.width - 125
            };
            componentLabelRect.x += btnWidth + 1;
        
            var editBtnRect = new Rect(rect)
            {
                x = rect.xMax
            };
            editBtnRect.width = editBtnRect.height = btnWidth;
            editBtnRect.x -= EditorGUIUtility.singleLineHeight;
            editBtnRect.x -= EditorGUIUtility.singleLineHeight - 1;
        
            if (EditorUtility.GetObjectEnabled(component) != -1)
            {
                bool oldValue = EditorUtility.GetObjectEnabled(component) == 1;
                bool newValue = EditorGUI.Toggle(enableToggleRect, oldValue);
                if (oldValue != newValue)
                {
                    Undo.RecordObject(component, (newValue ? "Enable " : "Disable ") + compName);
                    EditorUtility.SetObjectEnabled(component, newValue);
                }
            }

            var script = component.GetScript();
            if (script != null)
            {
                string path = AssetDatabase.GetAssetPath(script);
        
                if (path.EndsWith(".cs") || path.EndsWith(".js"))
                {
                    if (GUI.Button(editBtnRect, new GUIContent(EasyEditorIcons.Edit.image, "编辑脚本"), "RL FooterButton"))
                    {
                        if (Event.current.button == 0)
                            AssetDatabase.OpenAsset(script);
                        else
                            EditorGUIUtility.PingObject(script);
                    }
                }
            }
        
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var expand = EditorGUI.Foldout(foldoutRect, InternalEditorUtility.GetIsInspectorExpanded(component),
                    GUIContent.none, true);
                InternalEditorUtility.SetIsInspectorExpanded(component, expand);
                if (check.changed)
                {
                    bool value = InternalEditorUtility.GetIsInspectorExpanded(component);
                    InternalEditorUtility.SetIsInspectorExpanded(component, value);
                    EasyEditorHelper.ForceRebuildInspectors();
                }
            }
        
            var icon = EditorGUIUtility.ObjectContent(component, component.GetType()).image;
            EditorGUI.LabelField(compIconRect, new GUIContent(icon));
        
            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            // if (isCommon)
            //     style.normal.textColor = style.onNormal.textColor = new Color(0f, 1f, 0.3f);
            EditorGUI.LabelField(componentLabelRect, component.GetType().Name, style);
        }
    }
}
