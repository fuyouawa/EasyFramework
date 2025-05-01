using System;
using System.Linq;

namespace EasyFramework.Serialization
{
    internal static class TypeUtility
    {
        public static bool IsSubclassOfRawGeneric(this Type derivedType, Type genericBaseType)
        {
            while (derivedType != null && derivedType != typeof(object))
            {
                Type currentType = derivedType.IsGenericType ? derivedType.GetGenericTypeDefinition() : derivedType;
                if (currentType == genericBaseType)
                    return true;

                derivedType = derivedType.BaseType; // 继续向上查找
            }

            return false;
        }

        public static bool ImplementsGenericInterface(this Type type, Type genericInterfaceType)
        {
            var interfaces = type.GetInterfaces();
            var def = genericInterfaceType.GetGenericTypeDefinition();

            foreach (var i in interfaces)
            {
                if (!i.IsGenericType) continue;

                var def2 = i.GetGenericTypeDefinition();
                if (def == def2)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsDerivedOrImplementsGeneric(this Type type, Type genericType)
        {
            if (genericType.IsInterface)
            {
                return ImplementsGenericInterface(type, genericType);
            }

            return IsSubclassOfRawGeneric(type, genericType);
        }
    }
}
