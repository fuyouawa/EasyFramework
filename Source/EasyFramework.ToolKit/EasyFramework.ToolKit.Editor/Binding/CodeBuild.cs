using EasyFramework.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public static class CodeBuild
    {
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
public class {{ ClassName }} : {{ BaseClassName }}, IController
{
{{ BinderFields }}

{{ ClassTemplate }}

{{ Architecture }}
}
".Trim();

        private static readonly string CsBindersFieldCodeBegin = @"
#region 绑定变量
".Trim();

        private static readonly string CsBindersFieldCodeEnd = @"
#endregion
".Trim();

        public static bool CanBuild(Builder builder)
        {
            var dir = Application.dataPath + "/" + builder.GenerateDirectory;
            var path = dir + "/" + builder.GetScriptName() + ".cs";
            return !File.Exists(path);
        }

        public static void Build(Builder builder)
        {
            if (!CanBuild(builder))
                throw new Exception($"The target script '{builder.GetScriptName()}' already exists.");

            var settings = BuilderSettings.Instance;
            var dir = Application.dataPath + "/" + builder.GenerateDirectory;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var scriptName = builder.GetScriptName();
            if (!BindingUtility.IsValidIdentifier(scriptName))
            {
                throw new Exception($"The script name '{scriptName}' is invalid.");
            }

            var path = dir + "/" + scriptName + ".cs";
            var engine = new TemplateEngine();

            var types = new List<Type>()
            {
                typeof(NotImplementedException),
                builder.BaseClass,
                typeof(IController),
                typeof(BindingAttribute),
                builder.ArchitectureType
            };
            switch (builder.BindersGroupType)
            {
                case Builder.GroupType.None:
                    break;
                case Builder.GroupType.Title:
                    types.Add(typeof(TitleGroupAttribute));
                    break;
                case Builder.GroupType.Foldout:
                    types.Add(typeof(FoldoutGroupAttribute));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            types.AddRange(builder.GetOwnedBinders().Select(binder => binder.GetBindType()));

            var header = engine.Render(CsHeaderTemplate, new
            {
                Usings = types
                    .Where(type => type != null && type.Namespace.IsNotNullOrEmpty() && type.Namespace != builder.Namespace)
                    .Select(type => type.Namespace)
                    .Distinct().ToArray()
            });

            var binderFields = GetBinderFieldsCode(builder);

            var template = builder.BuildScriptType switch
            {
                Builder.ScriptType.UIPanel => settings.UIPanelScriptTemplate,
                Builder.ScriptType.Controller => settings.ControllerScriptTemplate,
                _ => throw new ArgumentOutOfRangeException()
            };
            if (settings.AutoIndentTemplate)
            {
                template = AddIndent(template);
            }

            var arch = "IArchitecture IBelongToArchitecture.GetArchitecture() => ";
            arch += builder.ArchitectureType != null
                ? $"{builder.ArchitectureType.Name}.Instance;"
                : "throw new NotImplementedException();";
            arch = AddIndent(arch);

            var body = engine.Render(CsBodyTemplate, new
            {
                ClassName = builder.GetScriptName(),
                BaseClassName = builder.BaseClass.Name,
                ClassTemplate = template,
                BinderFields = AddIndent(binderFields),
                Architecture = arch
            });

            var code = CombineCode(builder, header, body);
            File.WriteAllText(path, code);
            // Debug.Log(code);
            AssetDatabase.Refresh();
        }

        public static string GetIndentedBinderFieldsCode(Builder builder)
        {
            if (builder.Namespace.IsNotNullOrWhiteSpace())
            {
                return AddIndent(AddIndent(GetBinderFieldsCode(builder)));
            }
            return AddIndent(GetBinderFieldsCode(builder));
        }

        private static string GetBinderFieldsCode(Builder builder)
        {
            return CsBindersFieldCodeBegin + "\n\n" + GetBinderFieldsDefine(builder) + "\n\n" +
                   CsBindersFieldCodeEnd;
        }

        private static string CombineCode(Builder builder, string header, string body)
        {
            string code;
            var engine = new TemplateEngine();
            if (builder.Namespace.IsNotNullOrWhiteSpace())
            {
                if (!BindingUtility.IsValidIdentifier(builder.Namespace))
                {
                    throw new Exception($"The namespace '{builder.Namespace}' is invalid.");
                }
                code = engine.Render(CsNamespaceTemplate, new
                {
                    Header = header,
                    Namespace = builder.Namespace.Trim(),
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
            var indent = BuilderSettings.Instance.GetIndent();
            var splits = text.Split('\n').Select(s => indent + s);
            return string.Join("\n", splits);
        }

        private static string GetBinderFieldsDefine(Builder builder)
        {
            var binders = builder.GetOwnedBinders();

            var fields = binders.Select(b =>
            {
                var access = b.BindAccess switch
                {
                    Binder.Access.Public => "public",
                    Binder.Access.Protected => "protected",
                    Binder.Access.Private => "private",
                    _ => throw new ArgumentOutOfRangeException()
                };
                var bindName = b.GetBindName();
                if (!BindingUtility.IsValidIdentifier(bindName))
                {
                    throw new Exception($"The binding field name '{bindName}' is invalid.");
                }
                return new
                {
                    Access = access,
                    Type = b.GetBindType().Name,
                    Name = bindName,
                    Comment = b.GetComment()
                };
            }).ToArray();

            var fieldStatements = new List<string>();
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
                    "Binding"
                };

                switch (builder.BindersGroupType)
                {
                    case Builder.GroupType.None:
                        break;
                    case Builder.GroupType.Title:
                        attributes.Add($"TitleGroup(\"{builder.BindersGroupName}\")");
                        break;
                    case Builder.GroupType.Foldout:
                        attributes.Add($"FoldoutGroup(\"{builder.BindersGroupName}\")");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                statement += $"[{string.Join(", ", attributes)}]\n{field.Access} {field.Type} {field.Name};";
                fieldStatements.Add(statement);
            }

            return string.Join("\n\n", fieldStatements);
        }
    }
}
