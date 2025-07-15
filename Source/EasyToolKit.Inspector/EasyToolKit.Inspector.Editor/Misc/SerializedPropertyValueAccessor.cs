using System;
using EasyToolKit.Core.Editor;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    public class SerializedPropertyValueAccessor<TOwner, TValue> : ValueAccessor<TOwner, TValue>
    {
        private static readonly Func<SerializedProperty, TValue> ValueGetter = SerializedPropertyUtility.GetValueGetter<TValue>();
        private static readonly Action<SerializedProperty, TValue> ValueSetter = SerializedPropertyUtility.GetValueSetter<TValue>();

        private readonly SerializedProperty _serializedProperty;

        public SerializedPropertyValueAccessor(SerializedProperty serializedProperty)
        {
            _serializedProperty = serializedProperty;
        }

        public override void SetValue(ref TOwner target, TValue value)
        {
            ValueSetter(_serializedProperty, value);
        }

        public override TValue GetValue(ref TOwner target)
        {
            return ValueGetter(_serializedProperty);
        }
    }
}
