using System.Reflection;

namespace EasyFramework.Serialization
{
    public delegate bool MemberFilter(MemberInfo member);

    public static class MemberFilterPresets
    {
        private static readonly BindingFlags AllInstance =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static readonly MemberFilter Default = new MemberFilterConfiguration()
            .MemberFilterFlags.Add(MemberFilterFlags.Field)
            .MemberFilterFlags.Add(MemberFilterFlags.Public)
            .MemberFilterFlags.Add(MemberFilterFlags.SerializeField)
            .CreateFilter();

        public static readonly MemberFilter AllGettable = new MemberFilterConfiguration()
            .MemberFilterFlags.Add(MemberFilterFlags.Field | MemberFilterFlags.ReadOnlyProperty |
                                   MemberFilterFlags.ReadWriteProperty)
            .MemberFilterFlags.Add(MemberFilterFlags.Public | MemberFilterFlags.NonPublic)
            .CreateFilter();

        public static readonly MemberFilter AllGetAndSettable = new MemberFilterConfiguration()
            .MemberFilterFlags.Add(MemberFilterFlags.Field | MemberFilterFlags.ReadOnlyProperty |
                                   MemberFilterFlags.ReadWriteProperty)
            .MemberFilterFlags.Add(MemberFilterFlags.Public | MemberFilterFlags.NonPublic)
            .CreateFilter();

        public static readonly MemberFilter AllPublicGettable = new MemberFilterConfiguration()
            .MemberFilterFlags.Add(MemberFilterFlags.Field | MemberFilterFlags.ReadOnlyProperty |
                                   MemberFilterFlags.ReadWriteProperty)
            .MemberFilterFlags.Add(MemberFilterFlags.Public)
            .CreateFilter();

        public static readonly MemberFilter AllPublicGetAndSettable = new MemberFilterConfiguration()
            .MemberFilterFlags.Add(MemberFilterFlags.Field | MemberFilterFlags.ReadOnlyProperty |
                                   MemberFilterFlags.ReadWriteProperty)
            .MemberFilterFlags.Add(MemberFilterFlags.Public)
            .CreateFilter();
    }
}
