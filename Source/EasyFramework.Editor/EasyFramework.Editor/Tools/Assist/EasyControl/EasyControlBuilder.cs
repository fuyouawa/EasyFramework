using System.CodeDom.Compiler;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Editor
{
    internal class EasyControlBuilder
    {
        private EasyControlEditorArgs _args;
        private Component _comp;

        public void Setup(EasyControlEditorArgs args, Component comp)
        {
            _args = args;
            _comp = comp;
        }

        public void Build()
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

            var children = EasyControlHelper.GetChildren(_comp.gameObject);

            var data = new
            {
                Usings = new[] { "EasyFramework", "UnityEngine" }
                    .Concat(children
                        .Select(c => EasyControlHelper.GetArgs(c).Bounder.TypeToBind.Value.Namespace))
                    .Distinct(),
                Namespace = _args.ViewModel.Namespace,
                ClassName = _args.ViewModel.ClassName,
                Children = children.Select(c =>
                {
                    var args = EasyControlHelper.GetArgs(c);
                    return new
                    {
                        Type = args.Bounder.TypeToBind.Value.Name,
                        Name = args.Bounder.GetName(),
                        OriginName = args.Bounder.Name,
                        CommentSplits = EasyControlHelper.GetBounderCommentSplits(args),
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
                        field.Comments.Add(new CodeCommentStatement(split, true));
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

    }
}
