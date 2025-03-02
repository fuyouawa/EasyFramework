using System.Linq;
using System.Reflection;
using UnityEngine;

namespace EasyFramework.Serialization
{
    public class GenericSerializer<T> : EasySerializerBase<T>
    {
        protected override void Process(IArchive archive, ref T value)
        {
            if (value is Object unityObject)
            {
                archive.Process(ref unityObject);
            }
            else
            {
                var type = typeof(T);
                var serializeFields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .Where(f => f.GetCustomAttribute<SerializeField>() != null)
                    .ToArray();
                foreach (var field in serializeFields)
                {
                    archive.SetNextName(field.Name);
                }
            }
        }
    }
}
