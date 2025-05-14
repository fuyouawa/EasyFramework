using System;

namespace EasyFramework.Serialization
{
    [EasySerializerConfig(EasySerializerProiority.SystemBasic)]
    public class EnumSerializer<T> : EasySerializer<T>
        where T : struct, Enum
    {
        public override void Process(string name, ref T value, IArchive archive)
        {
            archive.SetNextName(name);

            var archiveIoType = archive.ArchiveIoType;

            if (archive.ArchiveType != ArchiveTypes.Binary)
            {
                var str = string.Empty;
                if (archiveIoType == ArchiveIoType.Output)
                    str = Enum.GetName(typeof(T), value);
                archive.Process(ref str);
                if (archiveIoType == ArchiveIoType.Input)
                    value = Enum.Parse<T>(str);
            }
            else
            {
                int val = 0;
                if (archiveIoType == ArchiveIoType.Output)
                    val = Convert.ToInt32(value);
                archive.Process(ref val);
                if (archiveIoType == ArchiveIoType.Input)
                    value = (T)(object)val;
            }
        }
    }
}
