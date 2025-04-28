using System;
using System.Diagnostics;
using System.Reflection;

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
            
            // var isNode = IsNode(typeof(T));
            // if (!IsRoot)
            // {
            //     if (isNode)
            //     {
            //         archive.StartNode();
            //     }
            // }

            var members = EasySerialize.CurrentSettings.MembersGetter(typeof(T));

            foreach (var member in members)
            {
                var memberType = ReflectionUtility.GetMemberType(member);

                var isMemberNode = IsNode(memberType);
                if (isMemberNode)
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
                if (isMemberNode)
                    serializer.Process(ref obj, memberType, archive);
                else
                    serializer.Process(member.Name, ref obj, memberType, archive);

                if (archive.ArchiveIoType == ArchiveIoTypes.Input)
                {
                    ReflectionUtility.SetMemberValue(member, value, obj);
                }

                if (isMemberNode)
                {
                    archive.FinishNode();
                }
            }

            // if (!IsRoot)
            // {
            //     if (isNode)
            //     {
            //         archive.FinishNode();
            //     }
            // }
        }

        private static bool IsNode(Type type)
        {
            return (type.IsClass && type != typeof(string)) ||
                   (type.IsValueType && !type.IsPrimitive && !type.IsEnum);
        }
    }
}
