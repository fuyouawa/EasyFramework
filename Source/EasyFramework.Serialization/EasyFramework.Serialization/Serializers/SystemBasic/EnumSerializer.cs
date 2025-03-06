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
            if (archive.ArchiveType != ArchiveTypes.Binary)
            {
                var str = string.Empty;
                if (archive.ArchiveIoType == ArchiveIoTypes.Output)
                    str = Enum.GetName(typeof(T), value);
                archive.Process(ref str);
                if (archive.ArchiveIoType == ArchiveIoTypes.Input)
                    value = Enum.Parse<T>(str);
            }
            else
            {
                int val = 0;
                if (archive.ArchiveIoType == ArchiveIoTypes.Output)
                    val = Convert.ToInt32(value);
                archive.Process(ref val);
                if (archive.ArchiveIoType == ArchiveIoTypes.Input)
                    value = (T)(object)val;
            }
        }
    }
}
