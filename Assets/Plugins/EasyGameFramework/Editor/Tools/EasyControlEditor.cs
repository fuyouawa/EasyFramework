using EasyFramework;
using Scriban;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System;
using UnityEditor;
using UnityEngine;

namespace EasyGameFramework.Editor
{
    public class EasyControlEditor : OdinEditor
    {
        #region Editor

        private List<Transform> _parents;
        private EasyControlSettings _settings => EasyGameFrameworkSettings.Instance.EasyControlSettings;
        private Component _comp;
        private EasyControlArgs _args;
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

        protected override void OnEnable()
        {
            base.OnEnable();

            _comp = (Component)target;
            _args = ((IEasyControl)target).GetEasyControlArgs();
            _isBind = target is not EasyControl;

            _parents = _comp.transform.FindParents(p =>
            {
                var c = p.GetComponent<IEasyControl>();
                if (c == null)
                    return false;
                return c.GetEasyControlArgs().DoViewModel;
            });
        }

        protected override void DrawTree()
        {
            base.DrawTree();

            _args.Expand = EasyEditorGUI.FoldoutGroup(
                new EasyEditorGUI.FoldoutGroupConfig(this, "EasyControl设置", _args.Expand)
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
                                _args.ViewModel.BaseClass.Type = typeof(MonoBehaviour);

                                _args.ViewModel.IsInitialized = true;
                            }

                            EasyEditorGUI.BoxGroup(new EasyEditorGUI.BoxGroupConfig("视图模型配置")
                            {
                                OnContentGUI = headerRect => OnViewModelContentGUI(headerRect, out needExitGui)
                            });
                        }

                        _args.DoBounder = EditorGUILayout.Toggle("作为被绑定者", _args.DoBounder);

                        if (_args.DoBounder)
                        {
                            if (!_args.Bounder.IsInitialized)
                            {
                                var bindableTypes = _comp.GetComponents<Component>().Select(c => c.GetType())
                                    .ToArray();

                                _args.Bounder.TypeToBind.Type =
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

                            EasyEditorGUI.BoxGroup(new EasyEditorGUI.BoxGroupConfig("绑定配置")
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

            var bindableTypes = _comp.GetComponents<Component>().Select(c => c.GetType()).ToArray();
            EditorGUI.BeginChangeCheck();

            if (_args.Bounder.TypeToBind.Type == null)
            {
                EasyEditorGUI.MessageBox("必须得有一个要绑定的组件", MessageType.Error);
            }

            var btnLabel = _args.Bounder.TypeToBind.Type == null
                ? "<None>"
                : _args.Bounder.TypeToBind.Type.FullName;

            EasyEditorGUI.DrawSelectorDropdown(
                new EasyEditorGUI.SelectorDropdownConfig<Type>("要绑定的组件", btnLabel,
                    bindableTypes,
                    t => _args.Bounder.TypeToBind.Type = t)
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
                new EasyEditorGUI.SelectorDropdownConfig<Transform>("父级", btnLabel,
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

            _args.Bounder.Access =
                (EasyControlBindAccess)SirenixEditorFields.EnumDropdown("访问标识符", _args.Bounder.Access);
            
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
                _args.ViewModel.ClassType = target.GetType();
                Debug.Assert(_args.ViewModel.ClassType != null);

                var path = AssetDatabase.GetAssetPath(_comp.GetScript()!);
                Debug.Assert(path.IsNotNullOrEmpty());

                _args.ViewModel.GenerateDir = path[(path.IndexOf('/') + 1)..path.LastIndexOf('/')];
            }
        }

        private static IEasyControl[] GetChildren(GameObject obj)
        {
            return obj.GetComponentsInChildren<IEasyControl>()
                .Where(m =>
                {
                    var arg = m.GetEasyControlArgs();
                    return arg.DoBounder
                           && arg.Bounder.Parent == obj.transform;
                }).ToArray();
        }

        private static string _codeGenerator;
        private static string _codeGenerator2;
        private static string _designerCodeGenerator;
        private static string _designerCodeGenerator2;

        private static string CodeGenerator
        {
            get
            {
                if (_codeGenerator.IsNullOrEmpty())
                {
                    _codeGenerator = File.ReadAllText(Path.Combine(Application.dataPath, AssetsPath.EditorResDirectory,
                        "EasyControl.CodeGenerator.scriban"));
                }

                return _codeGenerator;
            }
        }

        private static string CodeGenerator2
        {
            get
            {
                if (_codeGenerator2.IsNullOrEmpty())
                {
                    _codeGenerator2 = File.ReadAllText(Path.Combine(Application.dataPath, AssetsPath.EditorResDirectory,
                        "EasyControl.CodeGenerator2.scriban"));
                }

                return _codeGenerator2;
            }
        }

        private static string DesignerCodeGenerator
        {
            get
            {
                if (_designerCodeGenerator.IsNullOrEmpty())
                {
                    _designerCodeGenerator = File.ReadAllText(Path.Combine(Application.dataPath,
                        AssetsPath.EditorResDirectory,
                        "EasyControl.DesignerCodeGenerator.scriban"));
                }

                return _designerCodeGenerator;
            }
        }

        private static string DesignerCodeGenerator2
        {
            get
            {
                if (_designerCodeGenerator2.IsNullOrEmpty())
                {
                    _designerCodeGenerator2 = File.ReadAllText(Path.Combine(Application.dataPath,
                        AssetsPath.EditorResDirectory,
                        "EasyControl.DesignerCodeGenerator2.scriban"));
                }

                return _designerCodeGenerator2;
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

                var parsedTemplate = Template.Parse(_args.ViewModel.Namespace.IsNotNullOrWhiteSpace()
                    ? CodeGenerator
                    : CodeGenerator2);

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
            Debug.Assert(_args.DoViewModel);

            var children = GetChildren(_comp.gameObject);

            var data = new
            {
                Usings = new[] { "EasyGameFramework", "UnityEngine" }
                    .Concat(children
                        .Select(c => c.GetEasyControlArgs().Bounder.TypeToBind.Type.Namespace))
                    .Distinct(),
                Namespace = _args.ViewModel.Namespace,
                ClassName = _args.ViewModel.ClassName,
                Children = children.Select(c =>
                {
                    var args = c.GetEasyControlArgs();
                    return new
                    {
                        Type = args.Bounder.TypeToBind.Type.Name,
                        Name = args.Bounder.GetName(),
                        OriginName = args.Bounder.Name,
                        Comment = GetBounderComment(args),
                        Access = args.Bounder.Access.ToString(),
                    };
                })
            };

            var parsedTemplate = Template.Parse(_args.ViewModel.Namespace.IsNotNullOrWhiteSpace()
                ? DesignerCodeGenerator
                : DesignerCodeGenerator2);

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

            string GetBounderComment(EasyControlArgs args)
            {
                if (args.Bounder.Comment.IsNullOrWhiteSpace())
                {
                    return string.Empty;
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

                if (_args.ViewModel.Namespace.IsNotNullOrWhiteSpace())
                {
                    for (int i = 0; i < commentSplits.Count; i++)
                    {
                        commentSplits[i] = "\t\t/// " + commentSplits[i];
                    }
                }
                else
                {
                    for (int i = 0; i < commentSplits.Count; i++)
                    {
                        commentSplits[i] = "\t/// " + commentSplits[i];
                    }
                }

                return string.Join("\r\n", commentSplits);
            }
        }

        private void Bind()
        {
            Debug.Assert(_args.DoViewModel && _args.ViewModel.ClassType != null);

            if (_comp.GetComponent(_args.ViewModel.ClassType))
            {
                goto delete_self;
            }

            _comp = _comp.gameObject.AddComponent(_args.ViewModel.ClassType);
            ((IEasyControl)_comp).SetEasyControlArgs(_args);

            delete_self:
            var c = _comp.GetComponent<EasyControl>();
            if (c != null)
                DestroyImmediate(c);

            var children = GetChildren(_comp.gameObject);
            var fields = _comp.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.GetCustomAttribute<EasyBounderControlAttribute>() != null)
                .ToArray();

            foreach (var child in children)
            {
                var comp = (Component)child;
                var args = child.GetEasyControlArgs();
                var f = fields.FirstOrDefault(f => GetOriginName(f) == args.Bounder.Name);
                if (f == null)
                {
                    EditorUtility.DisplayDialog("错误",
                        $"绑定GameObject（{comp.gameObject.name}）失败，" +
                        $"视图模型中没有“{args.Bounder.Name}”，" +
                        $"可能需要重新生成视图模型！", "确认");
                    return;
                }

                var value = comp.gameObject.GetComponent(f.FieldType);
                if (value == null)
                {
                    EditorUtility.DisplayDialog("错误",
                        $"绑定GameObject（{comp.gameObject.name}）失败，" +
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

            var children = GetChildren(_comp.gameObject);
            var nameCheck = new HashSet<string>();
            foreach (var child in children)
            {
                var arg = child.GetEasyControlArgs();
                if (arg.DoBounder)
                {
                    string error = GetIdentifierError("变量名称", arg.Bounder.Name);
                    if (error.IsNotNullOrEmpty())
                    {
                        var comp = (Component)child;
                        EditorUtility.DisplayDialog("错误", $"绑定“{comp.gameObject.name}”出现错误：{error}", "确认");
                        return false;
                    }

                    if (!nameCheck.Add(arg.Bounder.Name))
                    {
                        var comp = (Component)child;
                        EditorUtility.DisplayDialog("错误",
                            $"绑定“{comp.gameObject.name}”出现错误：重复的变量名称（{arg.Bounder.Name}）", "确认");
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion
    }
}
