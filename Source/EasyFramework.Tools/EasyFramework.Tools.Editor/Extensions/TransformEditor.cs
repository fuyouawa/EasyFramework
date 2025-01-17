using EasyFramework.Inspector;
using UnityEditor;
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
            
            // OnComponentViewPanelGUI();

            EditorGUILayout.LabelField("局部空间", EditorStyles.boldLabel);
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("世界空间", EditorStyles.boldLabel);

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
            // _expand = EasyEditorGUI.WindowLikeToolGroup(new WindowLikeToolGroupConfig(this, "组件预览面板", _expand)
            // {
            //     OnContentGUI = () => OnComponentViewPanelGUI(Target),
            //     OnMaximize = () => OnExpandAllComponents(Target, true),
            //     OnMinimize = () => OnExpandAllComponents(Target, false),
            //     OnTitleBarGUI = rect => OnComponentViewPanelTitleBarGUI(Target, rect)
            // });
        }
        
        // private bool _expand = true;
        // private static MonoScript[] _allScripts;
        //
        // private bool TryGetComponentScript(Component component, out MonoScript script)
        // {
        //     if (_allScripts.IsNullOrEmpty())
        //     {
        //         _allScripts = MonoImporter.GetAllRuntimeMonoScripts();
        //     }
        //
        //     try
        //     {
        //         script = _allScripts.FirstOrDefault(s => s.GetClass() == component.GetType());
        //         return script != null;
        //     }
        //     catch (Exception e)
        //     {
        //         _allScripts = MonoImporter.GetAllRuntimeMonoScripts();
        //         script = _allScripts.FirstOrDefault(s => s.GetClass() == component.GetType());
        //         return script != null;
        //     }
        // }

        // void OnComponentViewPanelTitleBarGUI(Transform transform, Rect rect)
        // {
        //     if (EasyEditorGUI.ToolbarButton(
        //             new GUIContent(EasyEditorIcons.AddDropdown.image, "添加组件"),
        //             SirenixEditorGUI.currentDrawingToolbarHeight))
        //     {
        //         AddComponentWindowHelper.Show(
        //             new Rect(Screen.width / 2f - 230f / 2f, rect.y + rect.height, 230, 0),
        //             new GameObject[] { transform.gameObject });
        //     }
        // }
        //
        // void OnExpandAllComponents(Transform transform, bool b)
        // {
        //     foreach (var comp in transform.GetComponents<Component>())
        //     {
        //         var t = comp == null ? null : comp.GetType();
        //         if (t != typeof(Transform) && t != typeof(RectTransform))
        //         {
        //             InternalEditorUtility.SetIsInspectorExpanded(comp, b);
        //             UniEditorUtility.ForceRebuildInspectors();
        //         }
        //     }
        // }
        //
        // void OnComponentViewPanelGUI(Transform transform)
        // {
        //     int i = 0;
        //     foreach (var comp in transform.GetComponents<Component>())
        //     {
        //         var t = comp == null ? null : comp.GetType();
        //         if (t != typeof(Transform) && t != typeof(RectTransform))
        //         {
        //             OnComponentViewPanelItemGUI(comp, i);
        //             ++i;
        //         }
        //     }
        // }
        //
        // void OnComponentViewPanelItemGUI(Component component, int index)
        // {
        //     var height = EditorGUIUtility.singleLineHeight - 2;
        //     var btnWidth = height;
        //
        //     var rect = EditorGUILayout.GetControlRect(false, height);
        //
        //     var drawRect = new Rect(rect);
        //     drawRect.height += 2;
        //     EditorGUI.DrawRect(drawRect, index % 2 == 0 ? new Color(0.23f, 0.23f, 0.23f) : new Color(0.2f, 0.2f, 0.2f));
        //
        //     if (component == null)
        //     {
        //         rect.x += 14;
        //         var iconRect = new Rect(rect);
        //         iconRect.width = iconRect.height = btnWidth;
        //
        //         EditorGUI.LabelField(iconRect, EasyEditorIcons.Warn);
        //
        //         rect.x += (btnWidth + 1) * 2;
        //         //Name Handler
        //         GUIStyle guiStyle = new GUIStyle(EditorStyles.boldLabel);
        //         guiStyle.normal.textColor = guiStyle.onNormal.textColor = new Color32(209, 137, 24, 255);
        //         EditorGUI.LabelField(rect, "Mono脚本丢失！", guiStyle);
        //         return;
        //     }
        //
        //     var compName = component.GetType().Name;
        //
        //     var foldoutRect = new Rect(rect);
        //
        //     var compIconRect = new Rect(rect);
        //     compIconRect.x += 14;
        //     compIconRect.width = compIconRect.height = btnWidth;
        //
        //     var enableToggleRect = new Rect(compIconRect);
        //     enableToggleRect.x += btnWidth + 1;
        //
        //     var componentLabelRect = new Rect(enableToggleRect)
        //     {
        //         width = rect.width - 125
        //     };
        //     componentLabelRect.x += btnWidth + 1;
        //
        //     var editBtnRect = new Rect(rect)
        //     {
        //         x = rect.xMax
        //     };
        //     editBtnRect.width = editBtnRect.height = btnWidth;
        //     editBtnRect.x -= EditorGUIUtility.singleLineHeight;
        //     editBtnRect.x -= EditorGUIUtility.singleLineHeight - 1;
        //
        //     if (EditorUtility.GetObjectEnabled(component) != -1)
        //     {
        //         bool oldValue = EditorUtility.GetObjectEnabled(component) == 1;
        //         bool newValue = EditorGUI.Toggle(enableToggleRect, oldValue);
        //         if (oldValue != newValue)
        //         {
        //             Undo.RecordObject(component, (newValue ? "Enable " : "Disable ") + compName);
        //             EditorUtility.SetObjectEnabled(component, newValue);
        //         }
        //     }
        //
        //     if (TryGetComponentScript(component, out var script))
        //     {
        //         string path = AssetDatabase.GetAssetPath(script);
        //
        //         if (path.EndsWith(".cs") || path.EndsWith(".js"))
        //         {
        //             if (GUI.Button(editBtnRect, new GUIContent(EasyEditorIcons.Edit.image, "编辑脚本"), "RL FooterButton"))
        //             {
        //                 if (Event.current.button == 0)
        //                     AssetDatabase.OpenAsset(script);
        //                 else
        //                     EditorGUIUtility.PingObject(script);
        //             }
        //         }
        //     }
        //
        //     using (var check = new EditorGUI.ChangeCheckScope())
        //     {
        //         var expand = EditorGUI.Foldout(foldoutRect, InternalEditorUtility.GetIsInspectorExpanded(component),
        //             GUIContent.none, true);
        //         InternalEditorUtility.SetIsInspectorExpanded(component, expand);
        //         if (check.changed)
        //         {
        //             bool value = InternalEditorUtility.GetIsInspectorExpanded(component);
        //             InternalEditorUtility.SetIsInspectorExpanded(component, value);
        //             UniEditorUtility.ForceRebuildInspectors();
        //         }
        //     }
        //
        //     var icon = EditorGUIUtility.ObjectContent(component, component.GetType()).image;
        //     EditorGUI.LabelField(compIconRect, new GUIContent(icon));
        //
        //     GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
        //     // if (isCommon)
        //     //     style.normal.textColor = style.onNormal.textColor = new Color(0f, 1f, 0.3f);
        //     EditorGUI.LabelField(componentLabelRect, component.GetType().Name, style);
        // }

    //     #region 组件预览面板
    //
    //     protected override void OnEnable()
    //     {
    //         base.OnEnable();
    //         _targetItems = new List<TargetItem>();
    //         foreach (var o in targets)
    //         {
    //             _targetItems.Add(new TargetItem(o == null ? null : ((Transform)o).gameObject, this));
    //         }
    //     }
    //
    //     void OnComponentViewPanelGUI()
    //     {
    //         if (_targetItems.Count > 1)
    //         {
    //             _expandTargets = EditorGUILayout.Foldout(_expandTargets, $"多个游戏对象的组件预览面板（数量：{_targetItems.Count}）", true);
    //         }
    //
    //         if (_targetItems.Count == 1 || _expandTargets)
    //         {
    //             foreach (var item in _targetItems)
    //             {
    //                 item.DrawList();
    //             }
    //         }
    //
    //         serializedObject.ApplyModifiedProperties();
    //     }
    //
    //     private static class Styles
    //     {
    //         public static readonly GUIStyle Footer = "RL FooterButton";
    //     }
    //
    //     private static readonly float BlockWidth = EditorGUIUtility.singleLineHeight;
    //     private static readonly string CopiedComponentIDPrefsKey = "Component Tool Panel Copied Component ID";
    //
    //     private static MonoScript[] _allScripts;
    //
    //     private class TargetItem
    //     {
    //         public List<Component> Components;
    //         public GameObject Target;
    //
    //         private EasyReorderableList _listDrawer;
    //         private bool _foldout;
    //         private TransformEditor _editor;
    //         private bool _initialized;
    //
    //         private List<Component> GetComponents()
    //         {
    //             return Target.GetComponents<Component>().Where(IsVisibleComponent).ToList();
    //         }
    //
    //         public TargetItem(GameObject target, TransformEditor editor)
    //         {
    //             Target = target;
    //             _editor = editor;
    //             Components = GetComponents();
    //         }
    //
    //
    //         private void Initialize()
    //         {
    //             if (_initialized) return;
    //             _initialized = true;
    //
    //             _listDrawer = new EasyReorderableList(new List<Component>(Components), EasyReorderableListThemes.SquareLike);
    //             _foldout = true;
    //
    //             _listDrawer.OnAddDropdownCallback += buttonRect =>
    //             {
    //                 AddComponentWindowHelper.Show(
    //                     new Rect(Screen.width / 2f - 230f / 2f, buttonRect.y + BlockWidth, 230, 0),
    //                     _editor._targetItems.Select(i => i.Target).ToArray());
    //             };
    //
    //             _listDrawer.OnExpandCallback += () =>
    //             {
    //                 foreach (var c in Components)
    //                 {
    //                     InternalEditorUtility.SetIsInspectorExpanded(c, true);
    //                 }
    //
    //                 UniEditorUtility.ForceRebuildInspectors();
    //             };
    //
    //             _listDrawer.OnCollapseCallback += () =>
    //             {
    //                 foreach (var c in Components)
    //                 {
    //                     InternalEditorUtility.SetIsInspectorExpanded(c, false);
    //                 }
    //
    //                 UniEditorUtility.ForceRebuildInspectors();
    //             };
    //
    //             _listDrawer.DrawHeaderCallback += rect =>
    //             {
    //                 var foldoutRect = new Rect(rect);
    //                 foldoutRect.x += 14;
    //                 foldoutRect.width -= 14;
    //                 if (_editor._targetItems.Count > 1)
    //                 {
    //                     _foldout = EditorGUI.Foldout(foldoutRect, _foldout, new GUIContent($"{Target.name}"), true);
    //                 }
    //                 else
    //                 {
    //                     _foldout = EditorGUI.Foldout(foldoutRect, _foldout, new GUIContent("组件预览面板"), true);
    //                 }
    //             };
    //
    //             _listDrawer.DrawElementCallback += (rect, index, active, focused) =>
    //             {
    //                 rect.y += 2;
    //                 _editor.DrawElement(rect, Components[index], index);
    //             };
    //
    //             _listDrawer.OnReorderDecide += (index, newIndex) =>
    //             {
    //                 if (index == 0)
    //                 {
    //                     return false;
    //                 }
    //
    //                 if (Components[index] == null)
    //                 {
    //                     return false;
    //                 }
    //
    //                 return true;
    //             };
    //
    //             _listDrawer.OnReorderCallback += () =>
    //             {
    //                 //Move Down
    //                 for (int i = 0; i < Components.Count; i++)
    //                 {
    //                     int indexOf = _listDrawer.List.IndexOf(Components[i]);
    //                     int difference = indexOf - i;
    //                     if (difference <= 0) continue;
    //
    //                     if (i == 0)
    //                     {
    //                         break;
    //                     }
    //
    //                     for (int j = 0; j < Mathf.Abs(difference); j++)
    //                     {
    //                         ComponentUtility.MoveComponentDown(Components[i]);
    //                     }
    //                 }
    //
    //                 //Move Up
    //                 Components = GetComponents();
    //                 for (int i = Components.Count - 1; i >= 0; i--)
    //                 {
    //                     int indexOf = _listDrawer.List.IndexOf(Components[i]);
    //                     int difference = indexOf - i;
    //                     if (difference >= 0) continue;
    //
    //                     if (i == 0)
    //                     {
    //                         break;
    //                     }
    //
    //                     for (int j = 0; j < Mathf.Abs(difference); j++)
    //                     {
    //                         ComponentUtility.MoveComponentUp(Components[i]);
    //                     }
    //                 }
    //             };
    //
    //             _listDrawer.OnCanRemoveCallback += index =>
    //             {
    //                 var component = (Component)_listDrawer.List[index];
    //                 return IsRemovableComponent(component);
    //             };
    //
    //             _listDrawer.OnRemoveCallback += index =>
    //             {
    //                 var component = (Component)_listDrawer.List[index];
    //                 var targetComponents = _editor.GetTargetComponents(component);
    //                 foreach (var t in targetComponents)
    //                 {
    //                     if (IsDependantComponent(t, out var dependantTarget))
    //                     {
    //                         EditorUtility.DisplayDialog("错误",
    //                             $"不能删除\"{t.GetType()}\"因为\"{dependantTarget.GetType()}\"依赖于它",
    //                             "确认");
    //                     }
    //                     else
    //                     {
    //                         Undo.SetCurrentGroupName("Remove " + t.GetType().Name);
    //                         Undo.DestroyObjectImmediate(t);
    //                     }
    //                 }
    //
    //                 GUIUtility.ExitGUI();
    //             };
    //         }
    //
    //         public void DrawList()
    //         {
    //             Initialize();
    //             _listDrawer.HasListElementTopPadding = false;
    //             _listDrawer.DisplayElements = _foldout;
    //             _listDrawer.DoLayoutList();
    //         }
    //     }
    //
    //     private GameObject _gameObject;
    //     private List<TargetItem> _targetItems;
    //     private bool _expandTargets;
    //     
    //
    //     private void DrawElement(Rect rect, Component component, int index)
    //     {
    //         if (component == null)
    //         {
    //             rect.x += 14;
    //             var iconRect = new Rect(rect);
    //             iconRect.width = iconRect.height = BlockWidth;
    //
    //             EditorGUI.LabelField(iconRect, EasyEditorIcons.Warn);
    //             
    //             rect.x += (BlockWidth + 1) * 2;
    //             rect.y -= 2;
    //             //Name Handler
    //             GUIStyle guiStyle = new GUIStyle(EditorStyles.boldLabel);
    //             guiStyle.normal.textColor = guiStyle.onNormal.textColor = new Color32(209, 137, 24, 255);
    //             EditorGUI.LabelField(rect, "Mono脚本丢失！", guiStyle);
    //             return;
    //         }
    //
    //         //Is the components common for every inspected GameObject?
    //
    //         var foldoutRect = new Rect(rect);
    //         foldoutRect.x += 14;
    //         foldoutRect.width -= 14;
    //         foldoutRect.y -= 2;
    //
    //         var compIconRect = new Rect(rect);
    //         compIconRect.x += 14;
    //         compIconRect.width = compIconRect.height = BlockWidth;
    //
    //         var enableToggleRect = new Rect(compIconRect);
    //         enableToggleRect.x += BlockWidth + 1;
    //
    //         var componentLabelRect = new Rect(enableToggleRect)
    //         {
    //             width = rect.width - 125
    //         };
    //         componentLabelRect.x += BlockWidth + 1;
    //
    //         var editBtnRect = new Rect(rect)
    //         {
    //             x = rect.xMax
    //         };
    //         editBtnRect.width = editBtnRect.height = BlockWidth;
    //         editBtnRect.x -= EditorGUIUtility.singleLineHeight;
    //         editBtnRect.x -= EditorGUIUtility.singleLineHeight - 1;
    //
    //         bool isCommon = IsCommonComponentInTargets(component);
    //
    //         // 先绘制Enable开关
    //         if (EditorUtility.GetObjectEnabled(component) != -1)
    //         {
    //             bool oldValue = EditorUtility.GetObjectEnabled(component) == 1;
    //             bool newValue = EditorGUI.Toggle(enableToggleRect, oldValue);
    //             if (oldValue != newValue)
    //             {
    //                 var targetComponents = GetTargetComponents(component);
    //                 foreach (var t in targetComponents)
    //                 {
    //                     Undo.RecordObject(t, (newValue ? "Enable " : "Disable ") + t.GetType().Name);
    //                     EditorUtility.SetObjectEnabled(t, newValue);
    //                 }
    //             }
    //         }
    //
    //         if (TryGetComponentScript(component, out var script))
    //         {
    //             string path = AssetDatabase.GetAssetPath(script);
    //
    //             if (path.EndsWith(".cs") || path.EndsWith(".js"))
    //             {
    //                 if (GUI.Button(editBtnRect, new GUIContent(EasyEditorIcons.Edit.image, "编辑脚本"), Styles.Footer))
    //                 {
    //                     if (Event.current.button == 0)
    //                         AssetDatabase.OpenAsset(script);
    //                     else
    //                         EditorGUIUtility.PingObject(script);
    //                 }
    //             }
    //         }
    //
    //         using (var check = new EditorGUI.ChangeCheckScope())
    //         {
    //             var expand = EditorGUI.Foldout(foldoutRect, InternalEditorUtility.GetIsInspectorExpanded(component),
    //                 GUIContent.none, true);
    //             InternalEditorUtility.SetIsInspectorExpanded(component, expand);
    //             if (check.changed)
    //             {
    //                 bool value = InternalEditorUtility.GetIsInspectorExpanded(component);
    //                 var targetComponents = GetTargetComponents(component, false);
    //                 foreach (var c in targetComponents)
    //                     InternalEditorUtility.SetIsInspectorExpanded(c, value);
    //
    //                 UniEditorUtility.ForceRebuildInspectors();
    //             }
    //         }
    //
    //         Texture icon = EditorGUIUtility.ObjectContent(component, component.GetType()).image;
    //         EditorGUI.LabelField(compIconRect, new GUIContent(icon));
    //
    //         GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
    //         if (isCommon)
    //             style.normal.textColor = style.onNormal.textColor = new Color(0f, 1f, 0.3f);
    //         EditorGUI.LabelField(componentLabelRect, component.GetType().Name, style);
    //
    //         // //Reset Button
    //         // rect.x -= 18;
    //         // rect.width = 20;
    //         // rect.height += 4;
    //         // EditorGUI.LabelField(rect, new GUIContent(IconManager.ResetIcon.image, "Reset Component"));
    //         // if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
    //         // {
    //         // 	var targetComponents = GetTargetComponents(component);
    //         // 	foreach (var t in targetComponents)
    //         // 	{
    //         // 		Undo.RecordObject(t, "Reset " + t.GetType().Name);
    //         // 		Unsupported.SmartReset(t);
    //         // 	}
    //         // }
    //         //
    //         // rect.height -= 4;
    //         //
    //         // //Copy Button
    //         // rect.x -= 17;
    //         // rect.height += 1;
    //         // EditorGUI.LabelField(rect, new GUIContent(IconManager.DocumentIcon.image));
    //         // rect.x += 2;
    //         // rect.y += 2;
    //         // EditorGUI.LabelField(rect, new GUIContent(IconManager.DocumentIcon.image, "Copy Component"));
    //         // rect.height -= 1;
    //         //
    //         // //Inspect
    //         // rect.x -= 18;
    //         // rect.y -= 2;
    //
    //         //
    //         // rect.y += 2;
    //         //
    //         // //Paste Values Button
    //         // rect.height += 1;
    //         // rect.x -= 16;
    //         // if (CopiedComponent != null && CopiedComponent.GetType() == component.GetType())
    //         // {
    //         // 	rect.y -= 3;
    //         // 	EditorGUI.LabelField(rect, new GUIContent(IconManager.FolderIcon));
    //         // 	rect.x += 2;
    //         // 	rect.y += 3;
    //         // 	EditorGUI.LabelField(rect, new GUIContent(IconManager.DocumentIcon.image, "Paste Component Values"));
    //         // 	if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
    //         // 	{
    //         // 		var targetComponents = GetTargetComponents(component);
    //         // 		foreach (var t in targetComponents)
    //         // 			ComponentUtility.PasteComponentValues(t);
    //         // 	}
    //         //
    //         // 	rect.x -= 2;
    //         // }
    //     }
    //
    //     private List<Component> GetTargetComponents(Component component, bool includeSelf = true)
    //     {
    //         var total = new List<Component>();
    //
    //         if (includeSelf)
    //         {
    //             total.Add(component);
    //         }
    //
    //         if (component != null)
    //         {
    //             // 获取自己在GameObject中重复项的索引
    //             var ct = component.GetType();
    //             var ci = component.GetComponents(ct).IndexOf(component);
    //
    //             foreach (var comps in _targetItems.Where(i => i.Target != component.gameObject).Select(i => i.Components))
    //             {
    //                 foreach (var c in comps)
    //                 {
    //                     if (c == null) continue;
    //                     var ct1 = c.GetType();
    //                     // 如果类型相同。再判断他们重复项索引是否相同
    //                     if (ct1 == ct)
    //                     {
    //                         // 获取重复项索引，如果要对比的和当前的都相同，说明是匹配项
    //                         var ci2 = c.GetComponents(ct1).IndexOf(c);
    //                         if (ci == ci2)
    //                         {
    //                             total.Add(c);
    //                         }
    //                     }
    //                 }
    //             }
    //         }
    //
    //         return total;
    //     }
    //
    //     private bool IsRepeatedComponent(Component component)
    //     {
    //         var comps = component.gameObject.GetComponents<Component>();
    //         if (comps.Length > 1)
    //         {
    //             var i = comps.IndexOf(component);
    //             return i != 0;
    //         }
    //         return false;
    //     }
    //
    //     private bool TryGetComponentScript(Component component, out MonoScript script)
    //     {
    //         if (_allScripts.IsNullOrEmpty())
    //         {
    //             _allScripts = MonoImporter.GetAllRuntimeMonoScripts();
    //         }
    //
    //         try
    //         {
    //             script = _allScripts.FirstOrDefault(s => s.GetClass() == component.GetType());
    //             return script != null;
    //         }
    //         catch (Exception e)
    //         {
    //             _allScripts = MonoImporter.GetAllRuntimeMonoScripts();
    //             script = _allScripts.FirstOrDefault(s => s.GetClass() == component.GetType());
    //             return script != null;
    //         }
    //     }
    //
    //     private bool IsCommonComponentInTargets(Component component)
    //     {
    //         return GetTargetComponents(component, false).Count > 0;
    //     }
    //
    //     private static bool IsVisibleComponent(Component component)
    //     {
    //         if (component == null) return true;
    //         var t = component.GetType();
    //         if (t == typeof(ParticleSystemRenderer))
    //         {
    //             return false;
    //         }
    //
    //         return true;
    //     }
    //
    //     private static bool IsDependantComponent(Component component, out Component dependantTarget)
    //     {
    //         dependantTarget = null;
    //         if (component == null) return false;
    //
    //         var components = component.GetComponents<Component>();
    //         var t = component.GetType();
    //         foreach (var c in components)
    //         {
    //             if (c == null) continue;
    //             var attrs = c.GetType().GetCustomAttributes<RequireComponent>();
    //             if (attrs.Any(attr => attr.m_Type0 == t || attr.m_Type1 == t || attr.m_Type2 == t))
    //             {
    //                 dependantTarget = c;
    //                 return true;
    //             }
    //         }
    //
    //         return false;
    //     }
    //
    //     private static bool IsRemovableComponent(Component component)
    //     {
    //         if (component == null) return true;
    //
    //         var t = component.GetType();
    //         if (t == typeof(Transform) || t == typeof(RectTransform))
    //         {
    //             return false;
    //         }
    //
    //         return true;
    //     }
    //
    //     #endregion
    }
}
