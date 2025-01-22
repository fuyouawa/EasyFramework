using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System;
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
    public class EasyControlEditorArgsDrawer : OdinValueDrawer<EasyControlEditorArgs>
    {
        private List<Transform> _parents;
        private EasyControlSettings _settings => EasyControlSettings.Instance;
        private Component _comp;
        private EasyControlEditorArgs _args;
        private bool _isBind;
        private EasyControlBuilder _builder = new EasyControlBuilder();

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

        private FoldoutGroupConfig _foldoutGroupConfig;

        protected override void Initialize()
        {
            base.Initialize();
            _comp = (Component)Property.Parent.ValueEntry.WeakSmartValue;

            _args = ValueEntry.SmartValue;
            _isBind = _comp as EasyControl == null;

            _parents = _comp.transform.FindParents(p =>
            {
                var args = EasyControlHelper.GetArgsInGameObject(p.gameObject);
                if (args == null)
                {
                    return false;
                }

                return args.DoViewModel;
            });

            _foldoutGroupConfig = new FoldoutGroupConfig(
                UniqueDrawerKey.Create(Property, this),
                new GUIContent("EasyControl设置"), _args.Expand,
                OnEasyControlSettingsGUI);
        }

        private void OnEasyControlSettingsGUI(Rect headerRect)
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

                EasyEditorGUI.BoxGroup(
                    EditorHelper.TempContent("视图模型配置"),
                    rect => OnViewModelContentGUI(rect, out needExitGui));
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

                    Type initType;
                    if (bindableTypes.Length > 1)
                    {
                        var t = bindableTypes[1];
                        if (t == typeof(EasyControl))
                        {
                            initType = bindableTypes.Length > 2
                                ? bindableTypes[2]
                                : bindableTypes[0];
                        }
                        else
                        {
                            initType = t;
                        }
                    }
                    else
                    {
                        initType = bindableTypes[0];
                    }

                    _args.Bounder.TypeToBind.Value = initType;

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

                EasyEditorGUI.BoxGroup(
                    EditorHelper.TempContent("绑定配置"),
                    OnBounderContentGUI);
            }

            if (needExitGui)
            {
                GUIHelper.ExitGUI(false);
            }
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (_isBind)
            {
                _args.DoViewModel = true;
            }

            _foldoutGroupConfig.Expand = _args.Expand;
            _args.Expand = EasyEditorGUI.FoldoutGroup(_foldoutGroupConfig);
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

            EasyControlHelper.CheckIdentifier("类名", _args.ViewModel.ClassName);
            using (new EditorGUI.DisabledScope(_args.ViewModel.ClassNameSameAsGameObjectName))
            {
                _args.ViewModel.ClassName =
                    EditorGUILayout.TextField("类名", _args.ViewModel.ClassName);
            }

            var lbl = _args.ViewModel.BaseClass.Value == null
                ? "<None>"
                : _args.ViewModel.BaseClass.Value.FullName;
            EasyEditorGUI.DrawSelectorDropdown(
                new SelectorDropdownConfig<Type>(
                    EditorHelper.TempContent("父级"),
                    EditorHelper.TempContent2(lbl),
                    BaseTypes,
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

                _builder.Setup(_args, _comp);
                _builder.Build();
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
                new SelectorDropdownConfig<Type>(
                    EditorHelper.TempContent("要绑定的组件"),
                    EditorHelper.TempContent2(btnLabel),
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
                new SelectorDropdownConfig<Transform>(
                    EditorHelper.TempContent("父级"),
                    EditorHelper.TempContent2(btnLabel),
                    _parents,
                    c => _args.Bounder.Parent = c)
                {
                    MenuItemNameGetter = c => c.gameObject.name
                });

            _args.Bounder.NameSameAsGameObjectName = EditorGUILayout.Toggle(
                "变量名称与游戏对象名称相同",
                _args.Bounder.NameSameAsGameObjectName);

            EasyControlHelper.CheckIdentifier("变量名称", _args.Bounder.Name);
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

            _args.Bounder.Access = EasyEditorField.Enum(
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

        private void Bind()
        {
            Debug.Assert(_args.DoViewModel && _args.ViewModel.ClassType != null);

            if (_comp.GetComponent(_args.ViewModel.ClassType))
            {
                goto delete_self;
            }

            _comp = _comp.gameObject.AddComponent(_args.ViewModel.ClassType);
            EasyControlHelper.SetArgs(_comp, _args);

            delete_self:
            var c = _comp.GetComponent<EasyControl>();
            if (c != null)
                Object.DestroyImmediate(c);

            var children = EasyControlHelper.GetChildren(_comp.gameObject);
            var fields = _comp.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.GetCustomAttribute<EasyBounderControlAttribute>() != null)
                .ToArray();

            foreach (var child in children)
            {
                var args = EasyControlHelper.GetArgs(child);
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
                string error = EasyControlHelper.GetIdentifierError("类名", _args.ViewModel.ClassName);
                if (error.IsNotNullOrEmpty())
                {
                    EditorUtility.DisplayDialog("错误", $"类名不规范：{error}", "确认");
                    return false;
                }
            }

            var children = EasyControlHelper.GetChildren(_comp.gameObject);
            var nameCheck = new HashSet<string>();
            foreach (var child in children)
            {
                var arg = EasyControlHelper.GetArgs(child);
                if (arg.DoBounder)
                {
                    string error = EasyControlHelper.GetIdentifierError("变量名称", arg.Bounder.Name);
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
    }
}
