using System;
using System.IO;
using System.Linq;
using EasyFramework;
using Scriban;
using Sirenix.OdinInspector;
using UnityEngine;


#if UNITY_EDITOR
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
#endif

namespace EasyGameFramework
{
    [Serializable]
    public class EasyViewModelArgs
    {
        [Serializable]
        public class TypeStore
        {
            public string AssemblyQualifiedName;
            private Type _typeCache;

            public Type Type
            {
                get
                {
                    if (AssemblyQualifiedName.IsNullOrEmpty())
                        return null;

                    if (_typeCache == null)
                    {
                        _typeCache = Type.GetType(AssemblyQualifiedName, true);
                    }

                    Debug.Assert(_typeCache.AssemblyQualifiedName == AssemblyQualifiedName);

                    return _typeCache;
                }
                set
                {
                    AssemblyQualifiedName = value.AssemblyQualifiedName;
                    _typeCache = null;
                }
            }
        }

        [Serializable]
        public class ViewModelArgs
        {
            public string GenerateDir;
            public string Namespace;
            public bool ClassNameSameAsGameObjectName = true;
            public string ClassName;
            public TypeStore BaseClass = new();
            public bool IsInitialized;
        }

        [Serializable]
        public class BinderArgs
        {
            public Transform Parent;
            public TypeStore TypeToBind = new();
            public bool NameSameAsGameObjectName = true;
            public string Name;
            public string Comment;
            public bool IsInitialized;
        }

        public bool Expand;

        public bool DoViewModel;
        public bool DoBinder;

        public ViewModelArgs ViewModel = new();
        public BinderArgs Binder = new();
    }

    public interface IEasyViewModel
    {
        public EasyViewModelArgs EasyViewModelArgs { get; }
    }

    public class EasyViewModel : MonoBehaviour, IEasyViewModel
    {
        [SerializeField, HideInInspector]
        private EasyViewModelArgs _easyViewModelArgs = new();

        public EasyViewModelArgs EasyViewModelArgs => _easyViewModelArgs;
    }

#if UNITY_EDITOR
    namespace Editor
    {
        public abstract class EasyControlEditorBase : OdinEditor
        {
            private List<Transform> _parents;

            private static List<Type> _baseTypes;

            public static List<Type> BaseTypes
            {
                get
                {
                    if (_baseTypes == null)
                    {
                        _baseTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                            .Where(t => t.IsSubclassOf(typeof(Component)) && !t.IsSealed).ToList();
                    }

                    return _baseTypes;
                }
            }

            protected override void OnEnable()
            {
                base.OnEnable();

                var comp = (Component)target;
                _parents = comp.transform.FindParents(p =>
                {
                    var c = p.GetComponent<IEasyViewModel>();
                    if (c == null)
                        return false;
                    return c.EasyViewModelArgs.DoViewModel;
                });
            }

            protected override void DrawTree()
            {
                base.DrawTree();

                var control = (IEasyViewModel)target;
                var comp = (Component)target;
                var args = control.EasyViewModelArgs;

                args.Expand = EasyEditorGUI.FoldoutGroup(
                    new EasyEditorGUI.FoldoutGroupConfig(this, "EasyViewModel设置", args.Expand)
                    {
                        OnContentGUI = rect =>
                        {
                            args.DoViewModel = EditorGUILayout.Toggle("生成视图模型", args.DoViewModel);

                            bool needExitGui = false;
                            if (args.DoViewModel)
                            {
                                if (!args.ViewModel.IsInitialized)
                                {
                                    args.ViewModel.ClassName = comp.gameObject.name;
                                    args.ViewModel.GenerateDir = "Scripts/Ui";
                                    args.ViewModel.BaseClass.Type = typeof(MonoBehaviour);

                                    args.ViewModel.IsInitialized = true;
                                }

                                EasyEditorGUI.BoxGroup(new EasyEditorGUI.BoxGroupConfig("视图模型配置")
                                {
                                    OnContentGUI = headerRect => OnViewModelContentGUI(headerRect, out needExitGui)
                                });
                            }

                            args.DoBinder = EditorGUILayout.Toggle("作为被绑定者", args.DoBinder);

                            if (args.DoBinder)
                            {
                                if (!args.Binder.IsInitialized)
                                {
                                    var bindableTypes = comp.GetComponents<Component>().Select(c => c.GetType())
                                        .ToArray();

                                    args.Binder.TypeToBind.Type =
                                        bindableTypes.Length > 1 ? bindableTypes[1] : bindableTypes[0];

                                    if (_parents.IsNotNullOrEmpty())
                                    {
                                        args.Binder.Parent = _parents[0];
                                    }

                                    args.Binder.Name = comp.gameObject.name;

                                    args.Binder.IsInitialized = true;
                                }

                                EasyEditorGUI.BoxGroup(new EasyEditorGUI.BoxGroupConfig("绑定配置")
                                {
                                    OnContentGUI = OnBinderContentGUI
                                });
                            }

                            if (needExitGui)
                            {
                                GUIHelper.ExitGUI(false);
                            }
                        }
                    });
            }

            private void OnViewModelContentGUI(Rect headerRect, out bool needExitGui)
            {
                var control = (IEasyViewModel)target;
                var comp = (Component)target;
                var args = control.EasyViewModelArgs;
                var isBuild = target is not EasyViewModel;
                
                if (args.ViewModel.ClassNameSameAsGameObjectName)
                {
                    args.ViewModel.ClassName = comp.gameObject.name;
                }

                EditorGUILayout.LabelField("状态", isBuild ? "已构建" : "未构建");

                EditorGUI.BeginChangeCheck();
                EditorGUI.BeginChangeCheck();
                args.ViewModel.GenerateDir = SirenixEditorFields.FolderPathField(
                    new GUIContent("代码生成目录"),
                    args.ViewModel.GenerateDir, "Assets", false, false);
                needExitGui = EditorGUI.EndChangeCheck();
                args.ViewModel.Namespace =
                    EditorGUILayout.TextField("命名空间", args.ViewModel.Namespace);

                args.ViewModel.ClassNameSameAsGameObjectName =
                    EditorGUILayout.Toggle("类名与游戏对象名称相同",
                        args.ViewModel.ClassNameSameAsGameObjectName);

                using (new EditorGUI.DisabledScope(args.ViewModel.ClassNameSameAsGameObjectName))
                {
                    args.ViewModel.ClassName =
                        EditorGUILayout.TextField("类名", args.ViewModel.ClassName);
                }

                var lbl = args.ViewModel.BaseClass.Type == null
                    ? "<None>"
                    : args.ViewModel.BaseClass.Type.FullName;
                EasyEditorGUI.DrawSelectorDropdown(
                    new EasyEditorGUI.SelectorDropdownConfig<Type>("父级", lbl, BaseTypes,
                        t => args.ViewModel.BaseClass.Type = t)
                    {
                        MenuItemNameGetter = t => t.FullName,
                        PreferAssetPreviewAsIcon = true
                    });

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(comp);
                }

                if (SirenixEditorGUI.SDFIconButton("构建", EditorGUIUtility.singleLineHeight,
                        SdfIconType.ArrowUpSquare))
                {
                    Build();
                }
            }

            private void OnBinderContentGUI(Rect headerRect)
            {
                var control = (IEasyViewModel)target;
                var comp = (Component)target;
                var args = control.EasyViewModelArgs;
                
                if (args.Binder.NameSameAsGameObjectName)
                {
                    args.Binder.Name = comp.gameObject.name;
                }

                var bindableTypes = comp.GetComponents<Component>().Select(c => c.GetType()).ToArray();
                EditorGUI.BeginChangeCheck();

                if (args.Binder.TypeToBind.Type == null)
                {
                    EasyEditorGUI.MessageBox("必须得有一个要绑定的组件", MessageType.Error);
                }

                var btnLabel = args.Binder.TypeToBind.Type == null
                    ? "<None>"
                    : args.Binder.TypeToBind.Type.FullName;

                EasyEditorGUI.DrawSelectorDropdown(
                    new EasyEditorGUI.SelectorDropdownConfig<Type>("要绑定的组件", btnLabel,
                        bindableTypes,
                        t => args.Binder.TypeToBind.Type = t)
                    {
                        MenuItemNameGetter = t => t.FullName,
                        PreferAssetPreviewAsIcon = true
                    });

                btnLabel = args.Binder.Parent == null
                    ? "<None>"
                    : args.Binder.Parent.gameObject.name;

                if (args.Binder.Parent == null)
                {
                    EasyEditorGUI.MessageBox("必须得有一个父级", MessageType.Error);
                }

                EasyEditorGUI.DrawSelectorDropdown(
                    new EasyEditorGUI.SelectorDropdownConfig<Transform>("父级", btnLabel,
                        _parents,
                        c => args.Binder.Parent = c)
                    {
                        MenuItemNameGetter = c => c.gameObject.name
                    });

                args.Binder.NameSameAsGameObjectName =
                    EditorGUILayout.Toggle("变量名称与游戏对象名称相同",
                        args.Binder.NameSameAsGameObjectName);

                using (new EditorGUI.DisabledScope(args.Binder.NameSameAsGameObjectName))
                {
                    args.Binder.Name =
                        EditorGUILayout.TextField("变量名称", args.Binder.Name);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(comp);
                }
            }

            private static readonly string CsGenerateTemplateWithNamespace = @"
{{- for using in usings}}
using {{using}};
{{- end }}

namespace {{namespace}}
{
    public partial class {{class_name}} : {{base_class}}
    {
    }
}
";

            private static readonly string CsGenerateTemplateNoNamespace = @"
{{- for using in usings}}
using {{using}};
{{- end }}

public partial class {{class_name}} : {{base_class}}
{
}
";

            private static readonly string CsDesignerGenerateTemplateWithNamespace = @"
{{- for using in usings}}
using {{using}};
{{- end }}

namespace {{namespace}}
{
    public partial class {{class_name}} : IEasyViewModel
    {
        [SerializeField, HideInInspector]
        private EasyViewModelArgs _easyViewModelArgs = new();

        public EasyViewModelArgs EasyViewModelArgs => _easyViewModelArgs;


        {{- for child in children}}
        /// <summary>
        /// {{child.comment}}
        /// </summary>
        public {{child.type}} {{child.name}};
        {{- end }}
    }

#if UNITY_EDITOR
    namespace Editor
    {
        [CustomEditor(typeof({{class_name}}))]
        public class {{class_name}}Editor : Editor.EasyControlEditorBase
        {
        }
    }
#endif
}
";

            private static readonly string CsDesignerGenerateTemplateNoNamespace = @"
{{- for using in usings}}
using {{using}};
{{- end }}

public partial class {{class_name}} : IEasyViewModel
{
    [SerializeField, HideInInspector]
    private EasyViewModelArgs _easyViewModelArgs = new();

    public EasyViewModelArgs EasyViewModelArgs => _easyViewModelArgs;


    {{- for child in children}}
    /// <summary>
    /// {{child.comment}}
    /// </summary>
    public {{child.type}} {{child.name}};
    {{- end }}
}

#if UNITY_EDITOR
namespace Editor
{
    [CustomEditor(typeof({{class_name}}))]
    public class {{class_name}}Editor : Editor.EasyControlEditorBase
    {
    }
}
#endif
";

            private void Build()
            {
                var control = (IEasyViewModel)target;
                var args = control.EasyViewModelArgs;

                var dir = Path.Combine(Application.dataPath, args.ViewModel.GenerateDir);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var path = Path.Combine(dir, args.ViewModel.ClassName);
                var designerPath = path + ".Designer.cs";
                path += ".cs";

                BuildCs(path);
                BuildCsDesigner(designerPath);
            }

            private void BuildCs(string path)
            {
                var control = (IEasyViewModel)target;
                var args = control.EasyViewModelArgs;

                if (!File.Exists(path))
                {
                    var data = new
                    {
                        Usings = new List<string>
                        {
                            "System",
                            "System.Collections.Generic",
                            args.ViewModel.BaseClass.Type.Namespace
                        },
                        Namespace = args.ViewModel.Namespace,
                        ClassName = args.ViewModel.ClassName,
                        BaseClass = args.ViewModel.BaseClass.Type.Namespace
                    };

                    var parsedTemplate = Template.Parse(args.ViewModel.Namespace.IsNullOrEmpty()
                        ? CsGenerateTemplateNoNamespace
                        : CsGenerateTemplateWithNamespace);

                    if (parsedTemplate.HasErrors)
                    {
                        foreach (var error in parsedTemplate.Messages)
                        {
                            Debug.Log($"Error: {error.Message}");
                        }

                        return;
                    }

                    var result = parsedTemplate.Render(data);

                    Debug.Log(result);
                }
            }

            private void BuildCsDesigner(string path)
            {
                var control = (IEasyViewModel)target;
                var comp = (Component)target;
                var args = control.EasyViewModelArgs;

                var children = GetChildren();

                var data = new
                {
                    Usings = children.Select(c => c.EasyViewModelArgs.Binder.TypeToBind.Type.Namespace)
                        .Append("EasyGameFramework").Distinct(),
                    Namespace = args.ViewModel.Namespace,
                    ClassName = args.ViewModel.ClassName,
                    Children = children.Select(c => new
                    {
                        Type = c.EasyViewModelArgs.Binder.TypeToBind.Type.Name,
                        Name = c.EasyViewModelArgs.Binder.Name,
                        Comment = c.EasyViewModelArgs.Binder.Comment,
                    })
                };

                var parsedTemplate = Template.Parse(args.ViewModel.Namespace.IsNullOrEmpty()
                    ? CsDesignerGenerateTemplateNoNamespace
                    : CsDesignerGenerateTemplateWithNamespace);

                if (parsedTemplate.HasErrors)
                {
                    foreach (var error in parsedTemplate.Messages)
                    {
                        Debug.Log($"Error: {error.Message}");
                    }

                    return;
                }

                var result = parsedTemplate.Render(data);

                Debug.Log(result);
            }

            private IEasyViewModel[] GetChildren()
            {
                var comp = (Component)target;

                return comp.GetComponentsInChildren<IEasyViewModel>()
                    .Where(m => m.EasyViewModelArgs.DoBinder
                                && m.EasyViewModelArgs.Binder.Parent == comp.transform).ToArray();
            }
        }

        [CustomEditor(typeof(EasyViewModel))]
        public class EasyViewModelEditor : EasyControlEditorBase
        {
        }
    }
#endif
}
