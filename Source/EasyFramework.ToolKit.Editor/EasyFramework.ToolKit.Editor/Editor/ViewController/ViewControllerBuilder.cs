using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public class ViewControllerBuilder
    {
        private readonly ViewControllerEditorConfig _cfg;
        private readonly Component _component;
        private readonly IViewController _controller;
        private readonly ViewControllerSettings _settings;

        public ViewControllerBuilder(IViewController controller)
        {
            _controller = controller;
            _component = (Component)controller;
            _cfg = _controller.Config.EditorConfig;
            _settings = ViewControllerSettings.Instance;
        }

        public void Build()
        {
            var dir = Path.Combine(Application.dataPath, _cfg.GenerateDir);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var path = Path.Combine(dir, _controller.GetScriptName());
            var designerPath = path + ".Designer.cs";
            path += ".cs";

            BuildCs(path);
            BuildCsDesigner(designerPath);
            AssetDatabase.Refresh();
        }


        public bool Check()
        {
            string error = ViewControllerUtility.GetIdentifierError("类名", _cfg.ScriptName);
            if (error.IsNotNullOrEmpty())
            {
                EditorUtility.DisplayDialog("错误", $"类名不规范：{error}", "确认");
                return false;
            }

            var binders = _controller.GetAllBinders();

            var nameCheck = new HashSet<string>();
            foreach (var binder in binders)
            {
                var comp = (Component)binder;
                var bindName = binder.GetBindName();

                error = ViewControllerUtility.GetIdentifierError("变量名称", bindName);
                if (error.IsNotNullOrEmpty())
                {
                    EditorUtility.DisplayDialog("错误",
                        $"代码生成失败：绑定器 '{comp.gameObject.name}' 的绑定名称不规范（{error}）", "确认");
                    return false;
                }

                if (!nameCheck.Add(bindName))
                {
                    EditorUtility.DisplayDialog("错误",
                        $"代码生成失败：绑定器 '{comp.gameObject.name}' 的绑定变量名称冲突（{bindName}）", "确认");
                    return false;
                }
            }

            return true;
        }

        private static readonly string CsTemplate = @"
{{ Header }}
{{ Body }}
".TrimStart();

        private static readonly string CsNamespaceTemplate = @"
{{ Header }}
namespace {{ Namespace }}
{
{{ Body }}
}
".Trim();

        private static readonly string CsHeaderTemplate = @"
{%- for using in Usings -%}
using {{ using }};
{%- endfor -%}
".Trim();

        private static readonly string CsBodyTemplate = @"
public partial class {{ ClassName }} : {{ BaseClassName }}
{
{{ ClassTemplate }}
}
".Trim();

        private void BuildCs(string path)
        {
            if (File.Exists(path)) return;

            var engine = new TemplateEngine();
            var header = engine.Render(CsHeaderTemplate, new
            {
                Usings = new List<string>
                {
                    "System",
                    "System.Collections.Generic",
                    "UnityEngine",
                    _cfg.BaseClass.Namespace
                }.Distinct().ToArray()
            });
            var classTemplate = _settings.AutoIndent
                ? AddIndent(_settings.ClassTemplate)
                : _settings.ClassTemplate;
            var body = engine.Render(CsBodyTemplate, new
            {
                ClassName = _controller.GetScriptName(),
                BaseClassName = _cfg.BaseClass.Name,
                ClassTemplate = classTemplate
            });

            var code = CombineCode(engine, header, body, _cfg.Namespace);
            File.WriteAllText(path, code);
        }

        private static readonly string CsDesignerBodyTemplate = @"
public partial class {{ ClassName }} : IViewController
{
{{ Fields }}


    [SerializeField, PropertyOrder(1000000f)]
    private ViewControllerConfig _viewControllerConfig;

    ViewControllerConfig IViewController.Config
    {
        get => _viewControllerConfig;
        set => _viewControllerConfig = value;
    }
}
".Trim();

        private static readonly string CsDesignerHeaderStatement = @"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by view controller of the EasyFramework.
//     这些代码由EasyFramework的视图控制器生成。
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
//     更改可能导致未定义行为，并在下次代码生成时丢失。
// </auto-generated>
//------------------------------------------------------------------------------
".TrimStart();

        private void BuildCsDesigner(string path)
        {
            var binders = _controller.GetAllBinders();

            var engine = new TemplateEngine();

            var allTypes = new List<Type>()
            {
                typeof(IViewController),
                typeof(ViewControllerConfig),
                typeof(SerializeField),
                typeof(AutoBindingAttribute),
            };
            allTypes.AddRange(binders
                .Select(b => b.Config.EditorConfig.GetBindType())
                .Where(t => t.Namespace != _cfg.Namespace));

            switch (_cfg.BindersGroupType)
            {
                case ViewControllerBindersGroupType.None:
                    break;
                case ViewControllerBindersGroupType.Title:
                    allTypes.Add(typeof(TitleGroupAttribute));
                    break;
                case ViewControllerBindersGroupType.Foldout:
                    allTypes.Add(typeof(FoldoutGroupAttribute));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var header = engine.Render(CsHeaderTemplate, new
            {
                Usings = allTypes.Select(t => t.Namespace).Distinct().ToArray()
            });

            var fields = binders.Select(b =>
            {
                var cfg = b.Config.EditorConfig;

                var access = cfg.BindAccess switch
                {
                    ViewBindAccess.Public => "public",
                    ViewBindAccess.Protected => "protected",
                    ViewBindAccess.Private => "private",
                    _ => throw new ArgumentOutOfRangeException()
                };
                return new
                {
                    Access = access,
                    Type = cfg.GetBindType().Name,
                    Name = b.GetBindName(),
                    Comment = cfg.GetComment()
                };
            }).ToArray();

            var fieldStatements = new List<string>();

            int i = 0;
            foreach (var field in fields)
            {
                var statement = "";
                if (field.Comment.IsNotNullOrEmpty())
                {
                    statement += field.Comment + '\n';
                }

                var attributes = new List<string>
                {
                    "SerializeField",
                    $"PropertyOrder({100000 + i}f)",
                    "AutoBinding"
                };
                switch (_cfg.BindersGroupType)
                {
                    case ViewControllerBindersGroupType.None:
                        break;
                    case ViewControllerBindersGroupType.Title:
                        attributes.Add($"TitleGroup(\"{_cfg.BindersGroupName}\")");
                        break;
                    case ViewControllerBindersGroupType.Foldout:
                        attributes.Add($"FoldoutGroup(\"{_cfg.BindersGroupName}\")");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                statement += $"[{string.Join(", ", attributes)}]\n{field.Access} {field.Type} {field.Name};";
                fieldStatements.Add(statement);

                ++i;
            }

            var bodyData = new
            {
                ClassName = _controller.GetScriptName(),
                Fields = AddIndent(string.Join("\n\n", fieldStatements))
            };

            var body = engine.Render(CsDesignerBodyTemplate, bodyData);

            var code = CombineCode(engine, header, body, _cfg.Namespace);
            code = CsDesignerHeaderStatement + code;
            File.WriteAllText(path, code);
        }

        private static string CombineCode(TemplateEngine engine, string header, string body, string @namespace)
        {
            string code;
            @namespace ??= string.Empty;
            @namespace = @namespace.Trim();

            if (@namespace.IsNotNullOrWhiteSpace())
            {
                code = engine.Render(CsNamespaceTemplate, new
                {
                    Header = header,
                    Namespace = @namespace,
                    Body = AddIndent(body)
                });
            }
            else
            {
                code = engine.Render(CsTemplate, new
                {
                    Header = header,
                    Body = body
                });
            }

            return code.Replace("\r\n", "\n").Replace("\n", "\r\n");
        }

        private static string AddIndent(string text)
        {
            if (text.IsNullOrWhiteSpace())
                return string.Empty;
            var splits = text.Split('\n').Select(s => "    " + s);
            return string.Join("\n", splits);
        }
    }
}
