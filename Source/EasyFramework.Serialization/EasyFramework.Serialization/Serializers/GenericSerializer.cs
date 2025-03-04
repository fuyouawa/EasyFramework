using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using EasyFramework.Serialization;

namespace EasyFramework.Serialization
{
    [EasySerializerConfig(EasySerializerProiority.Generic)]
    public class GenericSerializer<T> : EasySerializer<T>
    {
        public override void Process(string name, ref T value, IArchive archive)
        {
            Debug.Assert(!typeof(T).IsSubclassOf(typeof(UnityEngine.Object)));

            if (value == null)
            {
                value = Activator.CreateInstance<T>();
            }

            var serializeFields = EasySerializationUtility.GetSerializableFields(typeof(T));

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

                object obj = null;
                if (archive.ArchiveIoType == ArchiveIoTypes.Output)
                {
                    obj = field.GetValue(value);
                }
                
                var serializer = EasySerializationUtility.GetSerializer(fieldType);
                if (isNode)
                    serializer.Process(ref obj, fieldType, archive);
                else
                    serializer.Process(field.Name, ref obj, fieldType, archive);

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
