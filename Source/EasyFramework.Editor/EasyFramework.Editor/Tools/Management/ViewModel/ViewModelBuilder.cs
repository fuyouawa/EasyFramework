using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Editor
{
    public class ViewModelBuilder
    {
        private ViewModelEditorInfo _editorInfo;
        private Component _component;

        public void Setup(ViewModelEditorInfo args, Component comp)
        {
            _editorInfo = args;
            _component = comp;
        }

        public void Build()
        {
            var dir = Path.Combine(Application.dataPath, _editorInfo.GenerateDir);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var path = Path.Combine(dir, _editorInfo.ClassName);
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
                    _editorInfo.BaseClass.Value.Namespace
                }.Distinct(),
                ClassName = _editorInfo.ClassName,
                Namespace = _editorInfo.Namespace,
                BaseClassName = _editorInfo.BaseClass.Value.Name
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
            var children = _component.GetComponentsInChildren<IViewBinder>();

            var data = new
            {
                Usings = new[] { "EasyFramework", "UnityEngine" }
                    .Concat(children
                        .Select(c => c.Info.BindComponent.GetType().Namespace))
                    .Distinct(),
                Namespace = _editorInfo.Namespace,
                ClassName = _editorInfo.ClassName,
                Children = children.Select(c =>
                {
                    var binderEditorInfo = c.Info.EditorData.Get<ViewBinderEditorInfo>();
                    return new
                    {
                        Type = c.Info.BindComponent.GetType().Name,
                        Name = binderEditorInfo.GetName(),
                        OriginName = binderEditorInfo.Name,
                        CommentSplits = ViewBinderHelper.GetCommentSplits(binderEditorInfo),
                        Access = binderEditorInfo.Access,
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
                TypeAttributes = TypeAttributes.Public,
                BaseTypes = { nameof(IViewModel) }
            };

            foreach (var child in data.Children)
            {
                var field = new CodeMemberField
                {
                    Name = child.Name,
                    Type = new CodeTypeReference(child.Type),
                    Attributes = child.Access == ViewBindAccess.Public
                        ? MemberAttributes.Public
                        : MemberAttributes.Private
                };

                if (child.Access == ViewBindAccess.PrivateWithSerializeFieldAttribute)
                {
                    field.CustomAttributes.Add(new CodeAttributeDeclaration("SerializeField"));
                }

                field.CustomAttributes.Add(new CodeAttributeDeclaration(
                    "ByViewBinder",
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

            //TODO 代码优化
            if (data.Namespace.IsNotNullOrWhiteSpace())
            {
                codeClass.Members.Add(new CodeSnippetTypeMember(@"
        [SerializeField] private ViewModelInfo _viewModelInfo;

        ViewModelInfo IViewModel.Info
        {
            get => _viewModelInfo;
            set => _viewModelInfo = value;
        }"
                ));
            }
            else
            {
                codeClass.Members.Add(new CodeSnippetTypeMember(@"
    [SerializeField] private ViewModelInfo _viewModelInfo;

    ViewModelInfo IViewModel.Info
    {
        get => _viewModelInfo;
        set => _viewModelInfo = value;
    }"
                ));
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
