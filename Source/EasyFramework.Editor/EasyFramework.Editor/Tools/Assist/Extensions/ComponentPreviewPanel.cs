using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyFramework;
using EasyFramework.Inspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace EasyGameFramework.Editor
{
    public class ComponentPreviewPanel
    {
        private static class Icons
        {
            public static readonly GUIContent Edit = EditorGUIUtility.IconContent("d_Grid.PaintTool@2x");
            public static readonly GUIContent Warn = EditorGUIUtility.IconContent("d_console.warnicon");
        }

        private static class Styles
        {
            public static readonly GUIStyle Footer = "RL FooterButton";
        }

        private static readonly float s_blockWidth = EditorGUIUtility.singleLineHeight;
        private static readonly string s_copiedComponentIdPrefsKey = "Component Tool Panel Copied Component ID";

        private class TargetItem
        {
            public List<Component> Components;
            public GameObject Target;

            private EasyReorderableList _listDrawer;
            private bool _foldout;
            private ComponentPreviewPanel _editor;
            private bool _initialized;

            private List<Component> GetComponents()
            {
                return Target.GetComponents<Component>().Where(IsVisibleComponent).ToList();
            }

            public TargetItem(GameObject target, ComponentPreviewPanel editor)
            {
                Target = target;
                _editor = editor;
                Components = GetComponents();
            }


            private void Initialize()
            {
                if (_initialized) return;
                _initialized = true;

                _listDrawer = new EasyReorderableList(new List<Component>(Components),
                    EasyReorderableListThemes.SquareLike);
                _foldout = true;

                _listDrawer.OnAddDropdownCallback += buttonRect =>
                {
                    EasyEditorHelper.ShowAddComponentWindow(
                        new Rect(Screen.width / 2f - 230f / 2f, buttonRect.y + s_blockWidth, 230, 0),
                        _editor._targetItems.Select(i => i.Target).ToArray());
                };

                _listDrawer.OnExpandCallback += () =>
                {
                    foreach (var c in Components)
                    {
                        InternalEditorUtility.SetIsInspectorExpanded(c, true);
                    }

                    EasyEditorHelper.ForceRebuildInspectors();
                };

                _listDrawer.OnCollapseCallback += () =>
                {
                    foreach (var c in Components)
                    {
                        InternalEditorUtility.SetIsInspectorExpanded(c, false);
                    }

                    EasyEditorHelper.ForceRebuildInspectors();
                };

                _listDrawer.DrawHeaderCallback += rect =>
                {
                    var foldoutRect = new Rect(rect);
                    foldoutRect.x += 10;
                    foldoutRect.width -= 10;
                    if (_editor._targetItems.Count > 1)
                    {
                        _foldout = SirenixEditorGUI.Foldout(foldoutRect, _foldout, $"{Target.name}");
                    }
                    else
                    {
                        _foldout = SirenixEditorGUI.Foldout(foldoutRect, _foldout, "组件预览面板");
                    }
                };

                _listDrawer.DrawElementCallback += (rect, index, active, focused) =>
                {
                    rect.y += 2;
                    _editor.DrawElement(rect, Components[index], index);
                };

                _listDrawer.OnReorderDecide += (index, newIndex) =>
                {
                    if (index == 0)
                    {
                        return false;
                    }

                    if (Components[index] == null)
                    {
                        return false;
                    }

                    return true;
                };

                _listDrawer.OnReorderCallback += () =>
                {
                    //Move Down
                    for (int i = 0; i < Components.Count; i++)
                    {
                        int indexOf = _listDrawer.List.IndexOf(Components[i]);
                        int difference = indexOf - i;
                        if (difference <= 0) continue;

                        if (i == 0)
                        {
                            break;
                        }

                        for (int j = 0; j < Mathf.Abs(difference); j++)
                        {
                            ComponentUtility.MoveComponentDown(Components[i]);
                        }
                    }

                    //Move Up
                    Components = GetComponents();
                    for (int i = Components.Count - 1; i >= 0; i--)
                    {
                        int indexOf = _listDrawer.List.IndexOf(Components[i]);
                        int difference = indexOf - i;
                        if (difference >= 0) continue;

                        if (i == 0)
                        {
                            break;
                        }

                        for (int j = 0; j < Mathf.Abs(difference); j++)
                        {
                            ComponentUtility.MoveComponentUp(Components[i]);
                        }
                    }
                };

                _listDrawer.OnCanRemoveCallback += index =>
                {
                    var component = (Component)_listDrawer.List[index];
                    return IsRemovableComponent(component);
                };

                _listDrawer.OnRemoveCallback += index =>
                {
                    var component = (Component)_listDrawer.List[index];
                    var targetComponents = _editor.GetTargetComponents(component);
                    foreach (var t in targetComponents)
                    {
                        if (IsDependantComponent(t, out var dependantTarget))
                        {
                            EditorUtility.DisplayDialog("错误",
                                $"不能删除\"{t.GetType()}\"因为\"{dependantTarget.GetType()}\"依赖于它",
                                "确认");
                        }
                        else
                        {
                            Undo.SetCurrentGroupName("Remove " + t.GetType().Name);
                            Undo.DestroyObjectImmediate(t);
                        }
                    }

                    GUIUtility.ExitGUI();
                };
            }

            public void DrawList()
            {
                Initialize();
                _listDrawer.HasListElementTopPadding = false;
                _listDrawer.DisplayElements = _foldout;
                _listDrawer.DoLayoutList();
            }
        }

        private GameObject _gameObject;
        private List<TargetItem> _targetItems;
        private bool _expandTargets;

        public void OnEnable(GameObject target, IEnumerable<GameObject> targets)
        {
            _gameObject = target;
            _targetItems = new List<TargetItem>();
            foreach (var o in targets)
            {
                _targetItems.Add(new TargetItem(o, this));
            }
        }

        public void OnHeaderGUI()
        {
            if (_targetItems.Count > 1)
            {
                _expandTargets =
                    EditorGUILayout.Foldout(_expandTargets, $"多个游戏对象的组件预览面板（数量：{_targetItems.Count}）", true);
            }

            if (_targetItems.Count == 1 || _expandTargets)
            {
                foreach (var item in _targetItems)
                {
                    item.DrawList();
                }
            }

            // serializedObject.ApplyModifiedProperties();
        }

        private void DrawElement(Rect rect, Component component, int index)
        {
            if (component == null)
            {
                rect.x += 14;
                var iconRect = new Rect(rect);
                iconRect.width = iconRect.height = s_blockWidth;

                EditorGUI.LabelField(iconRect, Icons.Warn);

                rect.x += (s_blockWidth + 1) * 2;
                rect.y -= 2;
                //Name Handler
                GUIStyle guiStyle = new GUIStyle(EditorStyles.boldLabel);
                guiStyle.normal.textColor = guiStyle.onNormal.textColor = new Color32(209, 137, 24, 255);
                EditorGUI.LabelField(rect, "Mono脚本丢失！", guiStyle);
                return;
            }

            //Is the components common for every inspected GameObject?

            var compIconRect = new Rect(rect);
            compIconRect.x += 14;
            compIconRect.width = compIconRect.height = s_blockWidth;

            var enableToggleRect = new Rect(compIconRect);
            enableToggleRect.x += s_blockWidth + 1;

            var componentLabelRect = new Rect(enableToggleRect)
            {
                width = rect.width - 125
            };
            componentLabelRect.x += s_blockWidth + 1;

            var editBtnRect = new Rect(rect)
            {
                x = rect.xMax
            };
            editBtnRect.width = editBtnRect.height = s_blockWidth;
            editBtnRect.x -= EditorGUIUtility.singleLineHeight;
            editBtnRect.x -= EditorGUIUtility.singleLineHeight - 1;

            bool isCommon = IsCommonComponentInTargets(component);

            // 先绘制Enable开关
            if (EditorUtility.GetObjectEnabled(component) != -1)
            {
                bool oldValue = EditorUtility.GetObjectEnabled(component) == 1;
                bool newValue = EditorGUI.Toggle(enableToggleRect, oldValue);
                if (oldValue != newValue)
                {
                    var targetComponents = GetTargetComponents(component);
                    foreach (var t in targetComponents)
                    {
                        Undo.RecordObject(t, (newValue ? "Enable " : "Disable ") + t.GetType().Name);
                        EditorUtility.SetObjectEnabled(t, newValue);
                    }
                }
            }

            var script = component.GetScript();
            if (script != null)
            {
                string path = AssetDatabase.GetAssetPath(script);

                if (path.EndsWith(".cs") || path.EndsWith(".js"))
                {
                    if (GUI.Button(editBtnRect, new GUIContent(Icons.Edit.image, "编辑脚本"), Styles.Footer))
                    {
                        if (Event.current.button == 0)
                            AssetDatabase.OpenAsset(script);
                        else
                            EditorGUIUtility.PingObject(script);
                    }
                }
            }

            var foldoutRect = new Rect(rect);
            foldoutRect.x += 10;
            foldoutRect.width -= 10;
            foldoutRect.y -= 2;

            var originExpand = InternalEditorUtility.GetIsInspectorExpanded(component);
            var expand = SirenixEditorGUI.Foldout(foldoutRect, originExpand, GUIContent.none);

            if (originExpand != expand)
            {
                InternalEditorUtility.SetIsInspectorExpanded(component, expand);

                var targetComponents = GetTargetComponents(component, false);
                foreach (var c in targetComponents)
                    InternalEditorUtility.SetIsInspectorExpanded(c, expand);

                EasyEditorHelper.ForceRebuildInspectors();
            }

            Texture icon = EditorGUIUtility.ObjectContent(component, component.GetType()).image;
            EditorGUI.LabelField(compIconRect, new GUIContent(icon));

            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            if (isCommon)
                style.normal.textColor = style.onNormal.textColor = new Color(0f, 1f, 0.3f);
            EditorGUI.LabelField(componentLabelRect, component.GetType().Name, style);

            // //Reset Button
            // rect.x -= 18;
            // rect.width = 20;
            // rect.height += 4;
            // EditorGUI.LabelField(rect, new GUIContent(IconManager.ResetIcon.image, "Reset Component"));
            // if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
            // {
            // 	var targetComponents = GetTargetComponents(component);
            // 	foreach (var t in targetComponents)
            // 	{
            // 		Undo.RecordObject(t, "Reset " + t.GetType().Name);
            // 		Unsupported.SmartReset(t);
            // 	}
            // }
            //
            // rect.height -= 4;
            //
            // //Copy Button
            // rect.x -= 17;
            // rect.height += 1;
            // EditorGUI.LabelField(rect, new GUIContent(IconManager.DocumentIcon.image));
            // rect.x += 2;
            // rect.y += 2;
            // EditorGUI.LabelField(rect, new GUIContent(IconManager.DocumentIcon.image, "Copy Component"));
            // rect.height -= 1;
            //
            // //Inspect
            // rect.x -= 18;
            // rect.y -= 2;

            //
            // rect.y += 2;
            //
            // //Paste Values Button
            // rect.height += 1;
            // rect.x -= 16;
            // if (CopiedComponent != null && CopiedComponent.GetType() == component.GetType())
            // {
            // 	rect.y -= 3;
            // 	EditorGUI.LabelField(rect, new GUIContent(IconManager.FolderIcon));
            // 	rect.x += 2;
            // 	rect.y += 3;
            // 	EditorGUI.LabelField(rect, new GUIContent(IconManager.DocumentIcon.image, "Paste Component Values"));
            // 	if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
            // 	{
            // 		var targetComponents = GetTargetComponents(component);
            // 		foreach (var t in targetComponents)
            // 			ComponentUtility.PasteComponentValues(t);
            // 	}
            //
            // 	rect.x -= 2;
            // }
        }

        private List<Component> GetTargetComponents(Component component, bool includeSelf = true)
        {
            var total = new List<Component>();

            if (includeSelf)
            {
                total.Add(component);
            }

            if (component != null)
            {
                // 获取自己在GameObject中重复项的索引
                var ct = component.GetType();
                var ci = component.GetComponents(ct).IndexOf(component);

                foreach (var comps in _targetItems.Where(i => i.Target != component.gameObject)
                             .Select(i => i.Components))
                {
                    foreach (var c in comps)
                    {
                        if (c == null) continue;
                        var ct1 = c.GetType();
                        // 如果类型相同。再判断他们重复项索引是否相同
                        if (ct1 == ct)
                        {
                            // 获取重复项索引，如果要对比的和当前的都相同，说明是匹配项
                            var ci2 = c.GetComponents(ct1).IndexOf(c);
                            if (ci == ci2)
                            {
                                total.Add(c);
                            }
                        }
                    }
                }
            }

            return total;
        }

        private bool IsRepeatedComponent(Component component)
        {
            var comps = component.gameObject.GetComponents<Component>();
            if (comps.Length > 1)
            {
                var i = comps.IndexOf(component);
                return i != 0;
            }

            return false;
        }

        private bool IsCommonComponentInTargets(Component component)
        {
            return GetTargetComponents(component, false).Count > 0;
        }

        private static bool IsVisibleComponent(Component component)
        {
            if (component == null) return true;
            var t = component.GetType();
            if (t == typeof(Transform))
            {
                return false;
            }

            //TODO IsVisibleComponent
            if (t.Name == "ParticleSystemRenderer")
            {
                return false;
            }

            return true;
        }

        private static bool IsDependantComponent(Component component, out Component dependantTarget)
        {
            dependantTarget = null;
            if (component == null) return false;

            var components = component.GetComponents<Component>();
            var t = component.GetType();
            foreach (var c in components)
            {
                if (c == null) continue;
                var attrs = c.GetType().GetCustomAttributes<RequireComponent>();
                if (attrs.Any(attr => attr.m_Type0 == t || attr.m_Type1 == t || attr.m_Type2 == t))
                {
                    dependantTarget = c;
                    return true;
                }
            }

            return false;
        }

        private static bool IsRemovableComponent(Component component)
        {
            if (component == null) return true;

            var t = component.GetType();
            if (t == typeof(Transform) || t == typeof(RectTransform))
            {
                return false;
            }

            return true;
        }
    }
}
