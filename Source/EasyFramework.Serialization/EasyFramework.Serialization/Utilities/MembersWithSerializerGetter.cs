using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyFramework.Core;

namespace EasyFramework.Serialization
{
    internal class MemberWithSerializer
    {
        public Type MemberType;
        public MemberInfo Member;
        public Func<object, object> ValueGetter;
        public Action<object, object> ValueSetter;
        public EasySerializer Serializer;
    }

    internal interface IMembersWithSerializerGetter
    {
        List<MemberWithSerializer> Get(Type targetType);
    }

    internal class MembersWithSerializerGetter : IMembersWithSerializerGetter
    {
        private readonly MemberFilterDelegate _filter;
        private readonly Dictionary<Type, List<MemberWithSerializer>> _dict = new Dictionary<Type, List<MemberWithSerializer>>();

        private static readonly BindingFlags AllBinding =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public MembersWithSerializerGetter(MemberFilterDelegate filter)
        {
            _filter = filter;
        }

        public List<MemberWithSerializer> Get(Type targetType)
        {
            if (!_dict.TryGetValue(targetType, out var list))
            {
                var members = targetType.GetMembers(AllBinding)
                    .Where(m => _filter(m))
                    .Select(m => new MemberWithSerializer()
                    {
                        MemberType = m.GetMemberType(),
                        Member = m,
                        ValueGetter = MemberAccessor.GetMemberValueGetter(m),
                        ValueSetter = MemberAccessor.GetMemberValueSetter(m),
                        Serializer = EasySerializersManager.GetSerializer(m.GetMemberType())
                    });

                list = new List<MemberWithSerializer>(members);
                _dict[targetType] = list;
            }

            return list;
        }
    }
}
