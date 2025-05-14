using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyFramework.Serialization
{
    [EasySerializerConfig(EasySerializerProiority.SystemBasic)]
    public class ArraySerializer<T> : EasySerializer<T[]>
    {
        private static readonly EasySerializer<T> Serializer = GetSerializer<T>();

        public override void Process(string name, ref T[] value, IArchive archive)
        {
            archive.SetNextName(name);
            archive.StartNode();

            var sizeTag = new SizeTag(value == null ? 0 : (uint)value.Length);
            archive.Process(ref sizeTag);

            if (archive.ArchiveIoType == ArchiveIoType.Output)
            {
                if (value == null)
                    return;

                foreach (var item in value)
                {
                    var tmp = item;
                    Serializer.Process(ref tmp, archive);
                }
            }
            else
            {
                var total = new T[sizeTag.Size];
                for (int i = 0; i < sizeTag.Size; i++)
                {
                    T item = default;
                    Serializer.Process(ref item, archive);
                    total[i] = item;
                }

                value = total;
            }
            archive.FinishNode();
        }
    }
}
