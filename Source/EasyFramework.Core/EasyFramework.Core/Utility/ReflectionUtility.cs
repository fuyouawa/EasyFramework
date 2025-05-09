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
    }
}
