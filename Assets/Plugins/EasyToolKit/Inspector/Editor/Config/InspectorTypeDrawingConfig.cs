using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [Serializable]
    public sealed class InspectorTypeDrawingConfig
    {
        private static readonly FieldInfo CustomEditorInspectedTypeField =
            typeof(CustomEditor).GetField("m_InspectedType", BindingFlagsHelper.AllInstance());

        private static readonly FieldInfo CustomEditorEditorForChildClassesField =
            typeof(CustomEditor).GetField("m_EditorForChildClasses", BindingFlagsHelper.AllInstance());

        private static readonly HashSet<Type> NeverDrawTypes;
        private static readonly List<Type> PossibleEditorTypes = new List<Type>();
        private static readonly HashSet<Type> PossibleDrawnTypes = new HashSet<Type>();
        private static readonly HashSet<Type> AllDrawnTypes = new HashSet<Type>();

        static InspectorTypeDrawingConfig()
        {
            if (CustomEditorInspectedTypeField == null || CustomEditorEditorForChildClassesField == null)
            {
                Debug.LogWarning(
                    "Could not find internal fields 'm_InspectedType' and/or 'm_EditorForChildClasses' in type UnityEditor.CustomEditor. Automatic inspector editor generation is highly unlikely to work.");
            }

            NeverDrawTypes = new HashSet<Type>();
            {
                var networkView = AssemblyUtility.GetTypeByFullName("UnityEngine.NetworkView");
                if (networkView != null) NeverDrawTypes.Add(networkView);
                var gUIText = AssemblyUtility.GetTypeByFullName("UnityEngine.GUIText");
                if (gUIText != null) NeverDrawTypes.Add(gUIText);
            }

            TypeCache.TypeCollection typesDerivedFrom = TypeCache.GetTypesDerivedFrom(typeof(UnityEngine.Object));
            var unityObjectTypes = new List<System.Type>(typesDerivedFrom.Count);
            using (TypeCache.TypeCollection.Enumerator enumerator = typesDerivedFrom.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    System.Type current = enumerator.Current;
                    if (!current.IsDefined(typeof(CompilerGeneratedAttribute), false) &&
                        !current.IsDefined(typeof(ObsoleteAttribute), false) && !NeverDrawTypes.Contains(current) &&
                        !typeof(Joint).IsAssignableFrom(current))
                        unityObjectTypes.Add(current);
                }
            }

            Dictionary<Type, Type> haveDrawersAlready = new Dictionary<Type, Type>();
            Dictionary<Type, Type> derivedClassDrawnTypes = new Dictionary<Type, Type>();


            foreach (var type in unityObjectTypes)
            {
                if (typeof(UnityEditor.Editor).IsAssignableFrom(type))
                {
                    try
                    {
                        bool editorForChildClasses;

                        var drawnType = GetEditorDrawnType(type, out editorForChildClasses);

                        if (drawnType != null)
                        {
                            if (!haveDrawersAlready.ContainsKey(drawnType))
                            {
                                haveDrawersAlready.Add(drawnType, type);
                            }

                            if (editorForChildClasses && !derivedClassDrawnTypes.ContainsKey(drawnType))
                            {
                                derivedClassDrawnTypes.Add(drawnType, type);
                            }
                        }

                        if (UnityInspectorEditorIsValidBase(type, null))
                        {
                            PossibleEditorTypes.Add(type);
                        }
                    }
                    catch (TypeLoadException)
                    {
                    }
                    catch (ReflectionTypeLoadException)
                    {
                    }
                }
            }


            HashSet<Type> stopBaseTypeLookUpTypes = new HashSet<Type>()
            {
                typeof(object),
                typeof(Component),
                typeof(Behaviour),
                typeof(MonoBehaviour),
                typeof(UnityEngine.Object),
                typeof(ScriptableObject),
                typeof(StateMachineBehaviour),
            };

            foreach (var type in unityObjectTypes
                         .Where(type =>
                             !type.IsAbstract && !type.IsGenericTypeDefinition && !type.IsGenericType &&
                             !typeof(UnityEditor.Editor).IsAssignableFrom(type) &&
                             !typeof(EditorWindow).IsAssignableFrom(type)))
            {
                Type preExistingEditorType;
                bool haveDrawerAlready = haveDrawersAlready.TryGetValue(type, out preExistingEditorType);

                if (!haveDrawerAlready)
                {
                    Type baseType = type.BaseType;

                    while (baseType != null && !stopBaseTypeLookUpTypes.Contains(baseType))
                    {
                        Type editor;

                        if (derivedClassDrawnTypes.TryGetValue(baseType, out editor))
                        {
                            haveDrawerAlready = true;
                            preExistingEditorType = editor;
                            break;
                        }

                        baseType = baseType.BaseType;
                    }
                }

                if (!haveDrawerAlready)
                {
                    PossibleDrawnTypes.Add(type);
                }

                AllDrawnTypes.Add(type);
            }
        }

        [SerializeField]
        private List<TypeDrawerPair> _configs = new List<TypeDrawerPair>();

        private Dictionary<Type, Type> _drawerCache = new Dictionary<Type, Type>();
        
        /// <summary>
        /// Forces the config's internal drawer type to value type lookup cache to rebuild itself.
        /// </summary>
        public void UpdateCaches()
        {
            this._drawerCache.Clear();

            for (int i = 0; i < this._configs.Count; i++)
            {
                var config = this._configs[i];

                Type drawnType = TypeConverter.FromName(config.DrawnTypeName);

                if (drawnType == null)
                {
                    continue;
                }

                Type drawerType;

                if (string.IsNullOrEmpty(config.EditorTypeName))
                {
                    drawerType = null;
                }
                else
                {
                    drawerType = TypeConverter.FromName(config.EditorTypeName);

                    if (drawerType == null)
                    {
                        drawerType = typeof(MissingEditor);
                    }
                }

                this._drawerCache[drawnType] = drawerType;
            }
        }
        public List<TypeDrawerPair> GetEditors()
        {
            var total = new List<TypeDrawerPair>();
            foreach (var type in AllDrawnTypes)
            {
                var editor = GetEditorType(type);

                if (editor != null && editor != typeof(InspectorTypeDrawingConfig.MissingEditor))
                {
                    total.Add(new TypeDrawerPair(type, editor));
                }
            }
            return total;
        }

        /// <summary>
        /// Determines whether Odin is capable of creating a custom editor for a given type.
        /// </summary>
        public static bool OdinCanCreateEditorFor(Type type)
        {
            return PossibleDrawnTypes.Contains(type);
        }

        /// <summary>
        /// Gets which editor type would draw the given type. If the type has not been assigned a custom editor type in the config, the default editor type is returned using <see cref="GetDefaultEditorType(Type)"/>.
        /// </summary>
        /// <param name="drawnType">The drawn type to get an editor type for.</param>
        /// <returns>The editor that would draw the given type.</returns>
        /// <exception cref="System.ArgumentNullException">drawnType is null</exception>
        public Type GetEditorType(Type drawnType)
        {
            if (drawnType == null)
            {
                throw new ArgumentNullException(nameof(drawnType));
            }

            Type editorType;

            if (this._drawerCache.TryGetValue(drawnType, out editorType))
            {
                return editorType;
            }

            return GetDefaultEditorType(drawnType);
        }

        /// <summary>
        /// Gets the default editor that this type would have, if no custom editor was set for this type in particular. This is calculated using the value of <see cref="P:Sirenix.OdinInspector.Editor.InspectorConfig.DefaultEditorBehaviour" />.
        /// </summary>
        /// <param name="drawnType">The drawn type to get the default editor for.</param>
        /// <returns>The editor that would draw this type by default, or null, if there is no default Odin-defined editor for the drawn type.</returns>
        /// <exception cref="T:System.ArgumentNullException">drawnType is null</exception>
        public static System.Type GetDefaultEditorType(System.Type drawnType)
        {
            if (drawnType == (System.Type)null)
                throw new ArgumentNullException(nameof(drawnType));
            if (!OdinCanCreateEditorFor(drawnType))
                return (System.Type)null;
            System.Type odinEditorType = null;
            if (InspectorConfig.Instance.DefaultEditorBehaviour == InspectorDefaultEditors.None)
                return (System.Type)null;
            bool useEasyInspector;
            switch (AssemblyUtility.GetAssemblyCategory(drawnType.Assembly))
            {
                case AssemblyCategory.Scripts:
                    useEasyInspector =
                        (InspectorConfig.Instance.DefaultEditorBehaviour & InspectorDefaultEditors.UserTypes) ==
                        InspectorDefaultEditors.UserTypes;
                    break;
                case AssemblyCategory.ImportedAssemblies:
                    useEasyInspector =
                        (InspectorConfig.Instance.DefaultEditorBehaviour & InspectorDefaultEditors.PluginTypes) ==
                        InspectorDefaultEditors.PluginTypes;
                    break;
                case AssemblyCategory.UnityEngine:
                    useEasyInspector =
                        (InspectorConfig.Instance.DefaultEditorBehaviour & InspectorDefaultEditors.UnityTypes) ==
                        InspectorDefaultEditors.UnityTypes;
                    break;
                default:
                    useEasyInspector =
                        (InspectorConfig.Instance.DefaultEditorBehaviour & InspectorDefaultEditors.OtherTypes) ==
                        InspectorDefaultEditors.OtherTypes;
                    break;
            }

            if (useEasyInspector)
                odinEditorType = typeof(EasyEditor);
            return odinEditorType;
        }

        /// <summary>
        /// Assigns a given editor to draw a given type.
        /// </summary>
        /// <param name="drawnType">The drawn type to assign an editor type for.</param>
        /// <param name="editorType">The editor type to assign. When generating editors, a type derived from this editor will be created and set to draw the given drawn type.</param>
        /// <exception cref="System.ArgumentNullException">drawnType</exception>
        /// <exception cref="System.ArgumentException">The type " + editorType.GetNiceName() + " is not a valid base editor for type " + drawnType.GetNiceName() + ". Check criteria using <see cref="UnityInspectorEditorIsValidBase(Type, Type)"/>.</exception>
        public void SetEditorType(Type drawnType, Type editorType)
        {
            if (drawnType == null)
            {
                throw new ArgumentNullException(nameof(drawnType));
            }

            string drawnTypeName = TypeConverter.ToName(drawnType);
            string editorTypeName = editorType == null ? "" : TypeConverter.ToName(editorType);

            if (editorType != null)
            {
                if (!UnityInspectorEditorIsValidBase(editorType, drawnType))
                {
                    throw new ArgumentException("The type " + editorType.GetNiceName() +
                                                " is not a valid base editor for type " + drawnType.GetNiceName() +
                                                ".");
                }
            }

            this._drawerCache[drawnType] = editorType;
            bool added = false;

            for (int i = 0; i < this._configs.Count; i++)
            {
                var pair = this._configs[i];

                if (pair.DrawnTypeName == drawnTypeName)
                {
                    pair.EditorTypeName = editorTypeName;
                    this._configs[i] = pair;
                    added = true;
                    break;
                }
            }

            if (!added)
            {
                this._configs.Add(new TypeDrawerPair(drawnType, editorType));
            }

            EditorUtility.SetDirty(InspectorConfig.Instance);
        }


        /// <summary>
        /// <para>Checks whether the given editor can be assigned to draw a given type using the <see cref="InspectorTypeDrawingConfig" /> class.</para>
        /// <para>This method checks the <see cref="CustomEditor"/> attribute on the type for whether the given type is compatible.</para>
        /// </summary>
        /// <param name="editorType">Type of the editor to check.</param>
        /// <param name="drawnType">Type of the drawn value to check. If this parameter is null, the drawn type is not checked for compatibility with the editor type; only the editor type itself is checked for validity.</param>
        /// <returns>True if the editor is valid, otherwise false</returns>
        /// <exception cref="System.ArgumentNullException">editorType</exception>
        public static bool UnityInspectorEditorIsValidBase(Type editorType, Type drawnType)
        {
            if (editorType == null)
            {
                throw new ArgumentNullException(nameof(editorType));
            }

            if (editorType.IsAbstract || !typeof(UnityEditor.Editor).IsAssignableFrom(editorType) ||
                editorType.FullName.StartsWith("UnityEditor", StringComparison.InvariantCulture))
            {
                return false;
            }

            if (CustomEditorInspectedTypeField == null)
            {
                return false;
            }

            var attribute = editorType.GetCustomAttribute<CustomEditor>(true);

            if (attribute == null)
            {
                return true;
            }

            if (drawnType != null)
            {
                Type inspectedType = (Type)CustomEditorInspectedTypeField.GetValue(attribute);

                if (inspectedType == drawnType)
                {
                    return true;
                }
                else if (CustomEditorEditorForChildClassesField != null && inspectedType.IsAssignableFrom(drawnType))
                {
                    return (bool)CustomEditorEditorForChildClassesField.GetValue(attribute);
                }
            }

            return false;
        }


        /// <summary>
        /// <para>Gets the type that an editor draws, by extracting it from the editor's <see cref="CustomEditor"/> attribute, if it is declared.</para>
        /// <para>This method returns null for abstract editor types, as those can never draw anything.</para>
        /// </summary>
        /// <param name="editorType">Type of the editor.</param>
        /// <param name="editorForChildClasses">Whether the editor in question is also an editor for types derived from the given type.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">editorType</exception>
        public static Type GetEditorDrawnType(Type editorType, out bool editorForChildClasses)
        {
            if (editorType == null)
            {
                throw new ArgumentNullException(nameof(editorType));
            }

            editorForChildClasses = false;

            if (editorType.IsAbstract || CustomEditorInspectedTypeField == null ||
                CustomEditorEditorForChildClassesField == null)
            {
                return null;
            }

            bool previous = Debug.unityLogger.logEnabled;
            Debug.unityLogger.logEnabled = false;
            var customEditorAttribute = editorType.GetCustomAttribute<CustomEditor>();
            Debug.unityLogger.logEnabled = previous;

            if (customEditorAttribute != null)
            {
                editorForChildClasses = (bool)CustomEditorEditorForChildClassesField.GetValue(customEditorAttribute);
                return (Type)CustomEditorInspectedTypeField.GetValue(customEditorAttribute);
            }

            return null;
        }

        /// <summary>
        /// A type that indicates that a drawer is missing.
        /// </summary>
        public static class MissingEditor
        {
        }
    }
}
