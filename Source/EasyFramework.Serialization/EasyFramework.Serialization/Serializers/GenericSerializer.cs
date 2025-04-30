using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace EasyFramework.Serialization
{
    [EasySerializerConfig(EasySerializerProiority.Generic)]
    public class GenericSerializer<T> : EasySerializer<T>
        where T : new()
    {
        private List<MemberWithSerializer> _memberWithSerializersCache;
        private static readonly bool IsNode = IsNodeImpl(typeof(T));
        private static readonly Func<T> Constructor = CreateConstructor();

        public override void Process(string name, ref T value, IArchive archive)
        {
            Debug.Assert(!typeof(T).IsSubclassOf(typeof(UnityEngine.Object)));

            _memberWithSerializersCache ??= Settings.MembersWithSerializerGetter.Get(typeof(T));

            if (value == null)
            {
                value = Constructor();
            }

            if (!IsRoot)
            {
                if (IsNode)
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
                    obj = memberWithSerializer.ValueGetter(value);
                }

                serializer.Process(member.Name, ref obj, memberType, archive);

                if (archive.ArchiveIoType == ArchiveIoTypes.Input)
                {
                    memberWithSerializer.ValueSetter(value, obj);
                }
            }

            if (!IsRoot)
            {
                if (IsNode)
                {
                    archive.FinishNode();
                }
            }
        }

        private static bool IsNodeImpl(Type type)
        {
            return (type.IsClass && type != typeof(string)) ||
                   (type.IsValueType && !type.IsPrimitive && !type.IsEnum);
        }

        private static Func<T> CreateConstructor()
        {
            var newExp = Expression.New(typeof(T));
            return Expression.Lambda<Func<T>>(newExp).Compile();
        }
    }
}
