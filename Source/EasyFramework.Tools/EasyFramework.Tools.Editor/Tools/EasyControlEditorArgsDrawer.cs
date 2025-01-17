using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using EasyFramework.Generic;
using EasyFramework.Inspector;
using EasyFramework.Utilities;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EasyFramework.Tools.Editor
{
    public static class EasyControlUtility
    {
        private static FieldInfo GetArgsField(Type targetType)
        {
            return targetType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(f =>
                {
                    if (f.FieldType != typeof(EasyControlEditorArgs))
                    {
                        return false;
                    }

                    return f.IsPublic || f.HasCustomAttribute<SerializeField>();
                });
        }

        public static EasyControlEditorArgs GetArgs(object target)
        {
            return GetArgsField(target.GetType())?.GetValue(target) as EasyControlEditorArgs;
        }

        public static void SetArgs(object target, EasyControlEditorArgs args)
        {
            GetArgsField(target.GetType())?.SetValue(target, args);
        }

        private static Type[] _easyControlTypes;

        public static bool IsEasyControl(Type targetType)
        {
            if (_easyControlTypes == null)
            {
                _easyControlTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => GetArgsField(t) != null).ToArray();
            }

            return Array.Exists(_easyControlTypes, type => targetType == type);
        }

        public static EasyControlEditorArgs GetArgsInGameObject(GameObject target)
        {
            foreach (var component in target.GetComponents<Component>())
            {
                if (component == null)
                    continue;
                var args = GetArgs(component);
                if (args != null)
                {
                    return args;
                }
            }

            return null;
        }

        public static List<Component> GetChildren(GameObject target)
        {
            return target.GetComponentsInChildren<Component>()
                .Where(c =>
                {
                    if (c == null)
                        return false;
                    if (c.gameObject == target)
                        return false;
                    var args = GetArgs(c);
                    if (args == null)
                        return false;
                    return args.DoBounder;
                })
                .ToList();
        }
    }

    public class EasyControlEditorArgsDrawer : OdinValueDrawer<EasyControlEditorArgs>
    {
        #region Editor

        private List<Transform> _parents;
        private EasyControlSettings _settings => EasyControlSettings.Instance;
        private Component _comp;
        private EasyControlEditorArgs _args;
        private bool _isBind;

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

        protected override void Initialize()
        {
            base.Initialize();
            _comp = (Component)Property.Parent.ValueEntry.WeakSmartValue;

            _args = ValueEntry.SmartValue;
            _isBind = _comp as EasyControl == null;

            _parents = _comp.transform.FindParents(p =>
            {
                var args = EasyControlUtility.GetArgsInGameObject(p.gameObject);
                if (args == null)
                {
                    return false;
                }

                return args.DoViewModel;
            });
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (_isBind)
            {
                _args.DoViewModel = true;
            }

            _args.Expand = EasyEditorGUI.FoldoutGroup(
                new FoldoutGroupConfig(UniqueDrawerKey.Create(Property, this), "EasyControl设置", _args.Expand)
                {
                    OnContentGUI = rect =>
                    {
                        EditorGUI.BeginDisabledGroup(_isBind);
                        _args.DoViewModel = EditorGUILayout.Toggle("生成视图模型", _args.DoViewModel);
                        EditorGUI.EndDisabledGroup();

                        bool needExitGui = false;
                        if (_args.DoViewModel)
                        {
                            if (!_args.ViewModel.IsInitialized)
                            {
                                _args.ViewModel.ClassName = _comp.gameObject.name;
                                _args.ViewModel.GenerateDir = _settings.ViewModelDefault.GenerateDir;
                                _args.ViewModel.Namespace = _settings.ViewModelDefault.Namespace;
                                _args.ViewModel.BaseClass.Value = typeof(MonoBehaviour);

                                _args.ViewModel.IsInitialized = true;
                            }

                            EasyEditorGUI.BoxGroup(new BoxGroupConfig("视图模型配置")
                            {
                                OnContentGUI = headerRect => OnViewModelContentGUI(headerRect, out needExitGui)
                            });
                        }

                        _args.DoBounder = EditorGUILayout.Toggle("作为被绑定者", _args.DoBounder);

                        if (_args.DoBounder)
                        {
                            if (!_args.Bounder.IsInitialized)
                            {
                                var bindableTypes = _comp.GetComponents<Component>()
                                    .Where(c => c != null)
                                    .Select(c => c.GetType())
                                    .ToArray();

                                _args.Bounder.TypeToBind.Value =
                                    bindableTypes.Length > 1 ? bindableTypes[1] : bindableTypes[0];

                                if (_parents.IsNotNullOrEmpty())
                                {
                                    _args.Bounder.Parent = _parents[0];
                                }

                                _args.Bounder.Name = _comp.gameObject.name;
                                _args.Bounder.Access = _settings.BounderDefault.Access;
                                _args.Bounder.AutoNamingNotations = _settings.BounderDefault.AutoNamingNotations;
                                _args.Bounder.AutoAddCommentPara = _settings.BounderDefault.AutoAddCommentPara;
                                _args.Bounder.Comment = _settings.BounderDefault.Comment;

                                _args.Bounder.IsInitialized = true;
                            }

                            EasyEditorGUI.BoxGroup(new BoxGroupConfig("绑定配置")
                            {
                                OnContentGUI = OnBounderContentGUI
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
            if (_args.ViewModel.ClassNameSameAsGameObjectName)
            {
                _args.ViewModel.ClassName = _comp.gameObject.name;
            }

            EditorGUILayout.LabelField("状态", _isBind ? "已构建" : "未构建");

            EditorGUI.BeginDisabledGroup(_isBind);
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

            var lbl = _args.ViewModel.BaseClass.Value == null
                ? "<None>"
                : _args.ViewModel.BaseClass.Value.FullName;
            EasyEditorGUI.DrawSelectorDropdown(
                new SelectorDropdownConfig<Type>("父级", lbl, BaseTypes,
                    t => _args.ViewModel.BaseClass.Value = t)
                {
                    MenuItemNameGetter = t => t.FullName
                });

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_comp);
            }

            EditorGUI.EndDisabledGroup();

            if (SirenixEditorGUI.SDFIconButton("构建", EditorGUIUtility.singleLineHeight,
                    SdfIconType.PencilFill))
            {
                if (!CheckBind())
                {
                    return;
                }

                Build();
            }

            if (_args.ViewModel.ClassType != null)
            {
                if (SirenixEditorGUI.SDFIconButton("绑定", EditorGUIUtility.singleLineHeight,
                        SdfIconType.Bezier))
                {
                    Bind();
                }
            }

            if (_isBind)
            {
                if (SirenixEditorGUI.SDFIconButton("修正", EditorGUIUtility.singleLineHeight,
                        SdfIconType.Tools))
                {
                    FixValues();
                }
            }
        }

        private void OnBounderContentGUI(Rect headerRect)
        {
            if (_args.Bounder.NameSameAsGameObjectName)
            {
                _args.Bounder.Name = _comp.gameObject.name;
            }

            var bindableTypes = _comp.GetComponents<Component>()
                .Where(c => c != null)
                .Select(c => c.GetType()).ToArray();
            EditorGUI.BeginChangeCheck();

            if (_args.Bounder.TypeToBind.Value == null)
            {
                EasyEditorGUI.MessageBox("必须得有一个要绑定的组件", MessageType.Error);
            }

            var btnLabel = _args.Bounder.TypeToBind.Value == null
                ? "<None>"
                : _args.Bounder.TypeToBind.Value.FullName;

            EasyEditorGUI.DrawSelectorDropdown(
                new SelectorDropdownConfig<Type>("要绑定的组件", btnLabel,
                    bindableTypes,
                    t => _args.Bounder.TypeToBind.Value = t)
                {
                    MenuItemNameGetter = t => t.FullName
                });

            btnLabel = _args.Bounder.Parent == null
                ? "<None>"
                : _args.Bounder.Parent.gameObject.name;

            if (_args.Bounder.Parent == null)
            {
                EasyEditorGUI.MessageBox("必须得有一个父级", MessageType.Error);
            }

            EasyEditorGUI.DrawSelectorDropdown(
                new SelectorDropdownConfig<Transform>("父级", btnLabel,
                    _parents,
                    c => _args.Bounder.Parent = c)
                {
                    MenuItemNameGetter = c => c.gameObject.name
                });

            _args.Bounder.NameSameAsGameObjectName = EditorGUILayout.Toggle(
                "变量名称与游戏对象名称相同",
                _args.Bounder.NameSameAsGameObjectName);

            CheckIdentifier("变量名称", _args.Bounder.Name);
            using (new EditorGUI.DisabledScope(_args.Bounder.NameSameAsGameObjectName))
            {
                _args.Bounder.Name =
                    EditorGUILayout.TextField("变量名称", _args.Bounder.Name);
            }

            _args.Bounder.AutoNamingNotations = EditorGUILayout.Toggle(
                "自动命名规范",
                _args.Bounder.AutoNamingNotations);

            if (_args.Bounder.AutoNamingNotations)
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.TextField("变量名称（自动命名规范）", _args.Bounder.GetName());
                }
            }

            _args.Bounder.Access = EnumSelector<EasyControlBindAccess>.DrawEnumField(
                new GUIContent("访问标识符"),
                _args.Bounder.Access);

            _args.Bounder.AutoAddCommentPara = EditorGUILayout.Toggle(
                "注释自动添加段落xml",
                _args.Bounder.AutoAddCommentPara);

            EditorGUILayout.PrefixLabel("注释");
            _args.Bounder.Comment = EditorGUILayout.TextArea(_args.Bounder.Comment);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_comp);
            }
        }

        #endregion

        #region Tool

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

        private void FixValues()
        {
            if (_isBind)
            {
                Debug.Assert(_comp.GetComponent<EasyControl>() == null);
                _args.DoViewModel = true;
            }

            if (_args.DoViewModel)
            {
                _args.ViewModel.ClassType = _comp.GetType();
                Debug.Assert(_args.ViewModel.ClassType != null);

                var path = AssetDatabase.GetAssetPath(_comp.GetScript()!);
                Debug.Assert(path.IsNotNullOrEmpty());

                _args.ViewModel.GenerateDir = path[(path.IndexOf('/') + 1)..path.LastIndexOf('/')];
            }
        }

        [MenuItem("GameObject/EasyFramework/Add EasyControl-ViewModel")]
        private static void AddEasyControlViewModel()
        {
            foreach (var o in Selection.gameObjects)
            {
                var c = o.GetOrAddComponent<EasyControl>();
                EasyControlUtility.GetArgs(c).DoViewModel = true;
            }
        }

        [MenuItem("GameObject/EasyFramework/Add EasyControl-Bounder")]
        private static void AddEasyControlBounder()
        {
            foreach (var o in Selection.gameObjects)
            {
                var c = o.GetOrAddComponent<EasyControl>();
                EasyControlUtility.GetArgs(c).DoBounder = true;
            }
        }

        #endregion

        #region Build

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
            if (File.Exists(path)) return;

            var data = new
            {
                Usings = new List<string>
                {
                    "System",
                    "System.Collections.Generic",
                    "UnityEngine",
                    _args.ViewModel.BaseClass.Value.Namespace
                }.Distinct(),
                ClassName = _args.ViewModel.ClassName,
                Namespace = _args.ViewModel.Namespace,
                BaseClassName = _args.ViewModel.BaseClass.Value.Name
            };

            var compileUnit = new CodeCompileUnit();

            var codeNamespace = new CodeNamespace(data.Namespace);
            compileUnit.Namespaces.Add(codeNamespace);

            foreach (var u in data.Usings)
            {
                codeNamespace.Imports.Add(new CodeNamespaceImport(u));
            }

            var codeClass = new CodeTypeDeclaration(data.ClassName)
            {
                IsPartial = true,
                TypeAttributes = TypeAttributes.Public
            };

            codeClass.BaseTypes.Add(data.BaseClassName);

            codeNamespace.Types.Add(codeClass);

            using var writer = new StringWriter();
            var provider = CodeDomProvider.CreateProvider("CSharp");
            var options = new CodeGeneratorOptions
            {
                BracingStyle = "C",
                IndentString = "    "
            };

            provider.GenerateCodeFromCompileUnit(compileUnit, writer, options);

            var result = writer.ToString();
            result = result[result.IndexOf("using System;", StringComparison.Ordinal)..];
            File.WriteAllText(path, result);
        }

        private void BuildCsDesigner(string path)
        {
            Debug.Assert(_args.DoViewModel);

            var children = EasyControlUtility.GetChildren(_comp.gameObject);

            var data = new
            {
                Usings = new[] { "EasyFramework.Tools", "UnityEngine" }
                    .Concat(children
                        .Select(c => EasyControlUtility.GetArgs(c).Bounder.TypeToBind.Value.Namespace))
                    .Distinct(),
                Namespace = _args.ViewModel.Namespace,
                ClassName = _args.ViewModel.ClassName,
                Children = children.Select(c =>
                {
                    var args = EasyControlUtility.GetArgs(c);
                    return new
                    {
                        Type = args.Bounder.TypeToBind.Value.Name,
                        Name = args.Bounder.GetName(),
                        OriginName = args.Bounder.Name,
                        CommentSplits = GetBounderCommentSplits(args),
                        Access = args.Bounder.Access,
                    };
                })
            };

            var compileUnit = new CodeCompileUnit();

            var codeNamespace = new CodeNamespace(data.Namespace);
            compileUnit.Namespaces.Add(codeNamespace);

            foreach (var u in data.Usings)
            {
                codeNamespace.Imports.Add(new CodeNamespaceImport(u));
            }

            var codeClass = new CodeTypeDeclaration(data.ClassName)
            {
                IsPartial = true,
                TypeAttributes = TypeAttributes.Public
            };

            foreach (var child in data.Children)
            {
                var field = new CodeMemberField
                {
                    Name = child.Name,
                    Type = new CodeTypeReference(child.Type),
                    Attributes = child.Access == EasyControlBindAccess.Public
                        ? MemberAttributes.Public
                        : MemberAttributes.Private
                };

                if (child.Access == EasyControlBindAccess.PrivateWithSerializeFieldAttribute)
                {
                    field.CustomAttributes.Add(new CodeAttributeDeclaration("SerializeField"));
                }

                field.CustomAttributes.Add(new CodeAttributeDeclaration(
                    "EasyBounderControl",
                    new CodeAttributeArgument(new CodePrimitiveExpression(child.OriginName))
                ));

                if (child.CommentSplits.IsNotNullOrEmpty())
                {
                    foreach (var split in child.CommentSplits)
                    {
                        field.Comments.Add(new CodeCommentStatement(split));
                    }
                }

                codeClass.Members.Add(field);
            }

            if (data.Namespace.IsNotNullOrWhiteSpace())
            {
                codeClass.Members.Add(new CodeSnippetTypeMember(@"
#if UNITY_EDITOR
        /// <summary>
        /// <para>EasyControl的编辑器参数</para>
        /// <para>（不要在代码中使用，仅在编辑器中有效！）</para>
        /// </summary>
        [SerializeField()]
        private EasyControlEditorArgs _easyControlEditorArgs;
#endif"));
            }
            else
            {
                codeClass.Members.Add(new CodeSnippetTypeMember(@"
#if UNITY_EDITOR
    /// <summary>
    /// <para>EasyControl的编辑器参数</para>
    /// <para>（不要在代码中使用，仅在编辑器中有效！）</para>
    /// </summary>
    [SerializeField()]
    private EasyControlEditorArgs _easyControlEditorArgs;
#endif"));
            }

            // Add class to namespace
            codeNamespace.Types.Add(codeClass);

            using var writer = new StringWriter();
            var provider = CodeDomProvider.CreateProvider("CSharp");
            var options = new CodeGeneratorOptions
            {
                BracingStyle = "C",
                IndentString = "    "
            };

            provider.GenerateCodeFromCompileUnit(compileUnit, writer, options);

            var result = writer.ToString();
            File.WriteAllText(path, result);
        }

        private List<string> GetBounderCommentSplits(EasyControlEditorArgs args)
        {
            if (args.Bounder.Comment.IsNullOrWhiteSpace())
            {
                return null;
            }

            var comment = args.Bounder.Comment.Replace("\r\n", "\n");
            var commentSplits = comment.Split('\n').ToList();

            if (args.Bounder.AutoAddCommentPara)
            {
                for (int i = 0; i < commentSplits.Count; i++)
                {
                    commentSplits[i] = "<para>" + commentSplits[i] + "</para>";
                }
            }

            commentSplits.Insert(0, "<summary>");
            commentSplits.Add("</summary>");

            return commentSplits;
        }

        private void Bind()
        {
            Debug.Assert(_args.DoViewModel && _args.ViewModel.ClassType != null);

            if (_comp.GetComponent(_args.ViewModel.ClassType))
            {
                goto delete_self;
            }

            _comp = _comp.gameObject.AddComponent(_args.ViewModel.ClassType);
            EasyControlUtility.SetArgs(_comp, _args);

            delete_self:
            var c = _comp.GetComponent<EasyControl>();
            if (c != null)
                Object.DestroyImmediate(c);

            var children = EasyControlUtility.GetChildren(_comp.gameObject);
            var fields = _comp.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.GetCustomAttribute<EasyBounderControlAttribute>() != null)
                .ToArray();

            foreach (var child in children)
            {
                var args = EasyControlUtility.GetArgs(child);
                var f = fields.FirstOrDefault(f => GetOriginName(f) == args.Bounder.Name);
                if (f == null)
                {
                    EditorUtility.DisplayDialog("错误",
                        $"绑定GameObject（{child.gameObject.name}）失败，" +
                        $"视图模型中没有“{args.Bounder.Name}”，" +
                        $"可能需要重新生成视图模型！", "确认");
                    return;
                }

                var value = child.gameObject.GetComponent(f.FieldType);
                if (value == null)
                {
                    EditorUtility.DisplayDialog("错误",
                        $"绑定GameObject（{child.gameObject.name}）失败，" +
                        $"没有组件“{f.FieldType}”", "确认");
                    return;
                }

                f.SetValue(_comp, value);
            }

            return;

            string GetOriginName(FieldInfo field) =>
                field.GetCustomAttribute<EasyBounderControlAttribute>().OriginName;
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

            var children = EasyControlUtility.GetChildren(_comp.gameObject);
            var nameCheck = new HashSet<string>();
            foreach (var child in children)
            {
                var arg = EasyControlUtility.GetArgs(child);
                if (arg.DoBounder)
                {
                    string error = GetIdentifierError("变量名称", arg.Bounder.Name);
                    if (error.IsNotNullOrEmpty())
                    {
                        EditorUtility.DisplayDialog("错误", $"绑定“{child.gameObject.name}”出现错误：{error}", "确认");
                        return false;
                    }

                    if (!nameCheck.Add(arg.Bounder.Name))
                    {
                        EditorUtility.DisplayDialog("错误",
                            $"绑定“{child.gameObject.name}”出现错误：重复的变量名称（{arg.Bounder.Name}）", "确认");
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion
    }
}
