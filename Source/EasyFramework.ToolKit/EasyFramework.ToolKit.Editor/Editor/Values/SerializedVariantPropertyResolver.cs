using System;
using EasyFramework.Core;
using Sirenix.OdinInspector.Editor;

namespace EasyFramework.ToolKit.Editor
{
    public class SerializedVariantPropertyResolver : OdinPropertyResolver<SerializedVariant>
    {
        public override InspectorPropertyInfo GetChildInfo(int childIndex)
        {
            var variant = ValueEntry.SmartValue;
            var type = variant.Type;

            if (type.IsBasic())
            {
                var getterSetterType = typeof(GetterSetter<>).MakeGenericType(type);
                var getterSetter = Activator.CreateInstance(getterSetterType) as IValueGetterSetter;

                return InspectorPropertyInfo.CreateValue(Property.Label.text, 0, SerializationBackend.Unity, getterSetter);
            }

            throw new NotImplementedException();
        }
    
        public override int ChildNameToIndex(string name)
        {
            return 0;
        }
    
        protected override int GetChildCount(SerializedVariant value)
        {
            return 1;
        }
    
        class GetterSetter<T> : IValueGetterSetter<SerializedVariant, T>
        {
            public bool IsReadonly => false;
            public Type OwnerType => typeof(SerializedVariant);
            public Type ValueType => typeof(T);
    
            public void SetValue(ref SerializedVariant owner, T value)
            {
                owner.Set(value);
            }
    
            public T GetValue(ref SerializedVariant owner)
            {
                return owner.Get<T>();
            }
    
            public void SetValue(object owner, object value)
            {
                ((SerializedVariant)owner).SetRawObject(value);
            }
    
            public object GetValue(object owner)
            {
                return ((SerializedVariant)owner).GetRawObject();
            }
        }
    }
}
