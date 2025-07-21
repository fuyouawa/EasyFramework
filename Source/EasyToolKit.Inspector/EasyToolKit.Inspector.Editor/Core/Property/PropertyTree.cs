using System;
using System.Collections.Generic;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class PropertyTree
    {
        private readonly List<InspectorProperty> _dirtyProperties = new List<InspectorProperty>();
        private Action _pendingCallbacks;
        private Action _pendingCallbacksUntilRepaint;

        public int UpdatedFrame { get; private set; }
        public SerializedObject SerializedObject { get; }
        public InspectorProperty LogicRootProperty { get; }

        public UnityEngine.Object[] Targets => SerializedObject.targetObjects;
        public Type TargetType => LogicRootProperty.Info.PropertyType;

        public bool DrawMonoScriptObjectField { get; set; }

        public event Action<InspectorProperty, int> OnPropertyValueChanged;


        public PropertyTree([NotNull] SerializedObject serializedObject)
        {
            if (serializedObject == null)
                throw new ArgumentNullException(nameof(serializedObject));

            SerializedObject = serializedObject;
            LogicRootProperty =
                new InspectorProperty(this, null, InspectorPropertyInfo.CreateForLogicRoot(serializedObject), 0);
        }

        public IEnumerable<InspectorProperty> EnumerateTree(bool includeChildren)
        {
            for (var i = 0; i < LogicRootProperty.Children!.Count; i++)
            {
                var property = LogicRootProperty.Children[i];
                yield return property;

                if (includeChildren && property.Children != null)
                {
                    foreach (var child in property.Children.Recurse())
                    {
                        yield return child;
                    }
                }
            }
        }

        public SerializedProperty GetUnityPropertyByPath(string propertyPath)
        {
            return SerializedObject.FindProperty(propertyPath);
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

            LogicRootProperty.Update();
            ++UpdatedFrame;
        }


        private void ApplyChanges()
        {
            bool changed = false;
            foreach (var property in _dirtyProperties)
            {
                if (property.ChildrenResolver != null)
                {
                    property.ChildrenResolver.ApplyChanges();
                }

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

        public static PropertyTree Create([NotNull] SerializedObject serializedObject)
        {
            if (serializedObject == null)
                throw new ArgumentNullException(nameof(serializedObject));

            return Create(serializedObject.targetObjects, serializedObject);
        }

        public static PropertyTree Create([NotNull] UnityEngine.Object[] targets,
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

            return new PropertyTree(serializedObject);
        }
    }
}
