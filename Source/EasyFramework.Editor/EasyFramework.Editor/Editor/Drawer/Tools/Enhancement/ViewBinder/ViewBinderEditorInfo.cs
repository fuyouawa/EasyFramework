using System;
using EasyFramework.Editor.Drawer;
using Sirenix.Serialization;
using UnityEngine;

[assembly: RegisterFormatter(typeof(ViewBinderEditorInfoFormatter))]

namespace EasyFramework.Editor.Drawer
{
    public enum ViewBindAccess
    {
        Public,
        PrivateWithSerializeFieldAttribute
    }

    public class ViewBinderEditorInfo
    {
        public bool AutoAddCommentPara = true;
        public bool AutoNamingNotations = true;
        public bool NameSameAsGameObjectName = true;
        public bool BindGameObject;
        public bool IsInitialized;

        public string Comment;
        public string Name;
        
        public ViewBindAccess Access;
        public Type BindType;

        public Component BindComponent;
    }

    public class ViewBinderEditorInfoFormatter : MinimalBaseFormatter<ViewBinderEditorInfo>
    {
        private static readonly Serializer<string> StringSerializer = Serializer.Get<string>();
        private static readonly Serializer<bool> BoolSerializer = Serializer.Get<bool>();
        private static readonly Serializer<int> IntSerializer = Serializer.Get<int>();
        private static readonly Serializer<Component> ComponentSerializer = Serializer.Get<Component>();

        protected override void Read(ref ViewBinderEditorInfo value, IDataReader reader)
        {
            value.AutoAddCommentPara = BoolSerializer.ReadValue(reader);
            value.AutoNamingNotations = BoolSerializer.ReadValue(reader);
            value.NameSameAsGameObjectName = BoolSerializer.ReadValue(reader);
            value.BindGameObject = BoolSerializer.ReadValue(reader);
            value.IsInitialized = BoolSerializer.ReadValue(reader);

            value.Comment = StringSerializer.ReadValue(reader);

            if (!value.NameSameAsGameObjectName)
            {
                value.Name = StringSerializer.ReadValue(reader);
            }

            value.Access = (ViewBindAccess)IntSerializer.ReadValue(reader);

            var name = StringSerializer.ReadValue(reader);
            if (name.IsNotNullOrEmpty())
            {
                value.BindType = Type.GetType(name);
            }

            value.BindComponent = ComponentSerializer.ReadValue(reader);
        }

        protected override void Write(ref ViewBinderEditorInfo value, IDataWriter writer)
        {
            BoolSerializer.WriteValue(value.AutoAddCommentPara, writer);
            BoolSerializer.WriteValue(value.AutoNamingNotations, writer);
            BoolSerializer.WriteValue(value.NameSameAsGameObjectName, writer);
            BoolSerializer.WriteValue(value.BindGameObject, writer);
            BoolSerializer.WriteValue(value.IsInitialized, writer);

            StringSerializer.WriteValue(value.Comment, writer);

            if (!value.NameSameAsGameObjectName)
            {
                StringSerializer.WriteValue(value.Name, writer);
            }

            IntSerializer.WriteValue((int)value.Access, writer);
            StringSerializer.WriteValue(value.BindType?.AssemblyQualifiedName, writer);
            ComponentSerializer.WriteValue(value.BindComponent, writer);
        }
    }
}
