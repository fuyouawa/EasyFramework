using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Sirenix.Utilities.Editor;
using UnityEngine.SceneManagement;

namespace EasyGameFramework.Editor
{
    internal class AssetReferenceSearcherWindow : EditorWindow
    {
        private static AssetReferenceSearcherWindow _instance;

        [MenuItem("Tools/UniTool/资源引用查找器")]
        public static void ShowWindow()
        {
            _instance = GetWindow<AssetReferenceSearcherWindow>("资源引用查找器");
        }

        public enum Modes
        {
            [LabelText("在场景中搜索")]
            InScene,

            [LabelText("在资产中搜索")]
            InAssets
        }

        public Type TypeToSearch;

        [LabelText("模式")]
        public Modes Mode;

        public List<Component> Results = new List<Component>();

        public bool ShowError;
        private bool _expandResults;

        public class ResultTreeNode
        {
            public bool Expand;
            public string Name;
            public ResultTreeNode Parent;
            public Dictionary<string, ResultTreeNode> Children;

            public ResultTreeNode(string name, ResultTreeNode parent = null)
            {
                Name = name;
                Expand = false;
                Children = new Dictionary<string, ResultTreeNode>();
                Parent = parent;
            }

            private GameObject _gameObject;

            public GameObject GameObjectInScene()
            {
                if (_gameObject == null)
                {
                    var path = GetAbsPath();
                    _gameObject = EasyGameFramework.GameObjectUtility.FindByAbsolutePath(path);
                }

                return _gameObject;
            }

            public int CalcHierarchy()
            {
                int res = 0;
                var p = Parent;
                while (p != null)
                {
                    ++res;
                    p = p.Parent;
                }

                return res;
            }

            public string GetAbsPath()
            {
                string path = Name;
                var p = Parent;
                while (p != null)
                {
                    path = p.Name + "/" + path;
                    p = p.Parent;
                }

                return path;
            }
        }

        public Dictionary<string, ResultTreeNode> ResultTree = new Dictionary<string, ResultTreeNode>();
        private static Type[] _allComponentTypes;

        public static Type[] AllComponentTypes
        {
            get
            {
                if (_allComponentTypes == null)
                {
                    _allComponentTypes = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .Where(t => t.IsSubclassOf(typeof(Component))).ToArray();
                }

                return _allComponentTypes;
            }
        }

        public void GenerateResultTree()
        {
            ResultTree.Clear();
            foreach (var result in Results)
            {
                var path = result.transform.GetAbsolutePath();
                path = path[1..];

                var split = path.Split('/');

                var s0 = split[0];
                if (!ResultTree.TryGetValue(s0, out var node))
                {
                    node = new ResultTreeNode(s0);
                    ResultTree[s0] = node;
                }

                for (int i = 1; i < split.Length; i++)
                {
                    var s = split[i];
                    if (!node.Children.TryGetValue(s, out var node2))
                    {
                        node2 = new ResultTreeNode(s, node);
                        node.Children[s] = node2;
                        node = node2;
                    }
                    else
                    {
                        node = node2;
                    }
                }
            }
        }

        void DrawResultTree(Dictionary<string, ResultTreeNode> resultTree)
        {
            if (resultTree == null || resultTree.Count == 0)
                return;

            foreach (var node in resultTree)
            {
                DrawResultTreeNode(node);
            }
        }

        void DrawResultTreeNode(KeyValuePair<string, ResultTreeNode> node)
        {
            var off = node.Value.CalcHierarchy() * 10;
            var rect = EditorGUILayout.GetControlRect();
            rect.x += off;
            rect.width -= off;

            if (node.Value.Children.Count == 0)
            {
                rect.x += 5;
                rect.width -= 5;

                if (Mode == Modes.InScene)
                {
                    var go = node.Value.GameObjectInScene();
                    if (go != null)
                    {
                        using (new EditorGUI.DisabledScope(true))
                        {
                            EditorGUI.ObjectField(rect, go, typeof(GameObject), true);
                        }
                    }
                    else
                    {
                        GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
                        style.normal.textColor = style.onNormal.textColor = new Color32(209, 137, 24, 255);
                        EditorGUI.LabelField(rect, "Missing GameObject!", style);
                    }
                }
            }
            else
            {
                // var config = new FoldoutHeaderConfig(node.Key)
                // {
                //     Expand = node.Value.Expand,
                //     HasBox = false,
                //     FoldoutRectGetter = () => rect
                // };
                // node.Value.Expand = EasyEditorGUI.FoldoutHeader(config);
                // if (SirenixEditorGUI.BeginFadeGroup(node, node.Value.Expand))
                // {
                //     DrawResultTree(node.Value.Children);
                // }
                // SirenixEditorGUI.EndFadeGroup();
                
                var config = new FoldoutGroupConfig(node, node.Key, node.Value.Expand)
                {
                    HasBox = false,
                    HeaderRectGetter = () => rect,
                    OnContentGUI = () => DrawResultTree(node.Value.Children)
                };
                node.Value.Expand = EasyEditorGUI.FoldoutGroup(config);
            }
        }

        void OnEnable()
        {
            EditorApplication.update += UpdateWindow;
        }

        void OnDisable()
        {
            EditorApplication.update -= UpdateWindow;
        }

        void UpdateWindow()
        {
            Repaint();
        }

        void OnExpandResultTree(Dictionary<string, ResultTreeNode> tree, bool b)
        {
            if (tree == null || tree.Count == 0)
                return;

            foreach (var node in tree)
            {
                OnExpandResultTreeNode(node, b);
            }
        }

        void OnExpandResultTreeNode(KeyValuePair<string, ResultTreeNode> node, bool b)
        {
            node.Value.Expand = b;
            OnExpandResultTree(node.Value.Children, b);
        }

        private void OnGUI()
        {
            EasyEditorGUI.Title("参数设置");
            if (ShowError)
            {
                SirenixEditorGUI.ErrorMessageBox("无效类型");
            }

            EasyEditorGUI.DrawSelectorDropdown(new SelectorDropdownConfig<Type>(
                "要查找的类型",
                TypeToSearch == null ? "None" : TypeToSearch.FullName, AllComponentTypes,
                t => { TypeToSearch = t; }));

            Mode = EnumSelector<Modes>.DrawEnumField(new GUIContent("模式"), Mode);

            var btnRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight + 2);
            if (SirenixEditorGUI.SDFIconButton(btnRect, "查找引用", SdfIconType.Search))
            {
                Results.Clear();

                var t = TypeToSearch;
                ShowError = t == null;
                if (t != null)
                {
                    if (Mode == Modes.InScene)
                    {
                        for (int i = 0; i < SceneManager.sceneCount; i++)
                        {
                            var scene = SceneManager.GetSceneAt(i);
                            if (scene.IsValid() && scene.isLoaded)
                            {
                                foreach (var o in scene.GetRootGameObjects())
                                {
                                    Results.AddRange(o.GetComponentsInChildren(t));
                                }
                            }
                        }
                    }

                    GenerateResultTree();

                    _expandResults = true;
                }
            }
            
            _expandResults = EasyEditorGUI.WindowLikeToolGroup(new WindowLikeToolGroupConfig(this, "结果视图")
            {
                Expand = _expandResults,
                OnMaximize = () => OnExpandResultTree(ResultTree, true),
                OnMinimize = () => OnExpandResultTree(ResultTree, false),
                ShowFoldout = ResultTree.Count > 0,
                OnContentGUI = () => DrawResultTree(ResultTree)
            });
        }
    }
}
