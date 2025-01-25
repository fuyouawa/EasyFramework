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
        public bool IsInitialized;

        public string Comment;
        public string Name;

        public ViewBindAccess Access;

        public string GetName()
        {
            if (Name.IsNullOrWhiteSpace())
                return Name;

            if (!AutoNamingNotations)
                return Name;

            //TODO 更多情况的处理
            if (Access == ViewBindAccess.Public)
            {
                return char.ToUpper(Name[0]) + Name[1..];
            }

            var name = char.ToLower(Name[0]) + Name[1..];
            return '_' + name;
        }
    }

    public class ViewBinderEditorInfoFormatter : MinimalBaseFormatter<ViewBinderEditorInfo>
    {
        private static readonly Serializer<string> StringSerializer = Serializer.Get<string>();
        private static readonly Serializer<bool> BoolSerializer = Serializer.Get<bool>();
        private static readonly Serializer<int> IntSerializer = Serializer.Get<int>();

        protected override void Read(ref ViewBinderEditorInfo value, IDataReader reader)
        {
            value.AutoAddCommentPara = BoolSerializer.ReadValue(reader);
            value.AutoNamingNotations = BoolSerializer.ReadValue(reader);
            value.NameSameAsGameObjectName = BoolSerializer.ReadValue(reader);
            value.IsInitialized = BoolSerializer.ReadValue(reader);

            value.Comment = StringSerializer.ReadValue(reader);
            value.Name = StringSerializer.ReadValue(reader);

            value.Access = (ViewBindAccess)IntSerializer.ReadValue(reader);
        }

        protected override void Write(ref ViewBinderEditorInfo value, IDataWriter writer)
        {
            BoolSerializer.WriteValue(value.AutoAddCommentPara, writer);
            BoolSerializer.WriteValue(value.AutoNamingNotations, writer);
            BoolSerializer.WriteValue(value.NameSameAsGameObjectName, writer);
            BoolSerializer.WriteValue(value.IsInitialized, writer);

            StringSerializer.WriteValue(value.Comment, writer);
            StringSerializer.WriteValue(value.Name, writer);

            IntSerializer.WriteValue((int)value.Access, writer);
        }
    }
}
