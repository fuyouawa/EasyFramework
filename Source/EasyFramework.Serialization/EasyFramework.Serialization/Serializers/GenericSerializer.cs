using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace EasyFramework.Serialization
{
    [EasySerializerConfig(EasySerializerProiority.Generic)]
    public class GenericSerializer<T> : EasySerializer<T>
    {
        private List<MemberWithSerializer> _memberWithSerializersCache;

        public override void Process(string name, ref T value, IArchive archive)
        {
            Debug.Assert(!typeof(T).IsSubclassOf(typeof(UnityEngine.Object)));

            _memberWithSerializersCache ??= Settings.MembersWithSerializerGetter.Get(typeof(T));

            if (value == null)
            {
                value = Activator.CreateInstance<T>();
            }
            
            var isNode = IsNode(typeof(T));
            if (!IsRoot)
            {
                if (isNode)
                {
                    archive.SetNextName(name);
                    archive.StartNode();
                }
            }

            foreach (var memberWithSerializer in _memberWithSerializersCache)
            {
                var memberType = memberWithSerializer.MemberType;
                var member = memberWithSerializer.Member;
                var serializer = memberWithSerializer.Serializer;

                object obj = null;
                if (archive.ArchiveIoType == ArchiveIoTypes.Output)
                {
                    obj = member.GetMemberValue(value);
                }
                
                serializer.Process(member.Name, ref obj, memberType, archive);

                if (archive.ArchiveIoType == ArchiveIoTypes.Input)
                {
                    member.SetMemberValue(value, obj);
                }
            }

            if (!IsRoot)
            {
                if (isNode)
                {
                    archive.FinishNode();
                }
            }
        }

        private static bool IsNode(Type type)
        {
            return (type.IsClass && type != typeof(string)) ||
                   (type.IsValueType && !type.IsPrimitive && !type.IsEnum);
        }
    }
}
