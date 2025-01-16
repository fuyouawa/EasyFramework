using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyFramework
{
    public static class AppDomainExtension
    {
        public static IEnumerable<Assembly> GetProjectAssemblies(this AppDomain domain)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith("Assembly."));
        }

        public static IEnumerable<Assembly> GetUnityEngineAssemblies(this AppDomain domain)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith("UnityEngine."));
        }

        public static IEnumerable<Assembly> GetUnityEditorAssemblies(this AppDomain domain)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith("UnityEditor."));
        }

        public static IEnumerable<Assembly> GetUnityAssemblies(this AppDomain domain)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith("Unity."));
        }

        public static IEnumerable<Assembly> GetSystemAssemblies(this AppDomain domain)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith("System."));
        }
    }
}
