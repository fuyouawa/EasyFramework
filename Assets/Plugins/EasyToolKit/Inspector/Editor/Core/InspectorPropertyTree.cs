using System;
using System.Collections;
using System.Collections.Generic;
using EasyToolKit.Core;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public abstract class InspectorPropertyTree
    {
        public abstract SerializedObject SerializedObject { get; }
        public abstract Type TargetType { get; }
        public abstract IReadOnlyList WeakTargets { get; }

        public bool DrawMonoScriptObjectField { get; set; }

        public abstract IReadOnlyList<InspectorProperty> RootProperties { get; }

        public abstract DrawerChainResolver DrawerChainResolver { get; set; }

        public abstract IEnumerable<InspectorProperty> EnumerateTree(bool includeChildren);

        public static InspectorPropertyTree Create([NotNull] SerializedObject serializedObject)
        {
            if (serializedObject == null)
                throw new ArgumentNullException(nameof(serializedObject));

            return Create(serializedObject.targetObjects, serializedObject);
        }

        public static InspectorPropertyTree Create([NotNull] IList targets, SerializedObject serializedObject)
        {
            if (targets == null) throw new ArgumentNullException(nameof(targets));


            if (serializedObject != null)
            {
                bool valid = true;
                var targetObjects = serializedObject.targetObjects;

                if (targets.Count != targetObjects.Length)
                {
                    valid = false;
                }
                else
                {
                    for (int i = 0; i < targets.Count; i++)
                    {
                        if (!object.ReferenceEquals(targets[i], targetObjects[i]))
                        {
                            valid = false;
                            break;
                        }
                    }
                }

                if (!valid)
                {
                    throw new ArgumentException(); //TODO 异常信息
                }
            }


            Type targetType = targets[0].GetType();

            for (int i = 1; i < targets.Count; i++)
            {
                object target = targets[i];

                if (ReferenceEquals(target, null))
                {
                    throw new ArgumentException("Target at index " + i + " was null.");
                }

                Type otherType = target.GetType();
                if (targetType != otherType)
                {
                    throw new ArgumentException(); //TODO 异常信息
                }
            }

            var treeType = typeof(InspectorPropertyTree<>).MakeGenericType(targetType);

            Array targetArray;
            if (targets.GetType().IsArray && targets.GetType().GetElementType() == targetType)
            {
                targetArray = (Array)targets;
            }
            else
            {
                targetArray = Array.CreateInstance(targetType, targets.Count);
                targets.CopyTo(targetArray, 0);
            }

            return (InspectorPropertyTree)Activator.CreateInstance(treeType, targetArray, serializedObject);
        }

        public void Draw()
        {
            BeginDraw();
            DrawProperties();
            EndDraw();
        }

        private void BeginDraw()
        {
            SerializedObject.UpdateIfRequiredOrScript();
            Update();

            if (DrawMonoScriptObjectField)
            {
                var scriptProperty = SerializedObject.FindProperty("m_Script");

                if (scriptProperty != null)
                {
                    var monoScript = scriptProperty.objectReferenceValue as MonoScript;
                    if (monoScript == null)
                    {
                        EditorGUILayout.HelpBox(
                            "The associated script can not be loaded.\nPlease fix any compile errors\nand assign a valid script.",
                            MessageType.Warning);
                    }

                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.PropertyField(scriptProperty);
                    EditorGUI.EndDisabledGroup();
                }
            }
        }

        private void DrawProperties()
        {
            foreach (var property in EnumerateTree(false))
            {
                try
                {
                    property.Draw();
                }
                catch (Exception e)
                {
                    if (e is ExitGUIException || e.InnerException is ExitGUIException)
                    {
                        throw;
                    }

                    Debug.LogException(e); //TODO 更详细的异常信息
                }
            }
        }

        private void EndDraw()
        {
            SerializedObject.ApplyModifiedProperties();
        }

        protected virtual void Update()
        {
            foreach (var property in RootProperties)
            {
                property.Update();
            }
        }
    }

    public class InspectorPropertyTree<T> : InspectorPropertyTree
    {
        private readonly T[] _targets;
        private readonly List<InspectorProperty> _rootProperties = new List<InspectorProperty>();
        private DrawerChainResolver _drawerChainResolver;

        public override SerializedObject SerializedObject { get; }
        public override Type TargetType => typeof(T);

        public override IReadOnlyList<InspectorProperty> RootProperties => _rootProperties;

        public override DrawerChainResolver DrawerChainResolver
        {
            get
            {
                if (_drawerChainResolver == null)
                {
                    _drawerChainResolver = DefaultDrawerChainResolver.Instance;
                }
                return _drawerChainResolver;
            }
            set
            {
                if (!ReferenceEquals(_drawerChainResolver, value))
                {
                    _drawerChainResolver = value;
                    RefreshRootProperties();
                }
            }
        }

        public override IReadOnlyList WeakTargets => new ReadOnlyList(_targets);

        public InspectorPropertyTree([NotNull] T[] targets, [NotNull] SerializedObject serializedObject)
        {
            if (targets == null)
                throw new ArgumentNullException(nameof(targets));

            if (targets.Length == 0)
                throw new ArgumentException(); //TODO 异常信息

            if (serializedObject == null)
                throw new ArgumentNullException(nameof(serializedObject));

            for (int i = 0; i < targets.Length; i++)
            {
                if (object.ReferenceEquals(targets[i], null))
                {
                    throw new ArgumentException(); //TODO 异常信息
                }
            }

            SerializedObject = serializedObject;
            _targets = targets;

            var iterator = serializedObject.GetIterator();
            if (!iterator.NextVisible(true))
            {
                return;
            }
            
            int index = 0;
            do
            {
                if (iterator.propertyPath == "m_Script")
                {
                    continue;
                }

                _rootProperties.Add(InspectorProperty.Create(this, null,
                    InspectorPropertyInfo.CreateForUnityProperty(iterator), index, true));
                index++;
            } while (iterator.NextVisible(false));
        }

        public void RefreshRootProperties()
        {
            foreach (var property in RootProperties)
            {
                property.Refresh();
            }
        }

        public override IEnumerable<InspectorProperty> EnumerateTree(bool includeChildren)
        {
            foreach (var property in RootProperties)
            {
                yield return property;

                if (includeChildren)
                {
                    foreach (var child in property.Children.Recurse())
                    {
                        yield return child;
                    }
                }
            }
        }
    }
}
