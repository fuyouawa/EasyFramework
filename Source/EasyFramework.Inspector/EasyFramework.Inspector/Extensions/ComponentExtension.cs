using System;
using System.Linq;
using EasyFramework;
using UnityEditor;
using UnityEngine;

namespace EasyGameFramework
{
    public static class ComponentExtension
    {
        private static MonoScript[] _allScriptsCache;
        public static MonoScript GetScript(this Component component)
        {
            if (_allScriptsCache.IsNullOrEmpty())
            {
                _allScriptsCache = MonoImporter.GetAllRuntimeMonoScripts();
            }

            try
            {
                return _allScriptsCache.First(s => s.GetClass() == component.GetType());
            }
            catch (Exception)
            {
                try
                {
                    _allScriptsCache = MonoImporter.GetAllRuntimeMonoScripts();
                    return _allScriptsCache.First(s => s.GetClass() == component.GetType());
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}
