using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class PropertyValueEntry<TValue> : IPropertyValueEntry<TValue>
    {
        private bool? _isConflictedCache;
        private int? _lastUpdateID;
        private Type _runtimeValueType;
        public InspectorProperty Property { get; }
        public IPropertyValueCollection<TValue> Values { get; }

        public PropertyValueEntry(InspectorProperty property)
        {
            Property = property;
            Values = new PropertyValueCollection<TValue>(property);
        }

        public event Action<int> OnValueChanged;

        public object WeakSmartValue
        {
            get => SmartValue;
            set => SmartValue = (TValue)value;
        }

        public TValue SmartValue
        {
            get => Values[0];
            set
            {
                for (int i = 0; i < Values.Count; i++)
                {
                    Values[i] = value;
                }
            }
        }

        public Type BaseValueType => typeof(TValue);
        public IPropertyValueCollection WeakValues => Values;

        [CanBeNull] public Type RuntimeValueType => _runtimeValueType;

        public Type ValueType
        {
            get
            {
                if (RuntimeValueType != null)
                {
                    return RuntimeValueType;
                }

                return BaseValueType;
            }
        }

        public void Update()
        {
            if (_lastUpdateID == Property.Tree.UpdatedID)
            {
                return;
            }
            _lastUpdateID = Property.Tree.UpdatedID;

            _isConflictedCache = null;
            Values.Update();

            _runtimeValueType = TryGetRuntimeValueType();
        }

        public bool ApplyChanges()
        {
            bool changed = false;
            if (Values.Dirty)
            {
                foreach (var target in Property.Tree.Targets)
                {
                    Undo.RecordObject(target, $"Change {Property.Info.PropertyPath} on {target.name}");
                }

                changed = Values.ApplyChanges();

                if (changed)
                {
                    for (int i = 0; i < Property.Tree.Targets.Length; i++)
                    {
                        TriggerValueChanged(i);
                    }
                }
            }

            return changed;
        }

        public bool IsConflicted()
        {
            if (_isConflictedCache.HasValue)
            {
                return _isConflictedCache.Value;
            }

            _isConflictedCache = IsConflictedImpl();
            return _isConflictedCache.Value;
        }

        private bool IsConflictedImpl()
        {
            if (Property.Info.IsUnityProperty)
            {
                var serializedProperty = Property.Tree.GetUnityPropertyByPath(Property.Info.PropertyPath);
                return serializedProperty.hasMultipleDifferentValues;
            }

            if (Values.Count > 1)
            {
                var first = Values[0];
                for (int i = 1; i < Values.Count; i++)
                {
                    if (!EqualityComparer<TValue>.Default.Equals(first, Values[i]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        private Type TryGetRuntimeValueType()
        {
            if (BaseValueType.IsValueType || BaseValueType.IsSealed)
            {
                return BaseValueType;
            }

            Type runtimeValueType = null;
            for (int i = 0; i < Values.Count; i++)
            {
                object value = Values[i];
                if (value == null)
                {
                    return null;
                }

                var valueType = value.GetType();

                if (runtimeValueType == null)
                {
                    runtimeValueType = valueType;
                }
                else if (runtimeValueType != valueType)
                {
                    return null;
                }
            }

            return runtimeValueType;
        }

        internal void TriggerValueChanged(int index)
        {
            void Action()
            {
                if (this.OnValueChanged != null)
                {
                    try
                    {
                        this.OnValueChanged(index);
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

                Property.Tree.InvokePropertyValueChanged(Property, index);
            }


            if (Event.current != null && Event.current.type == EventType.Repaint)
            {
                Action();
            }
            else
            {
                Property.Tree.QueueCallbackUntilRepaint(Action);
            }
        }

        public void Dispose()
        {
            Values.Dispose();
        }
    }
}
