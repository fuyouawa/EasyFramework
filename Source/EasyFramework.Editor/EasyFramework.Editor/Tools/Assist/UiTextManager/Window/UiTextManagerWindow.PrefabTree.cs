using EasyFramework.Inspector;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Editor
{
    public partial class UiTextManagerWindow
    {
        [Serializable]
        public abstract class PrefabTreeNodeBase
        {
            public string FullName { get; private set; }
            public string Path { get; private set; }
            public PrefabItemBase Owner { get; }

            public GameObject Prefab { get; private set; }

            public TextMeshProUGUI Text { get; private set; }
            public PrefabTreeNodeBase Parent { get; private set; }
            public GameObject Target { get; private set; }
            public List<PrefabTreeNodeBase> Children { get; } = new List<PrefabTreeNodeBase>();
            public TreeNodeState State { get; } = new TreeNodeState();

            protected PrefabTreeNodeBase(PrefabItemBase owner, GameObject prefab, string targetName,
                PrefabTreeNodeBase parent = null)
            {
                Owner = owner;
                Parent = parent;
                UpdatePrefab(prefab, targetName);
            }

            public void UpdatePrefab(GameObject prefab)
            {
                UpdatePrefab(prefab, Target.name);
            }

            private void UpdatePrefab(GameObject prefab, string targetName)
            {
                Prefab = prefab;
                if (Parent == null)
                {
                    FullName = targetName;
                    Path = string.Empty;
                    Target = prefab;
                }
                else
                {
                    FullName = Parent.FullName + '/' + targetName;
                    if (Parent.Path.IsNotNullOrEmpty())
                    {
                        Path = Parent.Path + '/' + targetName;
                    }
                    else
                    {
                        Path = targetName;
                    }

                    foreach (Transform child in Parent.Target.transform)
                    {
                        if (child.gameObject.name == targetName)
                        {
                            Target = child.gameObject;
                            break;
                        }
                    }
                }

                Text = Target.GetComponent<TextMeshProUGUI>();
            }

            public bool IsTextNode()
            {
                return Text != null;
            }

            public bool HasIncorrect()
            {
                Debug.Assert(IsTextNode());
                return UiTextManagerHelper.GameObjectHasIncorrect(Target.gameObject);
            }

            public bool HasIncorrectRecursion()
            {
                if (!IsTextNode())
                {
                    return false;
                }

                if (HasIncorrect())
                    return true;
                foreach (var child in Children)
                {
                    if (child.HasIncorrectRecursion())
                        return true;
                }

                return false;
            }

            public abstract void Select();
        }

        public class PrefabAssetTreeNode : PrefabTreeNodeBase
        {
            public PrefabAssetTreeNode(PrefabItemBase owner, GameObject prefab, string targetName, PrefabTreeNodeBase parent = null) : base(owner, prefab, targetName, parent)
            {
            }

            public override void Select()
            {
                Owner.OpenPrefab();
                Selection.activeObject = Target;
                EditorGUIUtility.PingObject(Target);
            }
        }

        public class PrefabSceneTreeNode : PrefabTreeNodeBase
        {
            public PrefabSceneTreeNode(PrefabItemBase owner, GameObject prefab, string targetName, PrefabTreeNodeBase parent = null) : base(owner, prefab, targetName, parent)
            {
            }

            public override void Select()
            {
                Selection.activeObject = Target;
                EditorGUIUtility.PingObject(Target);
            }
        }


        [Serializable]
        public class PrefabTree : List<PrefabTreeNodeBase>
        {
        }

        public class PrefabTreeDrawer : TreeValueDrawer<PrefabTree, PrefabTreeNodeBase>
        {
            public override GUIContent GetNodeLabel(PrefabTreeNodeBase node)
            {
                return new GUIContent("       " + node.Target.name);
            }

            public override IList<PrefabTreeNodeBase> GetNodeChildren(PrefabTreeNodeBase node)
            {
                return node.Children;
            }

            public override TreeNodeState GetNodeState(PrefabTreeNodeBase node)
            {
                node.State.Expandable = true;
                return node.State;
            }

            protected override void OnNodeCoveredTitleBarGUI(PrefabTreeNodeBase node, Rect headerRect,
                TreeNodeInfo info)
            {
                var iconWidth = EditorGUIUtility.singleLineHeight;
                if (node.IsTextNode() && node.HasIncorrectRecursion())
                {
                    var iconRect = headerRect.AlignRight(iconWidth).SetSize(Vector2.one * iconWidth);
                    var icon = EditorIcons.UnityWarningIcon;
                    EditorGUI.LabelField(iconRect, new GUIContent(icon));
                }

                if (node.Text != null)
                {
                    var iconRect = headerRect.AddX(14f).SetSize(Vector2.one * iconWidth);
                    var icon = EditorGUIUtility.ObjectContent(node.Text, typeof(TextMeshProUGUI)).image;
                    EditorGUI.LabelField(iconRect, new GUIContent(icon));
                }
            }

            protected override void OnBeforeChildrenContentGUI(PrefabTreeNodeBase node, Rect headerRect,
                TreeNodeInfo info)
            {
                bool hasIncorrect = false;
                if (node.IsTextNode())
                {
                    var mgrs = node.Target.GetComponents<UiTextManager>();
                    if (CollectionExtension.IsNullOrEmpty(mgrs))
                    {
                        EasyEditorGUI.MessageBox("未被UiTextManager管理", MessageType.Warning);
                        hasIncorrect = true;
                    }
                    else
                    {
                        if (mgrs.Length > 1)
                        {
                            EasyEditorGUI.MessageBox("被多个UiTextManager管理，这是无效的", MessageType.Warning);
                            hasIncorrect = true;
                        }
                        else
                        {
                            var mgr = mgrs[0];
                            if (mgr.FontAssetPresetId.IsNullOrEmpty())
                            {
                                EasyEditorGUI.MessageBox("字体资产预设为空！", MessageType.Warning);
                                hasIncorrect = true;
                            }
                            else
                            {
                                EasyEditorGUI.MessageBox(
                                    $"字体资产预设为：{mgr.FontAssetPresetId.DefaultIfNullOrEmpty("TODO")}",
                                    MessageType.Info);
                            }

                            if (mgr.TextPropertiesPresetId.IsNullOrEmpty())
                            {
                                EasyEditorGUI.MessageBox("文本属性预设为空！", MessageType.Warning);
                                hasIncorrect = true;
                            }
                            else
                            {
                                EasyEditorGUI.MessageBox(
                                    $"文本属性预设为：{mgr.TextPropertiesPresetId.DefaultIfNullOrEmpty("TODO")}",
                                    MessageType.Info);
                            }
                        }
                    }

                    var btnRect = EasyEditorGUI.GetIndentControlRect(false);
                    if (SirenixEditorGUI.SDFIconButton(btnRect, new GUIContent("选中该物体"), Color.yellow.SetA(0.5f),
                            Color.white, SdfIconType.HandIndexThumbFill))
                    {
                        node.Select();
                    }

                    if (hasIncorrect)
                    {
                        btnRect = EasyEditorGUI.GetIndentControlRect(false);
                        if (SirenixEditorGUI.SDFIconButton(btnRect, new GUIContent("自动处理"), Color.cyan.SetA(0.5f),
                                Color.white, SdfIconType.Tools))
                        {
                            node.Select();
                            node.Owner.AutoHandleNodeIncorrect(node);
                        }
                    }
                }
            }
        }
    }
}
