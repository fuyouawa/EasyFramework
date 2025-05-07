using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyFramework.Core;
using JetBrains.Annotations;
using Sirenix.Serialization;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    [Serializable, HideInInspector]
    public class SerializedAny
    {
        [SerializeField] private byte[] _serializedData;
        [SerializeField] private List<UnityEngine.Object> _serializedUnityObjects;

        public SerializedAny()
        {
        }

        public SerializedAny([CanBeNull] object value)
        {
            SetRawObject(value);
        }

        public bool IsEmpty()
        {
            return _serializedData.IsNullOrEmpty() && _serializedUnityObjects.IsNullOrEmpty();
        }

        public void SetNull()
        {
            _serializedData = null;
            _serializedUnityObjects = null;
        }

        public void SetRawObject([CanBeNull] object value)
        {
            if (value == null)
            {
                SetNull();
                return;
            }

            var from = SetMethod.MakeGenericMethod(value.GetType());
            from.Invoke(this, new[] { value });
        }

        public void Set<T>(T value)
        {
            if (value == null)
            {
                SetNull();
                return;
            }

            _serializedData = SerializationUtility.SerializeValue(
                value, DataFormat.Binary, out _serializedUnityObjects);
        }

        public T Get<T>()
        {
            if (_serializedData == null && _serializedUnityObjects.IsNullOrEmpty())
                return default;
            return SerializationUtility.DeserializeValue<T>(_serializedData, DataFormat.Binary,
                _serializedUnityObjects);
        }

        public object GetRawObject(Type type)
        {
            var to = GetMethod.MakeGenericMethod(type);
            return to.Invoke(this, null);
        }

        private static MethodInfo s_setMethod;

        private static MethodInfo SetMethod
        {
            get
            {
                if (s_setMethod == null)
                {
                    var methods = typeof(SerializedAny).GetMethods();
                    s_setMethod = methods.First(m => m.Name == "Set" && m.IsGenericMethod);
                }

                return s_setMethod;
            }
        }


        private static MethodInfo s_getMethod;

        private static MethodInfo GetMethod
        {
            get
            {
                if (s_getMethod == null)
                {
                    var methods = typeof(SerializedAny).GetMethods();
                    s_getMethod = methods.First(m => m.Name == "Get" && m.IsGenericMethod);
                }

                return s_getMethod;
            }
        }
    }
}
