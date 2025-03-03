using System;
using System.Linq;
using System.Reflection;
using EasyFramework.Serialization;
using UnityEngine;

namespace EasyFramework.Serialization
{
    [EasySerializerPriority(EasySerializerProiority.Generic)]
    public class GenericSerializer<T> : EasySerializerBase<T>
    {
        protected override void Process(IArchive archive, ref T value)
        {
            if (value == null)
            {
                value = Activator.CreateInstance<T>();
            }

            var serializeFields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(f => f.IsPublic || f.GetCustomAttribute<SerializeField>() != null)
                .ToArray();

            foreach (var field in serializeFields)
            {
                archive.SetNextName(field.Name);
                var isNode = field.FieldType.IsClass && field.FieldType != typeof(string);
                if (isNode)
                {
                    archive.StartNode();
                }

                var ser = EasySerializerUtility.GetSerializer(field.FieldType);

                object obj = null;
                if (archive.ArchiveIoType == ArchiveIoTypes.Output)
                {
                    obj = field.GetValue(value);
                }

                EasySerializerUtility.ProcessSerializer(ser, archive, ref obj, field.FieldType);

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
