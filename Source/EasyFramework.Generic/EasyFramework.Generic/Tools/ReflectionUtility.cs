using System;

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
    }
}