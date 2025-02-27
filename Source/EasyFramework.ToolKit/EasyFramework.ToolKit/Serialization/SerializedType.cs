using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace EasyFramework.ToolKit
{
    [Serializable]
    public class SerializedType : StringSerializedValue<Type>
    {
        public override Type Value
        {
            get => _type;
            set => _type = value;
        }

        [NonSerialized, ShowInInspector] private Type _type;

        public SerializedType()
        {
        }

        public SerializedType(Type type)
        {
            _type = type;
        }

        protected override void OnSerializeData(out string serializedData)
        {
            if (_type == null)
            {
                serializedData = string.Empty;
                return;
            }

            serializedData = TwoWaySerializationBinder.Default.BindToName(_type);
        }

        protected override void OnDeserializeData(ref string serializedData)
        {
            if (serializedData.IsNullOrEmpty())
            {
                _type = null;
                return;
            }

            _type = TwoWaySerializationBinder.Default.BindToType(serializedData);
        }
    }
}
