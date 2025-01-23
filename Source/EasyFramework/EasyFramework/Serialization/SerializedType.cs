using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace EasyFramework
{
    [Serializable]
    public class SerializedType : StringSerializedValue<Type>
    {
        public override Type Value
        {
            get => _type;
            set => _type = value;
        }

        [NonSerialized, ShowInInspector]
        private Type _type;

        protected override void SerializeData(ref string serializedData)
        {
            if (_type == null)
            {
                serializedData = string.Empty;
                return;
            }

            serializedData = TwoWaySerializationBinder.Default.BindToName(_type);
        }

        protected override void DeserializeData(ref string serializedData)
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
