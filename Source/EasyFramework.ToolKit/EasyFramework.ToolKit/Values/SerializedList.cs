using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Object = UnityEngine.Object;

namespace EasyFramework.ToolKit
{
    [Serializable]
    public class SerializedList<T> : SerializedListBase<T>
    {
        public override List<T> Value
        {
            get => _collection;
            set => _collection = value;
        }

        [NonSerialized, ShowInInspector]
        private List<T> _collection;

        public SerializedList()
        {
            _collection = new List<T>();
        }

        public SerializedList(int capacity)
        {
            _collection = new List<T>(capacity);
        }

        public SerializedList(IEnumerable<T> collection)
        {
            _collection = new List<T>(collection);
        }

        protected override void OnDeserializeData(ref byte[] serializedData, ref List<Object> serializedUnityObjects)
        {
            base.OnDeserializeData(ref serializedData, ref serializedUnityObjects);
            Value ??= new List<T>();
        }
    }
}
