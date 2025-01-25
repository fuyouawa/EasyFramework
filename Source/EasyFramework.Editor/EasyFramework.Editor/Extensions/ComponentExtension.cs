using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Editor
{
    public static class ComponentExtension
    {
        private static MonoScript[] s_allScriptsCache;
        public static MonoScript GetScript(this Component component)
        {
            if (s_allScriptsCache.IsNullOrEmpty())
            {
                s_allScriptsCache = MonoImporter.GetAllRuntimeMonoScripts();
            }

            try
            {
                return s_allScriptsCache.First(s => s.GetClass() == component.GetType());
            }
            catch (Exception)
            {
                try
                {
                    s_allScriptsCache = MonoImporter.GetAllRuntimeMonoScripts();
                    return s_allScriptsCache.First(s => s.GetClass() == component.GetType());
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}
