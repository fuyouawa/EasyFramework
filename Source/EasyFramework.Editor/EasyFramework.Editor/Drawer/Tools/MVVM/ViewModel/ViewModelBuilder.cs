using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Editor.Drawer
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
            var dir = Path.Combine(Application.dataPath, _editorInfo.GenerateDirectory);
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


        public bool Check()
        {
            string error = ViewModelHelper.GetIdentifierError("类名", _editorInfo.ClassName);
            if (error.IsNotNullOrEmpty())
            {
                EditorUtility.DisplayDialog("错误", $"类名不规范：{error}", "确认");
                return false;
            }

            var children = ViewModelHelper.GetChildren(_component.transform);

            var nameCheck = new HashSet<string>();
            foreach (var child in children)
            {
                var comp = (Component)child;
                var bindName = child.GetBindName();

                error = ViewModelHelper.GetIdentifierError("变量名称", bindName);
                if (error.IsNotNullOrEmpty())
                {
                    EditorUtility.DisplayDialog("错误", $"绑定“{comp.gameObject.name}”出现错误：{error}", "确认");
                    return false;
                }

                if (!nameCheck.Add(bindName))
                {
                    EditorUtility.DisplayDialog("错误",
                        $"绑定“{comp.gameObject.name}”出现错误：重复的变量名称（{bindName}）", "确认");
                    return false;
                }
            }

            return true;
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
                    _editorInfo.BaseClass.Namespace
                }.Distinct(),
                ClassName = _editorInfo.ClassName,
                Namespace = _editorInfo.Namespace,
                BaseClassName = _editorInfo.BaseClass.Name
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

            var classTemplate = ViewModelSettings.Instance.AutoIndent
                ? ProcessCodeSnippet(ViewModelSettings.Instance.ClassTemplate)
                : ViewModelSettings.Instance.ClassTemplate;

            codeClass.Members.Add(new CodeSnippetTypeMember(classTemplate));

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

        private string ProcessCodeSnippet(string snippet)
        {
            var indent = _editorInfo.Namespace.IsNullOrWhiteSpace() ? "\t" : "\t\t";
            var splits = snippet.Split('\n').Select(s => indent + s);
            return string.Join("\r\n", splits);
        }

        private void BuildCsDesigner(string path)
        {
            var children = _component.GetComponentsInChildren<IViewBinder>();

            var data = new
            {
                Usings = new[] { "EasyFramework", "UnityEngine" }
                    .Concat(children
                        .Select(c => c.GetBindObject().GetType().Namespace))
                    .Distinct(),
                Namespace = _editorInfo.Namespace,
                ClassName = _editorInfo.ClassName,
                Children = children.Select(c =>
                {
                    var binderEditorInfo = c.Info.EditorData.Get<ViewBinderEditorInfo>()!;
                    var comp = (Component)c;
                    
                    return new
                    {
                        Type = c.GetBindType(),
                        Name = c.GetBindName(),
                        GameObjectName = comp.gameObject.name,
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

                field.CustomAttributes.Add(new CodeAttributeDeclaration("FromViewBinder"));

                if (child.CommentSplits.IsNotNullOrEmpty())
                {
                    foreach (var split in child.CommentSplits)
                    {
                        field.Comments.Add(new CodeCommentStatement(split, true));
                    }
                }

                codeClass.Members.Add(field);
            }

            var infoField = new CodeMemberField(nameof(ViewModelInfo), "_viewModelInfo")
            {
                Attributes = MemberAttributes.Private
            };
            infoField.CustomAttributes.Add(
                new CodeAttributeDeclaration("SerializeField"));
            codeClass.Members.Add(infoField);

            var infoProperty = new CodeMemberProperty
            {
                Name = "Info",
                Type = new CodeTypeReference(nameof(ViewModelInfo)),
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                HasGet = true,
                HasSet = true
            };

            infoProperty.GetStatements.Add(
                new CodeMethodReturnStatement(
                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_viewModelInfo")
                )
            );

            infoProperty.SetStatements.Add(
                new CodeAssignStatement(
                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_viewModelInfo"),
                    new CodePropertySetValueReferenceExpression()
                )
            );

            infoProperty.ImplementationTypes.Add(new CodeTypeReference(nameof(IViewModel)));
            codeClass.Members.Add(infoProperty);

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
