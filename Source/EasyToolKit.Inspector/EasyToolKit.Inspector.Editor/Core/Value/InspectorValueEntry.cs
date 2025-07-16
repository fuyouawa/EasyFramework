using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class InspectorValueEntry<TValue> : IInspectorValueEntry<TValue>
    {
        private bool? _isConflictedCache;
        public InspectorProperty Property { get; }
        public IInspectorValueCollection<TValue> Values { get; }

        public InspectorValueEntry(InspectorProperty property)
        {
            Property = property;
            Values = new InspectorValueCollection<TValue>(property);
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
        public IInspectorValueCollection WeakValues => Values;

        void IInspectorValueEntry.Update()
        {
            _isConflictedCache = null;
            Values.Update();
        }

        bool IInspectorValueEntry.ApplyChanges()
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

            if (Values.Count > 1)
            {
                var first = Values[0];
                for (int i = 1; i < Values.Count; i++)
                {
                    if (!EqualityComparer<TValue>.Default.Equals(first, Values[i]))
                    {
                        _isConflictedCache = true;
                        break;
                    }
                }
            }

            if (!_isConflictedCache.HasValue)
            {
                _isConflictedCache = false;
            }

            return _isConflictedCache.Value;
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
