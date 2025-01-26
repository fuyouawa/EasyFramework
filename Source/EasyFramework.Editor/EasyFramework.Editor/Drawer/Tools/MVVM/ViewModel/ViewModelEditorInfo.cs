using System;
using EasyFramework.Editor.Drawer;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

[assembly: RegisterFormatter(typeof(ViewModelEditorInfoFormatter))]

namespace EasyFramework.Editor.Drawer
{
    [Serializable]
    public class ViewModelEditorInfo
    {
        public bool ClassNameSameAsGameObjectName = true;
        public bool IsInitialized;

        public string Namespace;
        public string ClassName;

        public string GenerateDirectory;
        public Type BaseClass;

        public Type GetClassType()
        {
            return ReflectionUtility.FindType(Namespace, ClassName);
        }
    }

    public class ViewModelEditorInfoFormatter : MinimalBaseFormatter<ViewModelEditorInfo>
    {
        private static readonly Serializer<string> StringSerializer = Serializer.Get<string>();
        private static readonly Serializer<bool> BoolSerializer = Serializer.Get<bool>();

        protected override void Read(ref ViewModelEditorInfo value, IDataReader reader)
        {
            value.Namespace = StringSerializer.ReadValue(reader);
            value.ClassName = StringSerializer.ReadValue(reader);
            
            value.ClassNameSameAsGameObjectName = BoolSerializer.ReadValue(reader);
            value.IsInitialized = BoolSerializer.ReadValue(reader);

            var classType = value.GetClassType();
            if (classType == null)
            {
                value.GenerateDirectory = StringSerializer.ReadValue(reader);
                var baseClassName = StringSerializer.ReadValue(reader);
                if (baseClassName.IsNotNullOrEmpty())
                {
                    value.BaseClass = Type.GetType(baseClassName);
                }
            }
            else
            {
                var script = classType.GetMonoScript();
                Debug.Assert(script != null);
                var path = AssetDatabase.GetAssetPath(script);
                path = path[(path.IndexOf('/') + 1)..];
                var i = path.LastIndexOf('/');
                path = i == -1
                    ? string.Empty
                    : path[..i];
                value.GenerateDirectory = path;
                value.BaseClass = classType.BaseType;
            }
        }

        protected override void Write(ref ViewModelEditorInfo value, IDataWriter writer)
        {
            var classType = value.GetClassType();

            StringSerializer.WriteValue(value.Namespace, writer);
            StringSerializer.WriteValue(value.ClassName, writer);

            BoolSerializer.WriteValue(value.ClassNameSameAsGameObjectName, writer);
            BoolSerializer.WriteValue(value.IsInitialized, writer);

            if (classType == null)
            {
                StringSerializer.WriteValue(value.GenerateDirectory, writer);
                StringSerializer.WriteValue(value.BaseClass?.AssemblyQualifiedName, writer);
            }
        }
    }
}
