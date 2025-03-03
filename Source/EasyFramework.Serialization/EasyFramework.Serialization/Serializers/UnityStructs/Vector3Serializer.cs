using UnityEngine;

namespace EasyFramework.Serialization
{
    [EasySerializerConfig(EasySerializerProiority.UnityStructs)]
    public class Vector3Serializer : EasySerializer<Vector3>
    {
        private static readonly EasySerializer<float> FloatSerializer = GetSerializer<float>();
        public override void Process(string name, ref Vector3 value, IArchive archive)
        {
            FloatSerializer.Process("x", ref value.x, archive);
            FloatSerializer.Process("y", ref value.y, archive);
            FloatSerializer.Process("z", ref value.z, archive);
        }
    }
}
