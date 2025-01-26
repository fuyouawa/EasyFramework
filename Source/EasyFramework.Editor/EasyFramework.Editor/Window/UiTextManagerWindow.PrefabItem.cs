using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities.Editor;
using System.Collections.Generic;
using System.Linq;
using System;
using EasyFramework.Editor.Inspector;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Editor.Window
{
    public partial class UiTextManagerWindow
    {
        [Serializable]
        public abstract class PrefabItemBase
        {
            [ReadOnly]
            [LabelText("路径")]
            [PropertyOrder(0)]
            public string PrefabPath;

            [Button("打开预制体", Icon = SdfIconType.Folder2Open)]
            [PropertyOrder(1)]
            public abstract void OpenPrefab();

            [FoldoutGroup("自动分析")]
            [InfoBoxCN("1、如果没有UiTextManager组件会自动添加\n" +
                       "2、如果\"字体资产预设\"是null，根据Text(UI)组件的字体和材质设置，判断属于\"预设管理器\"中的哪个字体资产预设，然后自动赋值\n" +
                       "3、如果\"文本属性预设\"是null，根据Text(UI)组件的字体大小和字体颜色设置，判断属于\"预设管理器\"中的哪个文本属性预设，然后自动赋值\n" +
                       "4、如果\"字体资产预设\"依然是null，会使用\"预设管理器\"中定义的\"默认字体资产预设\"（如果有）\n" +
                       "5、如果\"文本属性预设\"依然是null，会使用\"预设管理器\"中定义的\"默认文本属性预设\"（如果有）\n" +
                       "6、如果组件上有多个重复的UITextManager，则只保留第一个，删除其他重复项")]
            [Button("自动分析处理", Icon = SdfIconType.Tools)]
            [PropertyOrder(6)]
            private void AutoHandleIncorrect()
            {
                var msg = "您确定要使用自动处理吗";
                if (EditorUtility.DisplayDialog("警告", msg, "确认", "取消"))
                {
                    OpenPrefab();
                    AutoHandleIncorrectRecursion();
                }
            }

            public void AutoHandleIncorrectRecursion()
            {
                foreach (var node in Tree)
                {
                    if (node.IsTextNode())
                    {
                        AutoHandleNodeIncorrect(node);
                    }

                    AutoHandleIncorrectRecursion(node.Children);
                }
            }

            public void AutoHandleIncorrectRecursion(List<PrefabTreeNodeBase> children)
            {
                foreach (var child in children)
                {
                    if (child.IsTextNode())
                    {
                        AutoHandleNodeIncorrect(child);
                    }

                    AutoHandleIncorrectRecursion(child.Children);
                }
            }

            public void AutoHandleNodeIncorrect(PrefabTreeNodeBase node)
            {
                Debug.Assert(node.IsTextNode());

                UiTextManager mgr;

                var mgrs = node.Target.GetComponents<UiTextManager>();
                if (mgrs.Length >= 1)
                {
                    mgr = mgrs[0];
                    if (mgrs.Length > 1)
                    {
                        foreach (var m in mgrs.Skip(1))
                        {
                            GameObject.DestroyImmediate(m);
                        }
                    }
                }
                else
                {
                    mgr = node.Target.AddComponent<UiTextManager>();
                }

                // 根据UI文本组件的字体和材质设置, 判断属于"预设管理器"中的哪个字体资产预设, 然后自动赋值
                if (mgr.FontAssetPresetId.IsNullOrEmpty())
                {
                    foreach (var preset in UiTextPresetsSettings.Instance.FontAssetPresets)
                    {
                        if (preset.Value.FontAsset == node.Text.font && preset.Value.Material == node.Text.fontSharedMaterial)
                        {
                            mgr.FontAssetPresetId = preset.Key;
                            break;
                        }
                    }
                }

                if (mgr.TextPropertiesPresetId.IsNullOrEmpty())
                {
                    foreach (var preset in UiTextPresetsSettings.Instance.TextPropertiesPresets)
                    {
                        if (preset.Value.FontSize.Approximately(node.Text.fontSize) && preset.Value.FontColor == node.Text.color)
                        {
                            mgr.TextPropertiesPresetId = preset.Key;
                            break;
                        }
                    }
                }

                // 如果UITextManager的"字体资产预设"依然是null, 会使用"预设管理器"中定义的"默认字体资产预设"
                if (mgr.FontAssetPresetId.IsNullOrEmpty())
                {
                    mgr.FontAssetPresetId = UiTextPresetsSettings.Instance.DefaultFontAssetPresetId;
                }

                if (mgr.TextPropertiesPresetId.IsNullOrEmpty())
                {
                    mgr.TextPropertiesPresetId = UiTextPresetsSettings.Instance.DefaultTextPropertiesPresetId;
                }

                AssetDatabase.Refresh();
            }

            [EnumToggleButtons]
            [TitleGroupCN("UI文本组件实例视图")]
            [HideLabel]
            [PropertyOrder(7)]
            [TitleGroup("UI文本组件实例视图/显示模式", boldTitle: false)]
            public ItemsViewModes ViewMode = ItemsViewModes.All;

            [TitleGroupCN("UI文本组件实例视图")]
            [LabelText("实例列表")]
            [PropertyOrder(8)]
            [OdinSerialize]
            public PrefabTree Tree = new PrefabTree();


            public abstract GameObject GetPrefab();

            protected PrefabItemBase(string prefabPath)
            {
                PrefabPath = prefabPath;
            }

            [Flags]
            public enum ItemsViewModes
            {
                Correct = 1,
                Incorrect = 1 << 1,
                Disable = 1 << 2,
                All = Correct | Incorrect | Disable,
            }

            private List<string> _prefabPaths = new List<string>();

            public void Analyse()
            {
                ClearCache();
                var prefab = GetPrefab();
                foreach (var text in prefab.GetComponentsInChildren<TextMeshProUGUI>(true))
                {
                    if (!PrefabFilter(text.transform.gameObject))
                        continue;

                    var path = text.transform.gameObject.transform.GetRelativePath(prefab.transform, true);
                    if (path[0] == '/')
                        path = path[1..];

                    var split = path.Split('/');

                    var s0 = split[0];
                    var node = Tree.Find(n => n.Target.name == s0);
                    if (node == null)
                    {
                        node = GetPrefabTreeNode(prefab, s0, null);
                        Tree.Add(node);
                    }

                    for (int i = 1; i < split.Length; i++)
                    {
                        var s = split[i];
                        var node2 = node.Children.Find(n => n.Target.name == s);
                        if (node2 == null)
                        {
                            node2 = GetPrefabTreeNode(prefab, s, node);
                            node.Children.Add(node2);
                        }

                        node = node2;
                    }
                }
            }

            private void UpdatePrefabRecursion(GameObject prefab, List<PrefabTreeNodeBase> children)
            {
                if (children.IsNullOrEmpty())
                    return;

                foreach (var child in children)
                {
                    child.UpdatePrefab(prefab);
                    UpdatePrefabRecursion(prefab, child.Children);
                }
            }

            public void UpdatePrefabRecursion(GameObject prefab)
            {
                foreach (var node in Tree)
                {
                    node.UpdatePrefab(prefab);
                    UpdatePrefabRecursion(prefab, node.Children);
                }
            }

            private PrefabTreeNodeBase GetPrefabTreeNode(GameObject prefab, string name, PrefabTreeNodeBase parent)
            {
                if (parent == null)
                {
                    return AllocTreeNode(prefab, prefab, null);
                }

                foreach (Transform child in parent.Target.transform)
                {
                    if (child.gameObject.name == name)
                    {
                        return AllocTreeNode(prefab, child.gameObject, parent);
                    }
                }

                throw new Exception($"找不到{parent.FullName}/{name}，尝试“重新加载资源视图”");
            }

            protected abstract PrefabTreeNodeBase AllocTreeNode(GameObject prefab, GameObject target,
                PrefabTreeNodeBase parent);

            private bool PrefabFilter(GameObject prefab)
            {
                if (ViewMode == 0)
                    return false;
                if (ViewMode == ItemsViewModes.All)
                    return true;

                var warn = UiTextManagerHelper.GameObjectHasIncorrect(prefab);

                if (warn && ViewMode.HasFlag(ItemsViewModes.Incorrect))
                {
                    return true;
                }

                if (!prefab.activeInHierarchy && ViewMode.HasFlag(ItemsViewModes.Disable))
                {
                    return true;
                }

                if (!warn && ViewMode.HasFlag(ItemsViewModes.Correct))
                {
                    return true;
                }

                return false;
            }

            public void ClearCache()
            {
                Tree.Clear();
                _prefabPaths.Clear();
            }

            public bool HasIncorrect()
            {
                foreach (var node in Tree)
                {
                    if (node.HasIncorrectRecursion())
                    {
                        return true;
                    }
                }

                return false;
            }

            private void OnTextItemsTitleBarGUI()
            {
                if (SirenixEditorGUI.ToolbarButton(EditorIcons.Refresh))
                {
                    Analyse();
                }

                if (SirenixEditorGUI.ToolbarButton(SdfIconType.Question))
                {
                    //TODO 提示
                    EditorUtility.DisplayDialog("提示", "1.如果有关于UI文本组件实例列表的更改, 需要点一下旁边的重置按钮重新分析\n" +
                                                      "2.还没想好", "确认");
                }
            }
        }

        public class PrefabAssetItem : PrefabItemBase
        {
            private GameObject _prefab;

            public PrefabAssetItem(string prefabPath) : base(prefabPath)
            {
            }

            public override void OpenPrefab()
            {
                var ps = PrefabStageUtility.GetCurrentPrefabStage();
                if (ps != null && ps.assetPath == PrefabPath)
                    return;

                ps = PrefabStageUtility.OpenPrefab(PrefabPath);
                UpdatePrefabRecursion(ps.prefabContentsRoot);
            }

            public override GameObject GetPrefab()
            {
                if (_prefab == null)
                {
                    _prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
                }

                return _prefab;
            }

            protected override PrefabTreeNodeBase AllocTreeNode(GameObject prefab, GameObject target,
                PrefabTreeNodeBase parent)
            {
                var ps = PrefabStageUtility.GetCurrentPrefabStage();
                if (ps != null && ps.assetPath == PrefabPath)
                {
                    prefab = ps.prefabContentsRoot;
                }

                return new PrefabAssetTreeNode(this, prefab, target.name, parent);
            }
        }

        public class PrefabSceneItem : PrefabItemBase
        {
            private GameObject _root;

            public PrefabSceneItem(GameObject root, string prefabPath) : base(prefabPath)
            {
                _root = root;
            }

            public override void OpenPrefab()
            {
                Selection.activeObject = _root;
                EditorGUIUtility.PingObject(_root);
            }

            public override GameObject GetPrefab()
            {
                return _root;
            }

            protected override PrefabTreeNodeBase AllocTreeNode(GameObject prefab, GameObject target,
                PrefabTreeNodeBase parent)
            {
                return new PrefabSceneTreeNode(this, prefab, target.name, parent);
            }
        }
    }
}
