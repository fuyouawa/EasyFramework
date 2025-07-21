using System;
using EasyToolKit.Core.Editor;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    public class UnityPropertyAccessor<TOwner, TValue> : ValueAccessor<TOwner, TValue>
    {
        private static readonly Func<SerializedProperty, TValue> ValueGetter = SerializedPropertyUtility.GetValueGetter<TValue>();
        private static readonly Action<SerializedProperty, TValue> ValueSetter = SerializedPropertyUtility.GetValueSetter<TValue>();

        private readonly SerializedProperty _serializedProperty;

        public UnityPropertyAccessor(SerializedProperty serializedProperty)
        {
            _serializedProperty = serializedProperty.Copy();
        }

        public override void SetValue(ref TOwner owner, TValue value)
        {
            ValueSetter(_serializedProperty, value);
        }

        public override TValue GetValue(ref TOwner owner)
        {
            return ValueGetter(_serializedProperty);
        }
    }
}
