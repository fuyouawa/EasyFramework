using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using TMPro;
using EasyFramework;
using UnityEditor;
using UnityEngine;
using JetBrains.Annotations;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine.SceneManagement;

namespace EasyGameFramework.Editor
{
    public class UiTextManagerWindow : OdinMenuEditorWindow
    {
        #region Editor

        public class WindowTemp
        {
            public Rect WindowPosition;
            public float MenuWidth;
        }

        [Flags]
        public enum ViewModes
        {
            Correct = 1,
            Incorrect = 1 << 2,
            All = Correct | Incorrect,
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
                Instance.SaveEditor();
                AssetDatabase.Refresh();
            }

            [TitleGroup("数据存储")]
            [Button("加载编辑器设置")]
            [UsedImplicitly]
            private void Load()
            {
                Instance.LoadEditor();
            }
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
        private PrefabItemBase _prevSelectionItem;

        public void Rebuild()
        {
            ForceMenuTreeRebuild();
        }

        private static readonly string Group = "资产视图";

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree
            {
                { "设置", _settings, SdfIconType.GearFill },
                { Group, null }
            };

            if (_settings.ManagerPosition == ManagerPositions.InProject)
            {
                LoadPrefabsInProject(tree);
            }
            else
            {
                LoadPrefabsInScene(tree);
            }

            tree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;

            tree.Selection.SupportsMultiSelect = false;
            tree.Selection.SelectionChanged += type =>
            {
                if (type == SelectionChangedType.ItemAdded)
                {
                    if (tree.Selection.SelectedValue is PrefabItemBase item)
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


        protected override void OnEnable()
        {
            base.OnEnable();
            EditorApplication.update += EditorUpdate;

            LoadEditor();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            EditorApplication.update -= EditorUpdate;
            SaveEditor();
        }

        private double _prevAutoSaveTime;

        private void EditorUpdate()
        {
            if (EditorApplication.timeSinceStartup - _prevAutoSaveTime >= _settings.AutoSaveInterval)
            {
                SaveEditor();
                _prevAutoSaveTime = EditorApplication.timeSinceStartup;
            }
        }

        public void SaveEditor()
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

        public void LoadEditor()
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

        #endregion

        #region LoadPrefabs

        private void LoadPrefabsInScene(OdinMenuTree tree)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.IsValid() && scene.isLoaded)
                {
                    var sceneName = $"{scene.name}.scene";
                    tree.Add($"{Group}/{sceneName}", null, EditorIcons.UnityLogo);

                    foreach (var o in scene.GetRootGameObjects())
                    {
                        if (o.GetComponentsInChildren<TextMeshProUGUI>(true).IsNotNullOrEmpty())
                        {
                            var item = new PrefabSceneItem(o, $"{sceneName}/{o.name}");

                            item.Analyse();
                            if (AssetItemFilter(item))
                            {
                                var icon = item.HasIncorrect()
                                    ? EditorIcons.UnityWarningIcon
                                    : EasyEditorIcons.UnityPrefabIcon;

                                var menuItems = tree.Add($"{Group}/{sceneName}/{o.name}", item, icon).ToArray();
                                Debug.Assert(menuItems.Length == 1);
                            }

                            item.ClearCache();
                        }
                    }
                }
            }
        }


        private void LoadPrefabsInProject(OdinMenuTree tree)
        {
            var assetsPath = $"Assets/{_settings.AssetsManagerPath}/";

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
                            var icon = item.HasIncorrect()
                                ? EditorIcons.UnityWarningIcon
                                : EasyEditorIcons.UnityPrefabIcon;
                            var p = path.Substring(assetsPath.Length,
                                path.Length - assetsPath.Length - ".prefab".Length);
                            var menuItems = tree.Add($"{Group}/{p}", item, icon).ToArray();
                            foreach (var menuItem in menuItems)
                            {
                                menuItem.Icon = menuItem.Name != obj.name
                                    ? EditorIcons.UnityFolderIcon
                                    : icon;
                            }
                        }

                        item.ClearCache();
                    }
                }

                var items = tree.MenuItems.First(i => i.Name == Group).GetChildMenuItemsRecursive(false);
                foreach (var item in items)
                {
                    if (item.Value == null)
                    {
                        item.Icon = EditorIcons.UnityFolderIcon;
                    }
                }
            }
        }


        private bool AssetItemFilter(PrefabItemBase item)
        {
            if (_settings == null)
                return true;
            if (_settings.ViewModes == 0)
                return false;
            if (_settings.ViewModes == ViewModes.All)
                return true;

            var warn = item.HasIncorrect();

            if (warn && _settings.ViewModes.HasFlag(ViewModes.Incorrect))
            {
                return true;
            }

            if (!warn && _settings.ViewModes.HasFlag(ViewModes.Correct))
            {
                return true;
            }

            return false;
        }

        #endregion

        #region PrefabItem

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
            [InfoBoxCN("1、如果没有UITextManager组件会自动添加\n" +
                       "2、如果UITextManager的\"字体资产预设\"是null，根据Text(UI)组件的字体和材质设置，判断属于\"预设管理器\"中的哪个字体资产预设，然后自动赋值\n" +
                       "3、如果UITextManager的\"文本属性预设\"是null，根据Text(UI)组件的字体大小和字体颜色设置，判断属于\"预设管理器\"中的哪个文本属性预设，然后自动赋值\n" +
                       "4、如果UITextManager的\"字体资产预设\"依然是null，会使用\"预设管理器\"中定义的\"默认字体资产预设\"\n" +
                       "5、如果组件上有多个重复的UITextManager，则只保留第一个，删除其他重复项")]
            [Button("自动分析处理", Icon = SdfIconType.Tools)]
            [UsedImplicitly]
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
                    if (!node.IsTextNode())
                    {
                        AutoHandleNodeIncorrect(node);
                    }

                    AutoHandleIncorrectRecursion(node.Children);
                }
            }

            public void AutoHandleIncorrectRecursion(List<PrefabTreeNode> children)
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

            public void AutoHandleNodeIncorrect(PrefabTreeNode node)
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
                if (mgr.GetFontAssetPreset() == null)
                {
                    foreach (var preset in UiTextPresetsManager.Instance.FontAssetPresets)
                    {
                        if (preset.FontAsset == node.TextGUI.font && preset.Material == node.TextGUI.fontSharedMaterial)
                        {
                            mgr.SetFontAssetPreset(preset);
                        }
                    }
                }

                if (mgr.GetTextPropertiesPreset() == null)
                {
                    foreach (var preset in UiTextPresetsManager.Instance.TextPropertiesPresets)
                    {
                        if (preset.FontColor == node.TextGUI.color &&
                            preset.FontSize.Approximately(node.TextGUI.fontSize))
                        {
                            mgr.SetTextPropertiesPreset(preset);
                        }
                    }
                }

                // 如果UITextManager的"字体资产预设"依然是null, 会使用"预设管理器"中定义的"默认字体资产预设"
                if (mgr.GetFontAssetPreset() == null)
                {
                    mgr.SetFontAssetPreset(UiTextPresetsManager.Instance.GetDefaultFontAssetPreset());
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
            public PrefabTree Tree = new();


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

            private List<string> _prefabPaths = new();

            public void Analyse()
            {
                ClearCache();
                var prefab = GetPrefab();
                foreach (var text in prefab.GetComponentsInChildren<TextMeshProUGUI>(true))
                {
                    if (!PrefabFilter(text.gameObject))
                        continue;

                    var path = text.gameObject.transform.GetRelativePath(prefab.transform, true);
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

            private void UpdatePrefabRecursion(GameObject prefab, List<PrefabTreeNode> children)
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

            private PrefabTreeNode GetPrefabTreeNode(GameObject prefab, string name, PrefabTreeNode parent)
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

            protected abstract PrefabTreeNode AllocTreeNode(GameObject prefab, GameObject target,
                PrefabTreeNode parent);

            private bool PrefabFilter(GameObject prefab)
            {
                if (ViewMode == 0)
                    return false;
                if (ViewMode == ItemsViewModes.All)
                    return true;

                var warn = GameObjectHasIncorrect(prefab);

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
                if (ps.assetPath == PrefabPath)
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

            protected override PrefabTreeNode AllocTreeNode(GameObject prefab, GameObject target,
                PrefabTreeNode parent)
            {
                var ps = PrefabStageUtility.GetCurrentPrefabStage();
                if (ps.assetPath == PrefabPath)
                {
                    prefab = ps.prefabContentsRoot;
                }

                return new PrefabTreeNode(this, prefab, target.name, parent);
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

            protected override PrefabTreeNode AllocTreeNode(GameObject prefab, GameObject target,
                PrefabTreeNode parent)
            {
                return new PrefabTreeNode(this, prefab, target.name, parent);
            }
        }

        public static bool GameObjectHasIncorrect(GameObject obj)
        {
            var mgrs = obj.GetComponents<UiTextManager>();
            if (CollectionExtension.IsNullOrEmpty(mgrs) || mgrs.Length > 1)
                return true;
            var mgr = mgrs[0];
            if (mgr.GetFontAssetPreset() == null || mgr.GetTextPropertiesPreset() == null)
                return true;

            return false;
        }

        #endregion

        #region PrefabTreeNode

        [Serializable]
        public class PrefabTreeNode
        {
            public string FullName { get; private set; }
            public string Path { get; private set; }
            public PrefabItemBase Owner { get; }

            public GameObject Prefab { get; private set; }

            public TextMeshProUGUI TextGUI { get; private set; }
            public PrefabTreeNode Parent { get; private set; }
            public GameObject Target { get; private set; }
            public List<PrefabTreeNode> Children { get; } = new();
            public EasyEditorGUI.TreeNodeState State { get; } = new();

            public PrefabTreeNode(PrefabItemBase owner, GameObject prefab, string targetName,
                PrefabTreeNode parent = null)
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

                TextGUI = Target.GetComponent<TextMeshProUGUI>();
            }

            public bool IsTextNode()
            {
                return TextGUI != null;
            }

            public bool HasIncorrect()
            {
                Debug.Assert(IsTextNode());
                return GameObjectHasIncorrect(Target.gameObject);
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

            public void Select()
            {
                Selection.activeObject = Target;
                EditorGUIUtility.PingObject(Target);
            }
        }

        [Serializable]
        public class PrefabTree : List<PrefabTreeNode>
        {
        }

        public class PrefabTreeDrawer : TreeValueDrawer<PrefabTree, PrefabTreeNode>
        {
            public override string GetNodeLabel(PrefabTreeNode node)
            {
                return "       " + node.Target.name;
            }

            public override IList<PrefabTreeNode> GetNodeChildren(PrefabTreeNode node)
            {
                return node.Children;
            }

            public override EasyEditorGUI.TreeNodeState GetNodeState(PrefabTreeNode node)
            {
                node.State.Expandable = true;
                return node.State;
            }

            protected override void OnNodeCoveredTitleBarGUI(PrefabTreeNode node, Rect headerRect,
                EasyEditorGUI.TreeNodeInfo info)
            {
                var iconWidth = EditorGUIUtility.singleLineHeight;
                if (node.IsTextNode() && node.HasIncorrectRecursion())
                {
                    var iconRect = headerRect.AlignRight(iconWidth).SetSize(Vector2.one * iconWidth);
                    var icon = EditorIcons.UnityWarningIcon;
                    EditorGUI.LabelField(iconRect, new GUIContent(icon));
                }

                if (node.TextGUI != null)
                {
                    var iconRect = headerRect.AddX(14).SetSize(Vector2.one * iconWidth);
                    var icon = EditorGUIUtility.ObjectContent(node.TextGUI, typeof(TextMeshProUGUI)).image;
                    EditorGUI.LabelField(iconRect, new GUIContent(icon));
                }
            }

            protected override void OnBeforeChildrenContentGUI(PrefabTreeNode node, Rect headerRect,
                EasyEditorGUI.TreeNodeInfo info)
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
                            var fontAssetPreset = mgr.GetFontAssetPreset();
                            if (fontAssetPreset == null)
                            {
                                EasyEditorGUI.MessageBox("字体资产预设为null！", MessageType.Warning);
                                hasIncorrect = true;
                            }
                            else
                            {
                                EasyEditorGUI.MessageBox($"字体资产预设为：{fontAssetPreset.LabelToShow}",
                                    MessageType.Info);
                            }

                            var textPropertiesPreset = mgr.GetTextPropertiesPreset();
                            if (textPropertiesPreset == null)
                            {
                                EasyEditorGUI.MessageBox("文本属性预设为null！", MessageType.Warning);
                                hasIncorrect = true;
                            }
                            else
                            {
                                EasyEditorGUI.MessageBox($"文本属性预设为：{textPropertiesPreset.LabelToShow}",
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
                            node.Owner.OpenPrefab();
                            node.Owner.AutoHandleNodeIncorrect(node);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
