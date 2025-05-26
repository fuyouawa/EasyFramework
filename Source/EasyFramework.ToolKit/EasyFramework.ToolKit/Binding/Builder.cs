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
        [SerializeField] private bool _useGameObjectName = true;
        [ShowIf(nameof(ShowScriptName))]
        [SerializeField] private string _scriptName;

        [SerializeField] private ScriptType _buildScriptType;
        [NonSerialized, OdinSerialize] private Type _controllerBaseClass;
        [NonSerialized, OdinSerialize] private Type _uiPanelBaseClass;
        [NonSerialized, OdinSerialize] private Type _architectureType;

        [SerializeField] private GroupType _bindersGroupType;

        [ShowIf(nameof(ShowBindersGroupName))]
        [SerializeField] private string _bindersGroupName;

        [SerializeField] private bool _isInitialized;

        public string Namespace => _namespace;
        public string GenerateDirectory => _generateDirectory;
        public Type BaseClass => _buildScriptType switch
        {
            ScriptType.UIPanel => _uiPanelBaseClass,
            ScriptType.Controller => _controllerBaseClass,
            _ => throw new ArgumentOutOfRangeException()
        };
        public GroupType BindersGroupType => _bindersGroupType;
        public string BindersGroupName => _bindersGroupName;
        public ScriptType BuildScriptType => _buildScriptType;
        public Type ArchitectureType => _architectureType;

        private bool ShowScriptName => !_useGameObjectName;
        private bool ShowBindersGroupName => _bindersGroupType != GroupType.None;

        public string GetScriptName()
        {
            var scriptName = _scriptName;

            if (_useGameObjectName)
            {
                scriptName = gameObject.name;
            }

            return scriptName.Trim();
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

        public bool IsValidScriptName()
        {
            var scriptName = GetScriptName();
            if (scriptName.IsNullOrEmpty())
                return false;

            if (!char.IsLetter(scriptName[0]) && scriptName[0] != '_')
            {
                return false;
            }
            if (scriptName.Length == 1)
                return true;

            var other = scriptName[1..];
            foreach (var ch in other)
            {
                if (!char.IsLetterOrDigit(ch) && ch != '_')
                {
                    return false;
                }
            }
            return true;
        }
    }
}
