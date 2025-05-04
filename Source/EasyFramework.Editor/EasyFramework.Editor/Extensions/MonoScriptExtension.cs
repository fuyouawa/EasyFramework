using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace EasyFramework.Editor
{
    public static class MonoScriptExtension
    {
        private static MonoScript[] s_allScriptsCache;
        private static readonly Dictionary<Type, MonoScript> ScriptsCacheByType = new Dictionary<Type, MonoScript>(); 

        public static MonoScript GetMonoScript(this Type type)
        {
            if (!ScriptsCacheByType.TryGetValue(type, out var script))
            {
                script = InternalGetMonoScript(type);
                ScriptsCacheByType[type] = script;
            }
            return script;
        }

        private static MonoScript InternalGetMonoScript(Type type)
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
