using System;
using System.Collections.Generic;
using System.Linq;
using EasyFramework.Core;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    [Serializable]
    public sealed class Builder : SerializedMonoBehaviour
    {
        public enum GroupType
        {
            None,
            Title,
            Foldout
        }
        
        public enum ScriptType
        {
            UIPanel,
            Controller,
        }

        [FolderPath(ParentFolder = "Assets")]
        [SerializeField] private string _generateDirectory;

        [SerializeField] private string _namespace;
        [SerializeField] private bool _autoScriptName = true;
        [SerializeField] private string _scriptName;
        [NonSerialized, OdinSerialize] private Type _baseClass;

        [SerializeField] private GroupType _bindersGroupType;

        [ShowIf(nameof(ShowBindersGroupName))]
        [SerializeField] private string _bindersGroupName;

        [SerializeField] private ScriptType _buildScriptType;

        [SerializeField] private bool _isInitialized;

        public string Namespace => _namespace;
        public string GenerateDirectory => _generateDirectory;
        public Type BaseClass => _baseClass;
        public GroupType BindersGroupType => _bindersGroupType;
        public string BindersGroupName => _bindersGroupName;
        public ScriptType BuildScriptType => _buildScriptType;
        
        private bool ShowBindersGroupName => _bindersGroupType != GroupType.None;

        public string GetScriptName()
        {
            var scriptName = _scriptName;

            if (_autoScriptName)
            {
                scriptName = gameObject.name;
            }

            return scriptName;
        }

        public bool IsBuild()
        {
            return TryGetScriptType() != null;
        }

        public bool IsBuildAndBound()
        {
            var type = TryGetScriptType();
            return type != null && GetComponent(type) != null;
        }

        public string GetScriptTypeFullName()
        {
            return _namespace + "." + GetScriptName();
        }

        public Binder[] GetOwnedBinders()
        {
            return FindObjectsOfType<Binder>(true)
                .Where(binder => binder.OwningBuilders.Contains(this))
                .ToArray();
        }

        private static readonly Dictionary<string, Type> _typesByFullName = new Dictionary<string, Type>();
        public Type TryGetScriptType()
        {
            var fullname = GetScriptTypeFullName();
            if (_typesByFullName.TryGetValue(fullname, out var type))
            {
                return type;
            }

            type = ReflectionUtility.FindType(fullname);
            _typesByFullName[fullname] = type;
            return type;
        }

        public void BindTo(Component component)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            var fields = component.GetType().GetFields(BindingFlagsHelper.AllInstance())
                .Where(field => field.HasCustomAttribute<BindingAttribute>())
                .ToArray();

            var binders = GetOwnedBinders();

            foreach (var binder in binders)
            {
                var bindName = binder.GetBindName();
                var f = fields.FirstOrDefault(field => field.Name == bindName);
                if (f != null)
                {
                    var bindObject = binder.GetBindObject();
                    if (!f.FieldType.IsAssignableFrom(bindObject.GetType()))
                    {
                        throw new Exception($"The type of field '{f.Name}' does not match the type of the bound object.");
                    }
                    f.SetValue(component, bindObject);
                }
            }
        }
    }
}
