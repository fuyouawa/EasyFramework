using System;
using System.IO;
using System.Linq;
using System.Reflection;
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
    }

    public class EasyViewModel : MonoBehaviour, IEasyViewModel
    {
        [SerializeField, HideInInspector]
        private EasyViewModelArgs _easyViewModelArgs = new();
    }

#if UNITY_EDITOR
    namespace Editor
    {
        public abstract class EasyControlEditorBase : OdinEditor
        {
            private List<Transform> _parents;
            private Component _comp;
            private EasyViewModelArgs _args;

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

            private static EasyViewModelArgs GetArgs(object viewModel)
            {
                var argsField = viewModel.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .First(f => f.FieldType == typeof(EasyViewModelArgs));
                return (EasyViewModelArgs)argsField.GetValue(viewModel);
            }

            private static void SetArgs(object viewModel, EasyViewModelArgs args)
            {
                var argsField = viewModel.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .First(f => f.FieldType == typeof(EasyViewModelArgs));
                argsField.SetValue(viewModel, args);
            }

            protected override void OnEnable()
            {
                base.OnEnable();

                _comp = (Component)target;
                _args = GetArgs(target);

                _parents = _comp.transform.FindParents(p =>
                {
                    var c = p.GetComponent<IEasyViewModel>();
                    if (c == null)
                        return false;
                    return GetArgs(c).DoViewModel;
                });
            }

            protected override void DrawTree()
            {
                base.DrawTree();

                _args.Expand = EasyEditorGUI.FoldoutGroup(
                    new EasyEditorGUI.FoldoutGroupConfig(this, "EasyViewModel设置", _args.Expand)
                    {
                        OnContentGUI = rect =>
                        {
                            _args.DoViewModel = EditorGUILayout.Toggle("生成视图模型", _args.DoViewModel);

                            bool needExitGui = false;
                            if (_args.DoViewModel)
                            {
                                if (!_args.ViewModel.IsInitialized)
                                {
                                    _args.ViewModel.ClassName = _comp.gameObject.name;
                                    _args.ViewModel.GenerateDir = "Scripts/Ui";
                                    _args.ViewModel.BaseClass.Type = typeof(MonoBehaviour);

                                    _args.ViewModel.IsInitialized = true;
                                }

                                EasyEditorGUI.BoxGroup(new EasyEditorGUI.BoxGroupConfig("视图模型配置")
                                {
                                    OnContentGUI = headerRect => OnViewModelContentGUI(headerRect, out needExitGui)
                                });
                            }

                            _args.DoBinder = EditorGUILayout.Toggle("作为被绑定者", _args.DoBinder);

                            if (_args.DoBinder)
                            {
                                if (!_args.Binder.IsInitialized)
                                {
                                    var bindableTypes = _comp.GetComponents<Component>().Select(c => c.GetType())
                                        .ToArray();

                                    _args.Binder.TypeToBind.Type =
                                        bindableTypes.Length > 1 ? bindableTypes[1] : bindableTypes[0];

                                    if (_parents.IsNotNullOrEmpty())
                                    {
                                        _args.Binder.Parent = _parents[0];
                                    }

                                    _args.Binder.Name = _comp.gameObject.name;

                                    _args.Binder.IsInitialized = true;
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

            private static void CheckIdentifier(string name, string id)
            {
                var error = GetIdentifierError(name, id);
                if (error.IsNotNullOrEmpty())
                {
                    EasyEditorGUI.MessageBox(error, MessageType.Error);
                }
            }

            private static string GetIdentifierError(string name, string id)
            {
                try
                {
                    id.IsValidIdentifier(true);

                    return string.Empty;
                }
                catch (InvalidIdentifierException e)
                {
                    return e.Type switch
                    {
                        InvalidIdentifierTypes.Empty => $"{name}为空",
                        InvalidIdentifierTypes.IllegalBegin => $"{name}必须以字母或下划线开头",
                        InvalidIdentifierTypes.IllegalContent => $"{name}的其余部分只能是字母、数字或下划线",
                        InvalidIdentifierTypes.CSharpKeywordConflict => $"{name}不能与C#关键字冲突",
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }
            }

            private void OnViewModelContentGUI(Rect headerRect, out bool needExitGui)
            {
                var isBuild = target is not EasyViewModel;

                if (_args.ViewModel.ClassNameSameAsGameObjectName)
                {
                    _args.ViewModel.ClassName = _comp.gameObject.name;
                }

                EditorGUILayout.LabelField("状态", isBuild ? "已构建" : "未构建");

                EditorGUI.BeginChangeCheck();
                EditorGUI.BeginChangeCheck();
                _args.ViewModel.GenerateDir = SirenixEditorFields.FolderPathField(
                    new GUIContent("代码生成目录"),
                    _args.ViewModel.GenerateDir, "Assets", false, false);
                needExitGui = EditorGUI.EndChangeCheck();
                _args.ViewModel.Namespace =
                    EditorGUILayout.TextField("命名空间", _args.ViewModel.Namespace);

                _args.ViewModel.ClassNameSameAsGameObjectName =
                    EditorGUILayout.Toggle("类名与游戏对象名称相同",
                        _args.ViewModel.ClassNameSameAsGameObjectName);

                CheckIdentifier("类名", _args.ViewModel.ClassName);
                using (new EditorGUI.DisabledScope(_args.ViewModel.ClassNameSameAsGameObjectName))
                {
                    _args.ViewModel.ClassName =
                        EditorGUILayout.TextField("类名", _args.ViewModel.ClassName);
                }

                var lbl = _args.ViewModel.BaseClass.Type == null
                    ? "<None>"
                    : _args.ViewModel.BaseClass.Type.FullName;
                EasyEditorGUI.DrawSelectorDropdown(
                    new EasyEditorGUI.SelectorDropdownConfig<Type>("父级", lbl, BaseTypes,
                        t => _args.ViewModel.BaseClass.Type = t)
                    {
                        MenuItemNameGetter = t => t.FullName
                    });

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(_comp);
                }

                if (SirenixEditorGUI.SDFIconButton("构建", EditorGUIUtility.singleLineHeight,
                        SdfIconType.ArrowUpSquare))
                {
                    if (!CheckBind())
                    {
                        return;
                    }

                    Build();
                }
            }

            private void OnBinderContentGUI(Rect headerRect)
            {
                if (_args.Binder.NameSameAsGameObjectName)
                {
                    _args.Binder.Name = _comp.gameObject.name;
                }

                var bindableTypes = _comp.GetComponents<Component>().Select(c => c.GetType()).ToArray();
                EditorGUI.BeginChangeCheck();

                if (_args.Binder.TypeToBind.Type == null)
                {
                    EasyEditorGUI.MessageBox("必须得有一个要绑定的组件", MessageType.Error);
                }

                var btnLabel = _args.Binder.TypeToBind.Type == null
                    ? "<None>"
                    : _args.Binder.TypeToBind.Type.FullName;

                EasyEditorGUI.DrawSelectorDropdown(
                    new EasyEditorGUI.SelectorDropdownConfig<Type>("要绑定的组件", btnLabel,
                        bindableTypes,
                        t => _args.Binder.TypeToBind.Type = t)
                    {
                        MenuItemNameGetter = t => t.FullName
                    });

                btnLabel = _args.Binder.Parent == null
                    ? "<None>"
                    : _args.Binder.Parent.gameObject.name;

                if (_args.Binder.Parent == null)
                {
                    EasyEditorGUI.MessageBox("必须得有一个父级", MessageType.Error);
                }

                EasyEditorGUI.DrawSelectorDropdown(
                    new EasyEditorGUI.SelectorDropdownConfig<Transform>("父级", btnLabel,
                        _parents,
                        c => _args.Binder.Parent = c)
                    {
                        MenuItemNameGetter = c => c.gameObject.name
                    });

                _args.Binder.NameSameAsGameObjectName =
                    EditorGUILayout.Toggle("变量名称与游戏对象名称相同",
                        _args.Binder.NameSameAsGameObjectName);

                CheckIdentifier("变量名称", _args.Binder.Name);
                using (new EditorGUI.DisabledScope(_args.Binder.NameSameAsGameObjectName))
                {
                    _args.Binder.Name =
                        EditorGUILayout.TextField("变量名称", _args.Binder.Name);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(_comp);
                }
            }

            private static readonly string CsGenerateTemplateWithNamespace =
                @"{{- for using in usings}}
using {{using}};
{{- end }}

namespace {{namespace}}
{
    public partial class {{class_name}} : {{base_class}}
    {
    }
}
";

            private static readonly string CsGenerateTemplateNoNamespace =
                @"{{- for using in usings}}
using {{using}};
{{- end }}

public partial class {{class_name}} : {{base_class}}
{
}
";

            private static readonly string CsDesignerGenerateTemplateWithNamespace =
                @"{{- for using in usings}}
using {{using}};
{{- end }}

namespace {{namespace}}
{
    public partial class {{class_name}} : IEasyViewModel
    {
        [SerializeField, HideInInspector]
        private EasyViewModelArgs _easyViewModelArgs = new();

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
        using UnityEditor;
        using EasyGameFramework.Editor;

        [CustomEditor(typeof({{class_name}}))]
        public class {{class_name}}Editor : EasyControlEditorBase
        {
        }
    }
#endif
}
";

            private static readonly string CsDesignerGenerateTemplateNoNamespace =
                @"{{- for using in usings}}
using {{using}};
{{- end }}

public partial class {{class_name}} : IEasyViewModel
{
    [SerializeField, HideInInspector]
    private EasyViewModelArgs _easyViewModelArgs = new();

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
    using UnityEditor;
    using EasyGameFramework.Editor;

    [CustomEditor(typeof({{class_name}}))]
    public class {{class_name}}Editor : EasyControlEditorBase
    {
    }
}
#endif
";

            private void Build()
            {
                var dir = Path.Combine(Application.dataPath, _args.ViewModel.GenerateDir);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var path = Path.Combine(dir, _args.ViewModel.ClassName);
                var designerPath = path + ".Designer.cs";
                path += ".cs";

                BuildCs(path);
                BuildCsDesigner(designerPath);
                AssetDatabase.Refresh();
            }

            private void BuildCs(string path)
            {
                if (!File.Exists(path))
                {
                    var data = new
                    {
                        Usings = new List<string>
                        {
                            "System",
                            "System.Collections.Generic",
                            _args.ViewModel.BaseClass.Type.Namespace
                        },
                        Namespace = _args.ViewModel.Namespace,
                        ClassName = _args.ViewModel.ClassName,
                        BaseClass = _args.ViewModel.BaseClass.Type.Name
                    };

                    var parsedTemplate = Template.Parse(_args.ViewModel.Namespace.IsNullOrEmpty()
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

                    File.WriteAllText(path, result);
                }
            }

            private void BuildCsDesigner(string path)
            {
                var children = GetChildren();

                var data = new
                {
                    Usings = children.Select(c => GetArgs(c).Binder.TypeToBind.Type.Namespace)
                        .Append("EasyGameFramework").Distinct(),
                    Namespace = _args.ViewModel.Namespace,
                    ClassName = _args.ViewModel.ClassName,
                    Children = children.Select(c =>
                    {
                        var arg = GetArgs(c);
                        return new
                        {
                            Type = arg.Binder.TypeToBind.Type.Name,
                            Name = arg.Binder.Name,
                            Comment = arg.Binder.Comment,
                        };
                    })
                };

                var parsedTemplate = Template.Parse(_args.ViewModel.Namespace.IsNullOrEmpty()
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

                File.WriteAllText(path, result);
            }

            private void BindChildren()
            {

            }

            private bool CheckBind()
            {
                if (_args.DoViewModel)
                {
                    string error = GetIdentifierError("类名", _args.ViewModel.ClassName);
                    if (error.IsNotNullOrEmpty())
                    {
                        EditorUtility.DisplayDialog("错误", $"类名不规范：{error}", "确认");
                        return false;
                    }
                }

                var children = GetChildren();
                var nameCheck = new HashSet<string>();
                foreach (var child in children)
                {
                    var arg = GetArgs(child);
                    if (arg.DoBinder)
                    {
                        string error = GetIdentifierError("变量名称", arg.Binder.Name);
                        if (error.IsNotNullOrEmpty())
                        {
                            var comp = (Component)child;
                            EditorUtility.DisplayDialog("错误", $"绑定“{comp.gameObject.name}”出现错误：{error}", "确认");
                            return false;
                        }

                        if (!nameCheck.Add(arg.Binder.Name))
                        {
                            var comp = (Component)child;
                            EditorUtility.DisplayDialog("错误",
                                $"绑定“{comp.gameObject.name}”出现错误：重复的变量名称（{arg.Binder.Name}）", "确认");
                            return false;
                        }
                    }
                }

                return true;
            }

            private IEasyViewModel[] GetChildren()
            {
                return _comp.GetComponentsInChildren<IEasyViewModel>()
                    .Where(m =>
                    {
                        var arg = GetArgs(m);
                        return arg.DoBinder
                               && arg.Binder.Parent == _comp.transform;
                    }).ToArray();
            }
        }

        [CustomEditor(typeof(EasyViewModel))]
        public class EasyViewModelEditor : EasyControlEditorBase
        {
        }
    }
#endif
}
