using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EasyFramework.Serialization
{
    public delegate MemberInfo[] MembersGetterDelegate(Type targetType);

    public static class MembersGetterPresets
    {
        private static readonly BindingFlags AllInstance =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static readonly MembersGetterDelegate Default = new MembersGetterConfiguration()
            .FieldBindingFlags.Set(AllInstance)
            .FieldGetterFlags.Set(FieldGetterFlags.NeedSerializeFieldIfNonPublic)
            .CreateGetter();

        public static readonly MembersGetterDelegate AllGettable = new MembersGetterConfiguration()
            .FieldBindingFlags.Set(AllInstance)
            .PropertyBindingFlags.Set(AllInstance)
            .PropertyGetterFlags.Set(PropertyGetterFlags.Gettable)
            .CreateGetter();

        public static readonly MembersGetterDelegate AllGetAndSettable = new MembersGetterConfiguration()
            .FieldBindingFlags.Set(AllInstance)
            .PropertyBindingFlags.Set(AllInstance)
            .PropertyGetterFlags.Set(PropertyGetterFlags.Gettable | PropertyGetterFlags.Settable)
            .CreateGetter();

        public static readonly MembersGetterDelegate AllPublicGettable = new MembersGetterConfiguration()
            .FieldBindingFlags.Set(BindingFlags.Instance | BindingFlags.Public)
            .PropertyBindingFlags.Set(BindingFlags.Instance | BindingFlags.Public)
            .PropertyGetterFlags.Set(PropertyGetterFlags.Gettable)
            .CreateGetter();

        public static readonly MembersGetterDelegate AllPublicGetAndSettable = new MembersGetterConfiguration()
            .FieldBindingFlags.Set(BindingFlags.Instance | BindingFlags.Public)
            .PropertyBindingFlags.Set(BindingFlags.Instance | BindingFlags.Public)
            .PropertyGetterFlags.Set(PropertyGetterFlags.Gettable | PropertyGetterFlags.Settable)
            .CreateGetter();
    }
}
