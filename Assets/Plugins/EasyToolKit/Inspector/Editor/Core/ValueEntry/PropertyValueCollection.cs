using System.Collections;
using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor
{
    public abstract class PropertyValueCollection : IPropertyValueCollection
    {
        protected readonly InspectorProperty Property;

        protected PropertyValueCollection(InspectorProperty property)
        {
            Property = property;
        }

        public abstract int Count { get; }

        public object this[int index]
        {
            get => GetWeakValue(index);
            set => SetWeakValue(index, value);
        }

        public abstract IEnumerator GetEnumerator();

        protected abstract object GetWeakValue(int index);
        protected abstract void SetWeakValue(int index, object value);
    }
}
