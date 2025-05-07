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
        private readonly MemberFilter _filter;
        private readonly Dictionary<Type, List<MemberWithSerializer>> _memberWithSerializersByClassType = new Dictionary<Type, List<MemberWithSerializer>>();

        private static readonly BindingFlags AllBinding =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public MembersWithSerializerGetter(MemberFilter filter)
        {
            _filter = filter;
        }

        public List<MemberWithSerializer> Get(Type targetType)
        {
            if (!_memberWithSerializersByClassType.TryGetValue(targetType, out var list))
            {
                var members = targetType.GetMembers(AllBinding)
                    .Where(m => _filter(m))
                    .Select(m =>
                    {
                        var mtype = m.GetMemberType();
                        return new MemberWithSerializer()
                        {
                            MemberType = mtype,
                            Member = m,
                            ValueGetter = MemberAccessor.GetMemberValueGetter(m),
                            ValueSetter = MemberAccessor.GetMemberValueSetter(m),
                            Serializer = EasySerializersManager.Instance.GetSerializer(mtype)
                        };
                    });

                list = new List<MemberWithSerializer>(members);
                _memberWithSerializersByClassType[targetType] = list;
            }

            return list;
        }
    }
}
