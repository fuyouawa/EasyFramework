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
            var sizeTag = new SizeTag(value == null ? 0 : (uint)value.Length);
            archive.Process(ref sizeTag);

            if (archive.ArchiveIoType == ArchiveIoTypes.Output)
            {
                if (value == null)
                    return;

                foreach (var item in value)
                {
                    var i = item;
                    Serializer.Process(ref i, archive);
                }
            }
            else
            {
                var total = new List<T>();
                for (int i = 0; i < sizeTag.Size; i++)
                {
                    T item = default;
                    Serializer.Process(ref item, archive);
                    total.Add(item);
                }

                value = total.ToArray();
            }
        }
    }
}
