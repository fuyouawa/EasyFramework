using System;

namespace EasyFramework.Serialization
{
    [EasySerializerConfig(EasySerializerProiority.SystemBasic)]
    public class TypeSerializer : EasySerializer<Type>
    {
        public override void Process(string name, ref Type value, IArchive archive)
        {
            archive.SetNextName(name);

            string typeName = null;
            var archiveIoType = archive.ArchiveIoType;

            if (archiveIoType == ArchiveIoType.Output)
                typeName = TypeToName(value);

            archive.Process(ref typeName);

            if (archiveIoType == ArchiveIoType.Input)
                value = NameToType(typeName);
        }

        private static string TypeToName(Type type)
        {
            if (type == null)
            {
                return string.Empty;
            }
            return type.FullName + ", " + type.Assembly.GetName().Name;
        }

        private static Type NameToType(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            return Type.GetType(name);
        }
    }
}
