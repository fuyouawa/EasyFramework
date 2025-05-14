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
        public string MemberName;
        public Func<object, object> ValueGetter;
        public Action<object, object> ValueSetter;
        public IEasySerializer Serializer;
    }

    internal interface IMembersWithSerializerGetter
    {
        MemberWithSerializer[] Get(Type targetType);
    }

    internal class MembersWithSerializerGetter : IMembersWithSerializerGetter
    {
        private readonly MemberFilter _filter;
        private readonly Dictionary<Type, MemberWithSerializer[]> _memberWithSerializersByClassType = new Dictionary<Type, MemberWithSerializer[]>();

        private static readonly BindingFlags AllBinding =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public MembersWithSerializerGetter(MemberFilter filter)
        {
            _filter = filter;
        }

        public MemberWithSerializer[] Get(Type targetType)
        {
            if (!_memberWithSerializersByClassType.TryGetValue(targetType, out var array))
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
                            MemberName = m.Name,
                            ValueGetter = MemberAccessor.GetMemberValueGetter(m),
                            ValueSetter = MemberAccessor.GetMemberValueSetter(m),
                            Serializer = EasySerializersManager.Instance.GetSerializer(mtype)
                        };
                    });

                array = members.ToArray();
                _memberWithSerializersByClassType[targetType] = array;
            }

            return array;
        }
    }
}
