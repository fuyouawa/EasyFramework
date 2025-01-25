using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyFramework.Editor;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using SerializationUtility = Sirenix.Serialization.SerializationUtility;

namespace EasyFramework.Editor.Drawer
{
    public static class ViewModelHelper
    {
        private static List<Type> _baseTypes;

        public static List<Type> BaseTypes
        {
            get
            {
                if (_baseTypes == null)
                {
                    _baseTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                        .Where(t => t.IsSubclassOf(typeof(Component)) && !t.IsSealed).ToList();
                }

                return _baseTypes;
            }
        }

        public static void CheckIdentifier(string name, string id)
        {
            var error = GetIdentifierError(name, id);
            if (error.IsNotNullOrEmpty())
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
            }
        }

        public static string GetIdentifierError(string name, string id)
        {
            try
            {
                id.IsValidIdentifier(true);

                return string.Empty;
            }
            catch (InvalidIdentifierException e)
            {
                return e.Type switch
                {
                    InvalidIdentifierTypes.Empty => $"{name}为空",
                    InvalidIdentifierTypes.IllegalBegin => $"{name}必须以字母或下划线开头",
                    InvalidIdentifierTypes.IllegalContent => $"{name}的其余部分只能是字母、数字或下划线",
                    InvalidIdentifierTypes.CSharpKeywordConflict => $"{name}不能与C#关键字冲突",
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        public static  IViewBinder[] GetChildren(Transform target)
        {
            return target.GetComponentsInChildren<IViewBinder>()
                .Where(b => b.Info.OwnerViewModel == target)
                .ToArray();
        }

        [MenuItem("GameObject/EasyFramework/Add ViewModel")]
        private static void AddViewModel()
        {
            foreach (var o in Selection.gameObjects)
            {
                var c = o.GetOrAddComponent<ViewModel>();
            }
        }
    }
}
