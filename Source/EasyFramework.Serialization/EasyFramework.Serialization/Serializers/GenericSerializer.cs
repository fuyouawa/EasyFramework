using System;
using System.Linq;
using System.Reflection;
using EasyFramework.Serialization;
using UnityEngine;

namespace EasyFramework.Serialization
{
    [EasySerializerConfig(EasySerializerProiority.GenericCore)]
    public class GenericSerializer<T> : EasySerializer<T>
    {
        public override void Process(string name, ref T value, IArchive archive)
        {
            if (value == null)
            {
                value = Activator.CreateInstance<T>();
            }

            var serializeFields = typeof(T)
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(f => f.IsPublic || f.GetCustomAttribute<SerializeField>() != null)
                .ToArray();

            foreach (var field in serializeFields)
            {
                var fieldType = field.FieldType;

                var isNode = (fieldType.IsClass && fieldType != typeof(string)) ||
                             (fieldType.IsValueType && !fieldType.IsPrimitive && !fieldType.IsEnum);
                if (isNode)
                {
                    archive.SetNextName(field.Name);
                    archive.StartNode();
                }

                var serializer = EasySerializerUtility.GetSerializer(fieldType);

                object obj = null;
                if (archive.ArchiveIoType == ArchiveIoTypes.Output)
                {
                    obj = field.GetValue(value);
                }

                if (isNode)
                {
                    serializer.Process(ref obj, fieldType, archive);
                }
                else
                {
                    serializer.Process(field.Name, ref obj, fieldType, archive);
                }

                if (archive.ArchiveIoType == ArchiveIoTypes.Input)
                {
                    field.SetValue(value, obj);
                }

                if (isNode)
                {
                    archive.FinishNode();
                }
            }
        }
    }
}
