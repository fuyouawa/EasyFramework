using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using EasyToolKit.ThirdParty.OdinSerializer;
using EasyToolKit.ThirdParty.OdinSerializer.Utilities;
using UnityEditor;

namespace EasyToolKit.Core
{
    [Flags]
    public enum AssemblyCategory
    {
        None = 0,
        Scripts = 1,
        ImportedAssemblies = 2,
        UnityEngine = 4,
        DotNetRuntime = 8,
        DynamicAssemblies = 16, // 0x00000010
        Unknown = 32, // 0x00000020
        ProjectSpecific = ImportedAssemblies | Scripts, // 0x00000003
        All = ProjectSpecific | Unknown | DynamicAssemblies | DotNetRuntime | UnityEngine, // 0x0000003F
    }

    [InitializeOnLoad]
    public static class AssemblyUtility
    {
        private static string[] pluginAssemblyPrefixes = new string[6]
        {
            "assembly-csharp-firstpass",
            "assembly-csharp-editor-firstpass",
            "assembly-unityscript-firstpass",
            "assembly-unityscript-editor-firstpass",
            "assembly-boo-firstpass",
            "assembly-boo-editor-firstpass"
        };

        private static readonly Dictionary<Assembly, bool> IsDynamicCache =
            new Dictionary<Assembly, bool>((IEqualityComparer<Assembly>)ReferenceEqualityComparer<Assembly>.Default);

        private static readonly object IS_DYNAMIC_CACHE_LOCK = new object();
        private static readonly object ASSEMBLY_TYPE_FLAG_LOOKUP_LOCK = new object();
        private static readonly object ASSEMBLY_CATEGORY_LOOKUP_LOCK = new object();
        private static Assembly unityEngineAssembly = typeof(UnityEngine.Object).Assembly;
        private static Assembly unityEditorAssembly = typeof(UnityEditor.Editor).Assembly;
        private static DirectoryInfo projectAssetsFolderDirectory;
        private static DirectoryInfo scriptAssembliesDirectory;
        private static DirectoryInfo mscorlibDirectory;
        private static DirectoryInfo unityEngineDirectory;

        private static Dictionary<Assembly, AssemblyCategory> assemblyCategoryLookup =
            new Dictionary<Assembly, AssemblyCategory>(100);

        static AssemblyUtility()
        {
            string str = Environment.CurrentDirectory.Replace("\\", "//").Replace("//", "/").TrimEnd('/');
            string path1 = str + "/Assets";
            string path2 = str + "/Library/ScriptAssemblies";
            AssemblyUtility.projectAssetsFolderDirectory = new DirectoryInfo(path1);
            AssemblyUtility.scriptAssembliesDirectory = new DirectoryInfo(path2);
            AssemblyUtility.mscorlibDirectory = new DirectoryInfo(typeof(string).Assembly.GetAssemblyDirectory());
            AssemblyUtility.unityEngineDirectory =
                new DirectoryInfo(typeof(UnityEngine.Object).Assembly.GetAssemblyDirectory());
            if (!(AssemblyUtility.unityEngineDirectory.Parent.Name == "Managed"))
                return;
            AssemblyUtility.unityEngineDirectory = AssemblyUtility.unityEngineDirectory.Parent;
        }

        public static AssemblyCategory GetAssemblyCategory(Assembly assembly)
        {
            if (assembly == (Assembly)null)
                throw new NullReferenceException(nameof(assembly));
            lock (AssemblyUtility.ASSEMBLY_CATEGORY_LOOKUP_LOCK)
            {
                AssemblyCategory assemblyCategoryPrivate;
                if (!AssemblyUtility.assemblyCategoryLookup.TryGetValue(assembly, out assemblyCategoryPrivate))
                {
                    assemblyCategoryPrivate = AssemblyUtility.GetAssemblyCategoryPrivate(assembly);
                    AssemblyUtility.assemblyCategoryLookup[assembly] = assemblyCategoryPrivate;
                }

                return assemblyCategoryPrivate;
            }
        }

        private static AssemblyCategory GetAssemblyCategoryPrivate(Assembly assembly)
        {
            if (assembly.IsDynamic())
                return AssemblyCategory.DynamicAssemblies;
            string assemblyDirectory = assembly.GetAssemblyDirectory();
            if (assemblyDirectory != null && Directory.Exists(assemblyDirectory))
            {
                DirectoryInfo subDir = new DirectoryInfo(assemblyDirectory);
                if (subDir.FullName == AssemblyUtility.scriptAssembliesDirectory.FullName)
                    return AssemblyCategory.Scripts;
                if (File.Exists(assembly.Location + ".meta"))
                    return AssemblyCategory.ImportedAssemblies;
                if (AssemblyUtility.unityEngineDirectory.HasSubDirectory(subDir))
                    return AssemblyCategory.UnityEngine;
                if (AssemblyUtility.mscorlibDirectory.HasSubDirectory(subDir))
                    return AssemblyCategory.DotNetRuntime;
            }

            string name = assembly.GetName().Name;
            return name.StartsWith("UnityEngine.") || name.StartsWith("UnityEditor.")
                ? AssemblyCategory.UnityEngine
                : AssemblyCategory.Unknown;
        }

        /// <summary>
        /// Gets an <see cref="T:Sirenix.Utilities.ImmutableList" /> of all assemblies in the current <see cref="T:System.AppDomain" />.
        /// </summary>
        /// <returns>An <see cref="T:Sirenix.Utilities.ImmutableList" /> of all assemblies in the current <see cref="T:System.AppDomain" />.</returns>
        public static ImmutableList<Assembly> GetAllAssemblies()
        {
            return new ImmutableList<Assembly>((IList<Assembly>)AppDomain.CurrentDomain.GetAssemblies());
        }

        /// <summary>
        /// Determines whether an assembly is depended on another assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="otherAssembly">The other assembly.</param>
        /// <returns>
        ///   <c>true</c> if <paramref name="assembly" /> has a reference in <paramref name="otherAssembly" /> or <paramref name="assembly" /> is the same as <paramref name="otherAssembly" />.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException"><paramref name="assembly" /> is null.</exception>
        /// <exception cref="T:System.NullReferenceException"><paramref name="otherAssembly" /> is null.</exception>
        public static bool IsDependentOn(this Assembly assembly, Assembly otherAssembly)
        {
            if (assembly == (Assembly)null)
                throw new NullReferenceException(nameof(assembly));
            if (otherAssembly == (Assembly)null)
                throw new NullReferenceException(nameof(otherAssembly));
            if (assembly == otherAssembly)
                return true;
            string str = otherAssembly.GetName().ToString();
            foreach (object referencedAssembly in assembly.GetReferencedAssemblies())
            {
                if (str == referencedAssembly.ToString())
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the assembly module is a of type <see cref="T:System.Reflection.Emit.ModuleBuilder" />.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>
        ///   <c>true</c> if the specified assembly of type <see cref="T:System.Reflection.Emit.ModuleBuilder" />; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">assembly</exception>
        public static bool IsDynamic(this Assembly assembly)
        {
            if (assembly == (Assembly)null)
                throw new ArgumentNullException(nameof(assembly));
            bool flag;
            lock (AssemblyUtility.IS_DYNAMIC_CACHE_LOCK)
            {
                if (!AssemblyUtility.IsDynamicCache.TryGetValue(assembly, out flag))
                {
                    try
                    {
                        flag = assembly.GetType().FullName.EndsWith("AssemblyBuilder") || assembly.Location == null ||
                               assembly.Location == "";
                    }
                    catch
                    {
                        flag = true;
                    }

                    AssemblyUtility.IsDynamicCache.Add(assembly, flag);
                }
            }

            return flag;
        }

        /// <summary>
        /// Gets the full file path to a given assembly's containing directory.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The full file path to a given assembly's containing directory, or <c>Null</c> if no file path was found.</returns>
        /// <exception cref="T:System.NullReferenceException"><paramref name="assembly" /> is Null.</exception>
        public static string GetAssemblyDirectory(this Assembly assembly)
        {
            string path = !(assembly == (Assembly)null)
                ? assembly.GetAssemblyFilePath()
                : throw new ArgumentNullException(nameof(assembly));
            if (path == null)
                return (string)null;
            try
            {
                return Path.GetDirectoryName(path);
            }
            catch
            {
                return (string)null;
            }
        }

        /// <summary>Gets the full directory path to a given assembly.</summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The full directory path in which a given assembly is located, or <c>Null</c> if no file path was found.</returns>
        public static string GetAssemblyFilePath(this Assembly assembly)
        {
            if (assembly == (Assembly)null)
                return (string)null;
            if (assembly.IsDynamic())
                return (string)null;
            if (assembly.CodeBase == null)
                return (string)null;
            string str = "file:///";
            string codeBase = assembly.CodeBase;
            if (codeBase.StartsWith(str, StringComparison.InvariantCultureIgnoreCase))
            {
                string path = codeBase.Substring(str.Length).Replace('\\', '/');
                if (File.Exists(path))
                    return path;
                if (!Path.IsPathRooted(path))
                    path = !File.Exists("/" + path) ? Path.GetFullPath(path) : "/" + path;
                if (File.Exists(path))
                    return path;
                string location;
                try
                {
                    location = assembly.Location;
                }
                catch
                {
                    return (string)null;
                }

                if (File.Exists(location))
                    return location;
            }

            return File.Exists(assembly.Location) ? assembly.Location : (string)null;
        }

        /// <summary>Gets the type.</summary>
        /// <param name="fullName">The full name of the type, with or without any assembly information.</param>
        public static System.Type GetTypeByFullName(string fullName)
        {
            return TwoWaySerializationBinder.Default.BindToType(fullName);
        }

        /// <summary>
        /// Get types from the current AppDomain with a specified <see cref="T:Sirenix.Utilities.AssemblyCategory" /> filter.
        /// </summary>
        /// <param name="assemblyFlags">The <see cref="T:Sirenix.Utilities.AssemblyCategory" /> filters.</param>
        /// <returns>Types from the current AppDomain with the specified <see cref="T:Sirenix.Utilities.AssemblyCategory" /> filters.</returns>
        public static IEnumerable<System.Type> GetTypes(AssemblyCategory assemblyFlags)
        {
            Assembly[] assemblyArray = AppDomain.CurrentDomain.GetAssemblies();
            for (int index1 = 0; index1 < assemblyArray.Length; ++index1)
            {
                Assembly assembly = assemblyArray[index1];
                if ((AssemblyUtility.GetAssemblyCategory(assembly) & assemblyFlags) != AssemblyCategory.None)
                {
                    System.Type[] typeArray = assembly.SafeGetTypes();
                    for (int index2 = 0; index2 < typeArray.Length; ++index2)
                        yield return typeArray[index2];
                    typeArray = (System.Type[])null;
                }
            }

            assemblyArray = (Assembly[])null;
        }
    }
}
