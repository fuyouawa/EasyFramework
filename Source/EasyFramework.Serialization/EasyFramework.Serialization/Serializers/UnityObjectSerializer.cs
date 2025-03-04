using System;
using Object = UnityEngine.Object;

namespace EasyFramework.Serialization
{
    [EasySerializerConfig(EasySerializerProiority.UnityObject)]
    public class UnityObjectSerializer<T> : EasySerializer<T>
        where T : Object
    {
        public override void Process(string name, ref T value, IArchive archive)
        {
            if (IsRoot)
            {
                throw new NotImplementedException();
            }

            Object unityObject = value;
            
            archive.SetNextName(name);
            archive.Process(ref unityObject);

            if (archive.ArchiveIoType == ArchiveIoTypes.Input)
            {
                value = unityObject as T;
            }
        }
    }
}
