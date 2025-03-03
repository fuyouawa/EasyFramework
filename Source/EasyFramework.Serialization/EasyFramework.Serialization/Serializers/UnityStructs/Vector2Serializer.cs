using UnityEngine;

namespace EasyFramework.Serialization
{
    [EasySerializerConfig(EasySerializerProiority.UnityStructs)]
    public class Vector2Serializer : EasySerializer<Vector2>
    {
        private static readonly EasySerializer<float> FloatSerializer = GetSerializer<float>();
        public override void Process(string name, ref Vector2 value, IArchive archive)
        {
            FloatSerializer.Process("x", ref value.x, archive);
            FloatSerializer.Process("y", ref value.y, archive);
        }
    }
}
