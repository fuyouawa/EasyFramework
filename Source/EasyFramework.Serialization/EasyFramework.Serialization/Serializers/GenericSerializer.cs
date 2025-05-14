using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using EasyFramework.Core;

namespace EasyFramework.Serialization
{
    [EasySerializerConfig(EasySerializerProiority.Generic)]
    public class GenericSerializer<T> : EasySerializer<T>
    {
        private MemberWithSerializer[] _memberWithSerializersCache;
        private static readonly bool IsNode = IsNodeImpl(typeof(T));
        private static readonly Func<T> Constructor = CreateConstructor();

        public override bool CanSerialize(Type valueType)
        {
            return !valueType.IsBasic() && !valueType.IsSubclassOf(typeof(UnityEngine.Object));
        }

        public override void Process(string name, ref T value, IArchive archive)
        {
            _memberWithSerializersCache ??= Settings.MembersWithSerializerGetter.Get(typeof(T));

            if (value == null)
            {
                if (Constructor == null)
                {
                    throw new ArgumentException($"Type '{typeof(T)}' does not have a default constructor.");
                }

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

            var archiveIoType = archive.ArchiveIoType;
            foreach (var memberWithSerializer in _memberWithSerializersCache)
            {
                var memberType = memberWithSerializer.MemberType;
                var member = memberWithSerializer.Member;
                var memberName = memberWithSerializer.MemberName;
                var serializer = memberWithSerializer.Serializer;

                object obj = null;
                if (archiveIoType == ArchiveIoType.Output)
                {
                    var getter = memberWithSerializer.ValueGetter;
                    if (getter == null)
                    {
                        throw new ArgumentException($"Member '{member}' is not readable!");
                    }

                    obj = getter(value);
                }

                serializer.Process(memberName, ref obj, archive);

                if (archiveIoType == ArchiveIoType.Input)
                {
                    var setter = memberWithSerializer.ValueSetter;
                    if (setter == null)
                    {
                        throw new ArgumentException($"Member '{member}' is not writable!");
                    }

                    setter(value, obj);
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
            try
            {
                var newExp = Expression.New(typeof(T));
                return Expression.Lambda<Func<T>>(newExp).Compile();
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
