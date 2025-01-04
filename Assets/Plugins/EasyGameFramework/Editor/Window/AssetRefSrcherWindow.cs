using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;
using EasyFramework;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Sirenix.Utilities.Editor;
using UnityEngine.SceneManagement;

namespace EasyGameFramework.Editor
{
    internal class AssetRefSrcherWindow : OdinEditorWindow
    {
        private static AssetRefSrcherWindow _instance;

        public static AssetRefSrcherWindow Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.Assert(HasOpenInstances<AssetRefSrcherWindow>());
                    _instance = GetWindow<AssetRefSrcherWindow>("AssetReferenceSearcher Window");
                }

                Debug.Assert(_instance != null);
                return _instance;
            }
        }

        [MenuItem("Tools/EasyGameFramework/AssetReferenceSearcher Window")]
        [UsedImplicitly]
        public static void ShowWindow()
        {
            _instance = GetWindow<AssetRefSrcherWindow>("AssetReferenceSearcher Window");
        }

        public enum Modes
        {
            [LabelText("在场景中搜索")]
            InScene,

            [LabelText("在资产中搜索")]
            InAssets
        }

        [PropertyOrder(0)]
        [LabelText("要查找的类型")]
        [ValueDropdown("AllComponentDropdownTypes")]
        public Type TypeToSearch;
        
        [PropertyOrder(1)]
        [LabelText("模式")]
        public Modes Mode;
        
        [PropertyOrder(2)]
        [Button("查找引用", Icon = SdfIconType.Search)]
        [UsedImplicitly]
        private void FindReference()
        {
            _results.Clear();

            var t = TypeToSearch;
            _showError = t == null;
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
                                _results.AddRange(o.GetComponentsInChildren(t));
                            }
                        }
                    }
                }

                GenerateResultTree();
            }
        }
        
        [PropertyOrder(3)]
        [LabelText("结果视图")]
        public ResultTree Tree = new ResultTree();
        
        private List<Component> _results = new List<Component>();
        private bool _showError;

        #region ResultTree
        
        public class ResultTreeNode
        {
            public bool Expand { get; set; }
            public string Name { get; }
            public string FullName { get; }
            public Transform Target { get; }
            public ResultTreeNode Parent { get; }
            public bool IsSearched { get; }
            public List<ResultTreeNode> Children { get; } = new();

            public bool IsScene => Parent == null;

            public ResultTreeNode(string name, ResultTreeNode parent = null)
            {
                Name = name;
                Expand = false;
                Parent = parent;
                if (parent != null)
                {
                    if (parent.IsScene)
                    {
                        FullName = name;
                        var f = GameObject.Find('/' + name)!;
                        Target = f.transform!;
                    }
                    else
                    {
                        FullName = parent.FullName + '/' + name;
                        foreach (Transform child in parent.Target)
                        {
                            if (child.gameObject.name == name)
                            {
                                Target = child;
                            }
                        }
                    }

                    IsSearched = Target!.GetComponent(Instance.TypeToSearch) != null;
                }
            }
        }

        public class ResultTree : List<ResultTreeNode>
        {
        }

        public class ResultTreeDrawer : TreeValueDrawer<ResultTree, ResultTreeNode>
        {
            public override string GetNodeLabel(ResultTreeNode node)
            {
                return node.IsSearched ? string.Empty : node.Name;
            }

            public override IList<ResultTreeNode> GetNodeChildren(ResultTreeNode node)
            {
                return node.Children;
            }

            public override bool GetNodeExpandState(ResultTreeNode node)
            {
                return node.Expand;
            }

            public override void OnNodeExpandStateChanged(ResultTreeNode node, bool expand)
            {
                node.Expand = expand;
            }

            protected override void OnNodeCoveredTitleBarGUI(ResultTreeNode node, int hierarchy, Rect headerRect)
            {
                if (node.IsSearched)
                {
                    var off = Indent + 4;
                    headerRect.x += off;
                    headerRect.width -= off;
                    if (Instance.Mode == Modes.InScene)
                    {
                        using (new EditorGUI.DisabledScope(true))
                        {
                            EditorGUI.ObjectField(headerRect, node.Target, node.Target.GetType(), true);
                        }
                    }
                }
            }
        }

        #endregion

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

        private static ValueDropdownList<Type> _allComponentDropdownTypes;

        public static ValueDropdownList<Type> AllComponentDropdownTypes
        {
            get
            {
                if (_allComponentDropdownTypes == null)
                {
                    _allComponentDropdownTypes = new ValueDropdownList<Type>();
                    _allComponentDropdownTypes.AddRange(AllComponentTypes.Select(t =>
                        new ValueDropdownItem<Type>(t.FullName, t)));
                }

                return _allComponentDropdownTypes;
            }
        }

        public void GenerateResultTree()
        {
            Tree.Clear();
            foreach (var result in _results)
            {
                var path = result.transform.GetAbsolutePath();
                path = path[1..];

                var split = path.Split('/');

                var s0 = split[0];
                var node = Tree.Find(n => n.Name == s0);
                if (node == null)
                {
                    node = new ResultTreeNode(s0);
                    Tree.Add(node);
                }

                for (int i = 1; i < split.Length; i++)
                {
                    var s = split[i];
                    var node2 = node.Children.Find(n => n.Name == s);
                    if (node2 == null)
                    {
                        node2 = new ResultTreeNode(s, node);
                        node.Children.Add(node2);
                    }
                    node = node2;
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            EditorApplication.update += UpdateWindow;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            EditorApplication.update -= UpdateWindow;
        }

        void UpdateWindow()
        {
            Repaint();
        }
    }
}
