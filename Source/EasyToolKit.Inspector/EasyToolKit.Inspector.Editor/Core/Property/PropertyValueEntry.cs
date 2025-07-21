using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public interface IPropertyValueEntry
    {
        object WeakSmartValue { get; set; }
        Type ValueType { get; }
        IPropertyValueCollection WeakValues { get; }
        InspectorProperty Property { get; }

        event Action<int> OnValueChanged;

        bool IsConflicted();

        internal void Update();
        internal bool ApplyChanges();
    }

    public interface IPropertyValueEntry<TValue> : IPropertyValueEntry
    {
        TValue SmartValue { get; set; }
        IPropertyValueCollection<TValue> Values { get; }
    }

    public class PropertyValueEntry<TValue> : IPropertyValueEntry<TValue>
    {
        private bool? _isConflictedCache;
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

        public Type ValueType => typeof(TValue);
        public IPropertyValueCollection WeakValues => Values;

        void IPropertyValueEntry.Update()
        {
            _isConflictedCache = null;
            Values.Update();
        }

        bool IPropertyValueEntry.ApplyChanges()
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
    }
}
