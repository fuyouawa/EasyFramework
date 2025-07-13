using System;
using System.Collections.Generic;
using EasyToolKit.Core.Editor;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class InspectorPropertyTree
    {
        private readonly List<InspectorProperty> _rootProperties = new List<InspectorProperty>();
        private DrawerChainResolver _drawerChainResolver;
        private readonly List<InspectorProperty> _dirtyProperties = new List<InspectorProperty>();
        private Action _pendingCallbacks;
        private Action _pendingCallbacksUntilRepaint;

        public SerializedObject SerializedObject { get; }
        public UnityEngine.Object[] Targets => SerializedObject.targetObjects;
        public Type TargetType => SerializedObject.targetObject.GetType();

        public bool DrawMonoScriptObjectField { get; set; }

        public IReadOnlyList<InspectorProperty> RootProperties => _rootProperties;

        public event Action<InspectorProperty, int> OnPropertyValueChanged;


        public InspectorPropertyTree([NotNull] SerializedObject serializedObject)
        {
            if (serializedObject == null)
                throw new ArgumentNullException(nameof(serializedObject));

            SerializedObject = serializedObject;

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

                var info = InspectorPropertyInfo.CreateForUnityProperty(iterator);
                _rootProperties.Add(new InspectorProperty(this, null, info, index));
                index++;
            } while (iterator.NextVisible(false));
        }

        public DrawerChainResolver DrawerChainResolver
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

        public void RefreshRootProperties()
        {
            foreach (var property in RootProperties)
            {
                property.Refresh();
            }
        }

        public IEnumerable<InspectorProperty> EnumerateTree(bool includeChildren)
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

        public void SetPropertyDirty(InspectorProperty property)
        {
            _dirtyProperties.Add(property);
        }

        public void QueueCallback(Action action)
        {
            _pendingCallbacks += action;
        }

        public void QueueCallbackUntilRepaint(Action action)
        {
            _pendingCallbacksUntilRepaint += action;
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
            DoPendingCallbacks();
            SerializedObject.ApplyModifiedProperties();
        }

        private void DoPendingCallbacks()
        {
            if (_pendingCallbacks != null)
            {
                try
                {
                    _pendingCallbacks();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                _pendingCallbacks = null;
            }

            if (_pendingCallbacksUntilRepaint != null)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    try
                    {
                        _pendingCallbacksUntilRepaint();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }

                    _pendingCallbacksUntilRepaint = null;
                }
            }
        }

        private void Update()
        {
            ApplyChanges();
            foreach (var property in RootProperties)
            {
                property.Update();
            }
        }


        private void ApplyChanges()
        {
            bool changed = false;
            foreach (var property in _dirtyProperties)
            {
                if (property.ValueEntry != null)
                {
                    if (property.ValueEntry.ApplyChanges())
                    {
                        changed = true;
                    }
                }
            }

            if (changed)
            {
                foreach (var targetObject in SerializedObject.targetObjects)
                {
                    EasyEditorUtility.SetUnityObjectDirty(targetObject);
                }
            }

            _dirtyProperties.Clear();
        }

        internal void InvokePropertyValueChanged(InspectorProperty property, int index)
        {
            if (OnPropertyValueChanged != null)
            {
                try
                {
                    OnPropertyValueChanged(property, index);
                }
                catch (ExitGUIException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        public static InspectorPropertyTree Create([NotNull] SerializedObject serializedObject)
        {
            if (serializedObject == null)
                throw new ArgumentNullException(nameof(serializedObject));

            return Create(serializedObject.targetObjects, serializedObject);
        }

        public static InspectorPropertyTree Create([NotNull] UnityEngine.Object[] targets,
            SerializedObject serializedObject)
        {
            if (targets == null) throw new ArgumentNullException(nameof(targets));

            if (serializedObject != null)
            {
                bool valid = true;
                var targetObjects = serializedObject.targetObjects;

                if (targets.Length != targetObjects.Length)
                {
                    valid = false;
                }
                else
                {
                    for (int i = 0; i < targets.Length; i++)
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
            else
            {
                serializedObject = new SerializedObject(targets);
            }

            return new InspectorPropertyTree(serializedObject);
        }
    }
}
