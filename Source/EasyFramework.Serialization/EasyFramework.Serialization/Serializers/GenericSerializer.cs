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

            var members = EasySerialize.CurrentSettings.MembersGetterOfGeneric(typeof(T));

            foreach (var member in members)
            {
                var memberType = ReflectionUtility.GetMemberType(member);

                var isNode = (memberType.IsClass && memberType != typeof(string)) ||
                             (memberType.IsValueType && !memberType.IsPrimitive && !memberType.IsEnum);
                if (isNode)
                {
                    archive.SetNextName(member.Name);
                    archive.StartNode();
                }

                object obj = null;
                if (archive.ArchiveIoType == ArchiveIoTypes.Output)
                {
                    obj = ReflectionUtility.GetMemberValue(member, value);
                }
                
                var serializer = EasySerializationUtility.GetSerializer(memberType);
                if (isNode)
                    serializer.Process(ref obj, memberType, archive);
                else
                    serializer.Process(member.Name, ref obj, memberType, archive);

                if (archive.ArchiveIoType == ArchiveIoTypes.Input)
                {
                    ReflectionUtility.SetMemberValue(member, value, obj);
                }

                if (isNode)
                {
                    archive.FinishNode();
                }
            }
        }
    }
}
