using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public class ViewControllerBuilder
    {
        private readonly ViewControllerEditorConfig _cfg;
        private readonly Component _component;
        private readonly IViewController _controller;

        public ViewControllerBuilder(IViewController controller)
        {
            _controller = controller;
            _component = (Component)controller;
            _cfg = _controller.Config.EditorConfig;
        }

        public void Build()
        {
            var dir = Path.Combine(Application.dataPath, _cfg.GenerateDir);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var path = Path.Combine(dir, _cfg.ScriptName);
            var designerPath = path + ".Designer.cs";
            path += ".cs";

            BuildCs(path);
            // BuildCsDesigner(designerPath);
            AssetDatabase.Refresh();
        }


        public bool Check()
        {
            string error = ViewControllerEditorUtility.GetIdentifierError("类名", _cfg.ScriptName);
            if (error.IsNotNullOrEmpty())
            {
                EditorUtility.DisplayDialog("错误", $"类名不规范：{error}", "确认");
                return false;
            }

            var binders = ViewControllerUtility.GetAllBinders(_controller);

            var nameCheck = new HashSet<string>();
            foreach (var binder in binders)
            {
                var comp = (Component)binder;
                var bindName = ViewBinderEditorUtility.GetBindName(binder);

                error = ViewControllerEditorUtility.GetIdentifierError("变量名称", bindName);
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

        private static string CsTemplate = @"
{% for using in Usings %}
using {{ using }};
{% endfor %}

public class {{ ClassName }} : {{ BaseClassName }}
{
    {{ classTemplate }}
}
";

        private void BuildCs(string path)
        {
            // if (File.Exists(path)) return;
        
            var data = new
            {
                Usings = new List<string>
                {
                    "System",
                    "System.Collections.Generic",
                    "UnityEngine",
                    _cfg.BaseClass.Namespace
                }.Distinct(),
                ClassName = _cfg.ScriptName,
                Namespace = _cfg.Namespace,
                BaseClassName = _cfg.BaseClass.Name
            };

            var templateEngine = new TemplateEngine();
            var code = templateEngine.Render(CsTemplate, data);
            Debug.Log(code);
        }
        //
        // private string ProcessCodeSnippet(string snippet)
        // {
        //     var indent = _cfg.Namespace.IsNullOrWhiteSpace() ? "\t" : "\t\t";
        //     var splits = snippet.Split('\n').Select(s => indent + s);
        //     return string.Join("\r\n", splits);
        // }
        //
        // private void BuildCsDesigner(string path)
        // {
        //     var children = ViewModelHelper.GetChildren(_component.transform);
        //
        //     var data = new
        //     {
        //         Usings = new[] { "EasyFramework", "UnityEngine" }
        //             .Concat(children.Select(c => c.GetBindType().Namespace))
        //             .Distinct(),
        //         Namespace = _cfg.Namespace,
        //         ClassName = _cfg.ClassName,
        //         Children = children.Select(c =>
        //         {
        //             var binderEditorInfo = c.Info.EditorData.Get<ViewBinderEditorInfo>()!;
        //             var comp = (Component)c;
        //
        //             return new
        //             {
        //                 Type = c.GetBindType(),
        //                 Name = c.GetBindName(),
        //                 GameObjectName = comp.gameObject.name,
        //                 CommentSplits = ViewBinderHelper.GetCommentSplits(binderEditorInfo),
        //                 Access = binderEditorInfo.Access,
        //             };
        //         })
        //     };
        //     
        //     var compileUnit = new CodeCompileUnit();
        //
        //     compileUnit.Namespaces.Add(new CodeNamespace());
        //     compileUnit.UserData["Usings"] = new CodeNamespace();
        //     var usings = new CodeNamespace();
        //     foreach (var u in data.Usings)
        //     {
        //         usings.Imports.Add(new CodeNamespaceImport(u));
        //     }
        //     compileUnit.Namespaces.Add(usings);
        //
        //     var codeNamespace = new CodeNamespace(data.Namespace);
        //     compileUnit.Namespaces.Add(codeNamespace);
        //
        //     var codeClass = new CodeTypeDeclaration(data.ClassName)
        //     {
        //         IsPartial = true,
        //         TypeAttributes = TypeAttributes.Public,
        //         BaseTypes = { nameof(IViewController) }
        //     };
        //
        //     foreach (var child in data.Children)
        //     {
        //         var field = new CodeMemberField
        //         {
        //             Name = child.Name,
        //             Type = new CodeTypeReference(child.Type.Name),
        //             Attributes = child.Access == ViewBindAccess.Public
        //                 ? MemberAttributes.Public
        //                 : MemberAttributes.Private
        //         };
        //
        //         if (child.Access == ViewBindAccess.PrivateWithSerializeFieldAttribute)
        //         {
        //             field.CustomAttributes.Add(new CodeAttributeDeclaration("SerializeField"));
        //         }
        //
        //         field.CustomAttributes.Add(new CodeAttributeDeclaration("FromViewBinder"));
        //
        //         if (child.CommentSplits.IsNotNullOrEmpty())
        //         {
        //             foreach (var split in child.CommentSplits)
        //             {
        //                 field.Comments.Add(new CodeCommentStatement(split, true));
        //             }
        //         }
        //
        //         codeClass.Members.Add(field);
        //     }
        //
        //     var infoField = new CodeMemberField(nameof(ViewModelInfo), "_viewModelInfo")
        //     {
        //         Attributes = MemberAttributes.Private
        //     };
        //     infoField.CustomAttributes.Add(
        //         new CodeAttributeDeclaration("SerializeField"));
        //     codeClass.Members.Add(infoField);
        //
        //     var infoProperty = new CodeMemberProperty
        //     {
        //         Name = "Info",
        //         Type = new CodeTypeReference(nameof(ViewModelInfo)),
        //         Attributes = MemberAttributes.Public | MemberAttributes.Final,
        //         HasGet = true,
        //         HasSet = true
        //     };
        //
        //     infoProperty.GetStatements.Add(
        //         new CodeMethodReturnStatement(
        //             new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_viewModelInfo")
        //         )
        //     );
        //
        //     infoProperty.SetStatements.Add(
        //         new CodeAssignStatement(
        //             new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_viewModelInfo"),
        //             new CodePropertySetValueReferenceExpression()
        //         )
        //     );
        //
        //     infoProperty.ImplementationTypes.Add(new CodeTypeReference(nameof(IViewController)));
        //     codeClass.Members.Add(infoProperty);
        //
        //     // Add class to namespace
        //     codeNamespace.Types.Add(codeClass);
        //
        //     using var writer = new StringWriter();
        //     var provider = CodeDomProvider.CreateProvider("CSharp");
        //     var options = new CodeGeneratorOptions
        //     {
        //         BracingStyle = "C",
        //         IndentString = "    "
        //     };
        //
        //     provider.GenerateCodeFromCompileUnit(compileUnit, writer, options);
        //
        //     var result = writer.ToString();
        //     File.WriteAllText(path, result);
        // }
    }
}
