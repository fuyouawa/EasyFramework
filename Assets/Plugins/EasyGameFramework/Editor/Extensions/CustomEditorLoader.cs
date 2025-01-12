using Sirenix.OdinInspector.Editor;
using System;
using System.Linq;
using EasyFramework;
using UnityEditor.Callbacks;

namespace EasyGameFramework.Editor
{
    internal static class CustomEditorLoader
    {
        [DidReloadScripts]
        private static void LoadCustomEditors()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                .Where(t => t.HasInterface(typeof(IEasyControl)) && !t.IsInterface && !t.IsAbstract);

            foreach (var type in types)
            {
                CustomEditorUtility.SetCustomEditor(type, typeof(EasyControlEditor), false, false);
            }
        }
    }
}
