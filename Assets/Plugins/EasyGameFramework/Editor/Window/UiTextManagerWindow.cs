using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using TMPro;
using EasyFramework;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using JetBrains.Annotations;
using Pokemon.UI;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using Sirenix.Serialization;

namespace EasyGameFramework.Editor
{
    public class UiTextManagerWindow : OdinMenuEditorWindow
    {
        public class WindowTemp
        {
            public Rect WindowPosition;
            public float MenuWidth;
        }

        private static UiTextManagerWindow _instance;

        public static UiTextManagerWindow Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.Assert(HasOpenInstances<UiTextManagerWindow>());
                    _instance = GetWindow<UiTextManagerWindow>("UiTextManager Window");
                }

                Debug.Assert(_instance != null);
                return _instance;
            }
        }

        [MenuItem("Tools/EasyGameFramework/UiTextManager Window")]
        public static void ShowWindow()
        {
            var isNew = HasOpenInstances<UiTextManagerWindow>();
            var window = GetWindow<UiTextManagerWindow>("UiTextManager Window");
            if (!isNew)
            {
                if (!File.Exists(EditorAssetsPath.UiTextManagerWindowTempPath))
                {
                    window.CenterWindowWithSizeRadio(new Vector2(0.4f, 0.45f));
                }
            }

            _instance = window;
        }

        private Settings _settings = new();
        private PrefabAssetItem _prevSelectionItem;

        public void Rebuild()
        {
            ForceMenuTreeRebuild();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var assetsPath = $"Assets/{_settings.AssetsManagerPath}/";
            var prefix = "资产视图";
            var tree = new OdinMenuTree()
            {
                { "设置", _settings, SdfIconType.GearFill },
                { prefix, null }
            };
            tree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;

            if (_settings.AssetsManagerPath.IsNotNullOrEmpty())
            {
                var allPrefabs = AssetDatabase.GetAllAssetPaths()
                    .Where(p => p.StartsWith(assetsPath) &&
                                p.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase));
                foreach (var path in allPrefabs)
                {
                    var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (obj != null && obj.GetComponentInChildren<TextMeshProUGUI>() != null)
                    {
                        var item = new PrefabAssetItem(path);

                        item.Analyse();
                        if (AssetItemFilter(item))
                        {
                            Texture2D icon;
                            if (item.HasWarn())
                            {
                                icon = EditorIcons.UnityWarningIcon;
                            }
                            else
                            {
                                icon = InternalEditorUtility.GetIconForFile(path);
                            }

                            var p = path.Substring(assetsPath.Length,
                                path.Length - assetsPath.Length - ".prefab".Length);
                            tree.Add($"{prefix}/{p}", item, icon);
                        }

                        item.ClearCache();
                    }
                }

                var items = tree.MenuItems.First(i => i.Name == prefix).GetChildMenuItemsRecursive(false);
                foreach (var item in items)
                {
                    if (item.Value == null)
                    {
                        item.Icon = EditorIcons.UnityFolderIcon;
                    }
                }
            }

            tree.Selection.SupportsMultiSelect = false;
            tree.Selection.SelectionChanged += type =>
            {
                if (type == SelectionChangedType.ItemAdded)
                {
                    if (tree.Selection.SelectedValue is PrefabAssetItem item)
                    {
                        _prevSelectionItem?.ClearCache();
                        _prevSelectionItem = item;
                        item.Analyse();
                        if (_settings.AutoOpenSelectionPrefab)
                        {
                            item.OpenPrefab();
                        }
                    }
                }
            };
            return tree;
        }


        private bool AssetItemFilter(PrefabAssetItem item)
        {
            if (_settings == null)
                return true;
            if (_settings.ViewModes == 0)
                return false;
            if (_settings.ViewModes == ViewModes.All)
                return true;

            var warn = item.HasWarn();

            if (warn && _settings.ViewModes.HasFlag(ViewModes.Warn))
            {
                return true;
            }

            if (!warn && _settings.ViewModes.HasFlag(ViewModes.Correct))
            {
                return true;
            }

            return false;
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            EditorApplication.update += EditorUpdate;

            Load();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            EditorApplication.update -= EditorUpdate;
            Save();
        }

        private double _prevAutoSaveTime;

        private void EditorUpdate()
        {
            if (EditorApplication.timeSinceStartup - _prevAutoSaveTime >= _settings.AutoSaveInterval)
            {
                Save();
                _prevAutoSaveTime = EditorApplication.timeSinceStartup;
            }
        }

        public void Save()
        {
            var json = EasyJsonConvert.SerializeObject(_settings, Formatting.Indented);
            File.WriteAllText(EditorAssetsPath.UiTextManagerSettingsPath, json);

            var temp = new WindowTemp
            {
                WindowPosition = position,
                MenuWidth = MenuWidth
            };
            json = EasyJsonConvert.SerializeObject(temp, Formatting.Indented);
            File.WriteAllText(EditorAssetsPath.UiTextManagerWindowTempPath, json);
        }

        public void Load()
        {
            if (!File.Exists(EditorAssetsPath.UiTextManagerSettingsPath))
                return;
            var json = File.ReadAllText(EditorAssetsPath.UiTextManagerSettingsPath);
            var val = EasyJsonConvert.DeserializeObject<Settings>(json);
            if (val != null)
            {
                _settings = val;
            }

            if (!File.Exists(EditorAssetsPath.UiTextManagerWindowTempPath))
                return;
            json = File.ReadAllText(EditorAssetsPath.UiTextManagerWindowTempPath);
            var temp = EasyJsonConvert.DeserializeObject<WindowTemp>(json);
            if (temp != null)
            {
                position = temp.WindowPosition;
                MenuWidth = temp.MenuWidth;
            }
        }

        #region Settings

        [Flags]
        public enum ViewModes
        {
            Correct = 1,
            Warn = 1 << 2,
            All = Correct | Warn,
        }

        public enum ManagerPositions
        {
            InScene,
            InProject
        }

        public class Settings
        {
            [TitleGroupCN("配置")]
            [LabelText("管理位置")]
            public ManagerPositions ManagerPosition = ManagerPositions.InProject;

            [TitleGroupCN("配置")]
            [FolderPath(ParentFolder = "Assets")]
            [ShowIf("ManagerPosition", ManagerPositions.InProject)]
            [LabelText("管理路径")]
            public string AssetsManagerPath = "Resources";

            [EnumToggleButtons]
            [TitleGroup("配置/显示模式", boldTitle: false)]
            [HideLabel]
            public ViewModes ViewModes = ViewModes.All;

            [TitleGroupCN("配置")]
            [InfoBoxCN("注意：资产视图中的预制体不会实时更新，可能会出现类似错误都解决了但是图标依然是错误，或者预制体删除了但依然在显示，需要再次“重新加载资源视图”",
                InfoMessageType.Warning)]
            [InfoBoxCN("任何资产配置的更改, 都需要点击“重新加载资源视图”才会实际应用")]
            [Button("重新加载资源视图")]
            [UsedImplicitly]
            private void Rebuild()
            {
                Instance.Rebuild();
            }


            [TitleGroupCN("视图配置")]
            [LabelText("自动打开选中预制体")]
            public bool AutoOpenSelectionPrefab = true;

            [TitleGroup("数据存储")]
            [InfoBoxCN("注意: 当编辑器重新编译的时候, 未保存的更改可能会丢失", InfoMessageType.Warning)]
            [LabelText("自动保存间隔(秒)")]
            public float AutoSaveInterval = 5f;

            [TitleGroup("数据存储")]
            [Button("保存编辑器设置")]
            [UsedImplicitly]
            private void Save()
            {
                Instance.Save();
                AssetDatabase.Refresh();
            }

            [TitleGroup("数据存储")]
            [Button("加载编辑器设置")]
            [UsedImplicitly]
            private void Load()
            {
                Instance.Load();
            }
        }

        #endregion

        #region PrefabAssetItem

        [Serializable]
        public class PrefabAssetItem
        {
            [ReadOnly]
            [LabelText("资产路径")]
            [PropertyOrder(0)]
            public string AssetPath;

            [Button("打开预制体", Icon = SdfIconType.Folder2Open)]
            [PropertyOrder(1)]
            public void OpenPrefab()
            {
                PrefabStageUtility.OpenPrefab(AssetPath);
            }

            [FoldoutGroup("自动分析")]
            [InfoBoxCN("如果没有UITextManager组件会自动添加")]
            [LabelText("条件1")]
            [PropertyOrder(2)]
            public bool Condition1 = true;

            [FoldoutGroup("自动分析")]
            [InfoBoxCN("如果UITextManager的\"字体资产预设\"依然null，根据UI文本组件的字体和材质设置，判断属于\"预设管理器\"中的哪个字体资产预设，然后自动赋值")]
            [LabelText("条件2")]
            [PropertyOrder(3)]
            public bool Condition2 = true;

            [FoldoutGroup("自动分析")]
            [InfoBoxCN("如果UITextManager的\"字体资产预设\"依然是null，会使用\"预设管理器\"中定义的\"默认字体资产预设\"")]
            [LabelText("条件3")]
            [PropertyOrder(4)]
            public bool Condition3 = true;

            [FoldoutGroup("自动分析")]
            [InfoBoxCN("如果组件上有多个重复的UITextManager，则只保留第一个，删除其他重复项")]
            [LabelText("条件4")]
            [PropertyOrder(5)]
            public bool Condition4 = true;

            [FoldoutGroup("自动分析")]
            [Button("自动分析处理", Icon = SdfIconType.Tools)]
            [UsedImplicitly]
            [PropertyOrder(6)]
            private void AutoHandleWarn()
            {
                var msg = "您确定要使用自动处理吗";
                if (EditorUtility.DisplayDialog("警告", msg, "确认", "取消"))
                {
                    OpenPrefab();
                    foreach (var node in Tree)
                    {
                        var mgr = node.Target.GetComponent<UiTextManager>();
                        if (mgr == null)
                        {
                            // 如果没有UITextManager组件会自动添加
                            if (Condition1)
                            {
                                mgr = node.Target.gameObject.AddComponent<UiTextManager>();
                            }
                            else
                            {
                                continue;
                            }
                        }

                        var text = node.Target.GetComponent<TextMeshProUGUI>();
                        // 根据UI文本组件的字体和材质设置, 判断属于"预设管理器"中的哪个字体资产预设, 然后自动赋值
                        if (Condition2 && mgr.FontAssetPreset == null)
                        {
                            for (int i = 0; i < UITextPresetsManager.Instance.FontAssetPresets.Count; i++)
                            {
                                var preset = UITextPresetsManager.Instance.FontAssetPresets[i];
                                if (preset.FontAsset == text.font && preset.Material == text.fontSharedMaterial)
                                {
                                    mgr.FontAssetPresetIndex = i;
                                }
                            }
                        }

                        // 如果UITextManager的"字体资产预设"依然是null, 会使用"预设管理器"中定义的"默认字体资产预设"
                        if (Condition3 && mgr.FontAssetPreset == null)
                        {
                            mgr.FontAssetPresetIndex = UITextPresetsManager.Instance.DefaultFontAssetPresetIndex;
                        }

                        if (Condition4)
                        {
                            var mgrs = node.Target.GetComponents<UiTextManager>();
                            if (mgrs.Length > 1)
                            {
                                foreach (var m in mgrs.Skip(1))
                                {
                                    GameObject.DestroyImmediate(m);
                                }
                            }

                            Debug.Assert(node.Target.GetComponents<UiTextManager>().Length <= 1);
                        }
                    }
                }
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
            public PrefabTree Tree = new();


            public GameObject Asset => AssetDatabase.LoadAssetAtPath<GameObject>(AssetPath);

            public PrefabAssetItem(string assetPath)
            {
                AssetPath = assetPath;
            }

            [Flags]
            public enum ItemsViewModes
            {
                Correct = 1,
                Warn = 1 << 1,
                Disable = 1 << 2,
                All = Correct | Warn | Disable,
            }

            private List<string> _prefabPaths = new();

            public void Analyse()
            {
                ClearCache();
                foreach (var text in Asset.GetComponentsInChildren<TextMeshProUGUI>(true))
                {
                    if (!PrefabFilter(text.gameObject))
                        continue;

                    var path = text.gameObject.transform.GetRelativePath(Asset.transform, true);
                    if (path[0] == '/')
                        path = path[1..];

                    var split = path.Split('/');

                    var s0 = split[0];
                    var node = Tree.Find(n => n.Name == s0);
                    if (node == null)
                    {
                        node = new PrefabTreeNode(s0, AssetPath);
                        Tree.Add(node);
                    }

                    for (int i = 1; i < split.Length; i++)
                    {
                        var s = split[i];
                        var node2 = node.Children.Find(n => n.Name == s);
                        if (node2 == null)
                        {
                            node2 = new PrefabTreeNode(s, AssetPath, node);
                            node.Children.Add(node2);
                        }

                        node = node2;
                    }
                }
            }

            private bool PrefabFilter(GameObject prefab)
            {
                if (ViewMode == 0)
                    return false;
                if (ViewMode == ItemsViewModes.All)
                    return true;

                var warn = PrefabHasWarn(prefab);

                if (warn && ViewMode.HasFlag(ItemsViewModes.Warn))
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

            public bool HasWarn()
            {
                foreach (var node in Tree)
                {
                    if (PrefabHasWarn(node.Prefab))
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

        public static bool PrefabHasWarn(GameObject obj)
        {
            return PrefabHasNotManager(obj) || PrefabHasRepeatedManager(obj);
        }

        public static bool PrefabHasNotManager(GameObject obj)
        {
            return obj.GetComponent<UiTextManager>() == null;
        }

        public static bool PrefabHasRepeatedManager(GameObject obj)
        {
            return obj.GetComponents<UiTextManager>().Length > 1;
        }

        #endregion

        #region PrefabAssetNode

        [Serializable]
        public class PrefabTreeNode
        {
            public string Name { get; }

            public string AssetPath { get; }
            public string FullName { get; }

            public GameObject Prefab { get; }

            public TextMeshProUGUI TextGUI { get; }
            public PrefabTreeNode Parent { get; }
            public Transform Target { get; }
            public List<PrefabTreeNode> Children { get; } = new();
            public EasyEditorGUI.TreeNodeState State { get; } = new();

            public PrefabTreeNode(string name, PrefabTreeNode parent = null)
                : this(name, string.Empty, parent)
            {
            }

            public PrefabTreeNode(string name, string assetPath, PrefabTreeNode parent = null)
            {
                Name = name;
                AssetPath = assetPath;
                Parent = parent;
                if (parent == null)
                {
                    Prefab = FindPrefab();
                    Target = Prefab.transform;
                    FullName = name;
                }
                else
                {
                    Prefab = parent.Prefab;
                    FullName = parent.FullName + '/' + name;
                    foreach (Transform child in parent.Target)
                    {
                        if (child.gameObject.name == name)
                        {
                            Target = child;
                            break;
                        }
                    }
                }

                Debug.Assert(Target != null);
                TextGUI = Target!.GetComponent<TextMeshProUGUI>();
            }

            private GameObject FindPrefab()
            {
                var ps = PrefabStageUtility.GetCurrentPrefabStage();
                if (ps != null && ps.assetPath == AssetPath)
                {
                    return ps.prefabContentsRoot;
                }

                return AssetDatabase.LoadAssetAtPath<GameObject>(AssetPath);
            }

            public void Select()
            {
                var ps = PrefabStageUtility.OpenPrefab(AssetPath);
                var target = ps.prefabContentsRoot.transform.Find(FullName);
                Debug.Assert(target != null);
                Selection.activeObject = target;
                EditorGUIUtility.PingObject(target);
            }

            public bool HasNotManager() => PrefabHasNotManager(Prefab);
            public bool HasRepeatedManager() => PrefabHasRepeatedManager(Prefab);
        }

        [Serializable]
        public class PrefabTree : List<PrefabTreeNode>
        {
        }

        public class PrefabTreeDrawer : TreeValueDrawer<PrefabTree, PrefabTreeNode>
        {
            public override string GetNodeLabel(PrefabTreeNode node)
            {
                return "       " + node.Name;
            }

            public override IList<PrefabTreeNode> GetNodeChildren(PrefabTreeNode node)
            {
                return node.Children;
            }

            public override EasyEditorGUI.TreeNodeState GetNodeState(PrefabTreeNode node)
            {
                return node.State;
            }

            protected override bool ChildrenHasBox => true;

            protected override void OnNodeCoveredTitleBarGUI(PrefabTreeNode node, Rect headerRect, EasyEditorGUI.TreeNodeInfo info)
            {
                var p = headerRect.position;
                if (!info.IsLastNode)
                {
                    p.x += 14;
                }
                DrawIcon(node, p);
            }

            private void DrawIcon(PrefabTreeNode node, Vector2 position)
            {
                if (node.TextGUI != null)
                {
                    var iconRect = new Rect(position, Vector2.one * EditorGUIUtility.singleLineHeight);
                    var icon = EditorGUIUtility.ObjectContent(node.TextGUI, typeof(TextMeshProUGUI)).image;
                    EditorGUI.LabelField(iconRect, new GUIContent(icon));
                }
            }

            protected override void OnBeforeChildrenContentGUI(PrefabTreeNode node, Rect headerRect, EasyEditorGUI.TreeNodeInfo info)
            {
                if (node.TextGUI != null)
                {
                    var mgr = node.Target.GetComponent<UiTextManager>();
                    if (mgr == null)
                    {
                        SirenixEditorGUI.MessageBox("未被UiTextManager管理", MessageType.Warning, EasyGUIStyles.InfoBoxCN, true);
                    }
                }
            }
        }

        #endregion
    }
}
