using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyFramework
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
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(asm => asm.GetTypes());
        }
    }
}
