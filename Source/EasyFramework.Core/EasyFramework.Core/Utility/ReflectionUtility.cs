using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyFramework.Core
{
    public static class ReflectionUtility
    {
        public static Type FindType(string @namespace, string className)
        {
            return FindType($"{@namespace}.{className}");
        }

        public static Type FindType(string fullname)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(fullname);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }

        private static readonly Dictionary<string, Type> TypeByFullName = new Dictionary<string, Type>();

        public static Type FindTypeWithCache(string fullname)
        {
            if (!TypeByFullName.TryGetValue(fullname, out var type))
            {
                type = FindType(fullname);
                TypeByFullName.Add(fullname, type);
            }

            return type;
        }

        public static void PreBuildTypeSearchCache(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                if (type.FullName == null)
                    throw new ArgumentException($"The fullname of type '{type}' is null!");

                TypeByFullName[type.FullName] = type;
            }
        }

        public static IEnumerable<Type> GetAssemblyTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetTypes());
        }

        public static IEnumerable<Type> GetAssemblyExportedTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetExportedTypes());
        }

        public static IEnumerable<object> GetAssemblyAttributes(Type attributeType)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetCustomAttributes(attributeType, true));
        }

        public static IEnumerable<T> GetAssemblyAttributes<T>() where T : Attribute
        {
            return GetAssemblyAttributes(typeof(T)).Cast<T>();
        }

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
