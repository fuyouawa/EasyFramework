using System;
using EasyToolKit.Core.Internal.OdinSerializer;

namespace EasyToolKit.Core
{
    public static class TypeConverter
    {
        public static string ToName(Type type)
        {
            return TwoWaySerializationBinder.Default.BindToName(type);
        }
        public static Type FromName(string typeName)
        {
            return TwoWaySerializationBinder.Default.BindToType(typeName);
        }
    }
}
