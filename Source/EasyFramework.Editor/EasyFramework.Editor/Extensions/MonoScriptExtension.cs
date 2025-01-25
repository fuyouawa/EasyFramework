using System;
using System.Linq;
using UnityEditor;

namespace EasyFramework.Editor
{
    public static class MonoScriptExtension
    {
        private static MonoScript[] s_allScriptsCache;

        public static MonoScript GetMonoScript(this Type type)
        {
            if (s_allScriptsCache.IsNullOrEmpty())
            {
                s_allScriptsCache = MonoImporter.GetAllRuntimeMonoScripts();
            }

            try
            {
                return s_allScriptsCache.First(s => s.GetClass() == type);
            }
            catch (Exception)
            {
                try
                {
                    s_allScriptsCache = MonoImporter.GetAllRuntimeMonoScripts();
                    return s_allScriptsCache.First(s => s.GetClass() == type);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}
