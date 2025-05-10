using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace EasyFramework.Serialization
{
    [EasySerializerConfig(EasySerializerProiority.Generic)]
    public class GenericSerializer<T> : EasySerializer<T>
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

            foreach (var memberWithSerializer in _memberWithSerializersCache)
            {
                var memberType = memberWithSerializer.MemberType;
                var member = memberWithSerializer.Member;
                var serializer = memberWithSerializer.Serializer;

                object obj = null;
                if (archive.ArchiveIoType == ArchiveIoTypes.Output)
                {
                    var getter = memberWithSerializer.ValueGetter;
                    if (getter == null)
                    {
                        throw new ArgumentException($"Member '{member}' is not readable!");
                    }

                    obj = getter(value);
                }

                serializer.Process(member.Name, ref obj, archive);

                if (archive.ArchiveIoType == ArchiveIoTypes.Input)
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
