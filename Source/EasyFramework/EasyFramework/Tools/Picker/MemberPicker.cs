using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EasyFramework
{
    [Serializable, InlineProperty, HideLabel, HideReferenceObjectPicker]
    public abstract class MemberPicker : ISerializationCallbackReceiver
    {
        enum MemberTypes
        {
            None,
            Field,
            Property,
            Method
        }

        [NonSerialized, ShowInInspector] private GameObject _targetObject;
        [NonSerialized, ShowInInspector] private Component _targetComponent;
        [NonSerialized, ShowInInspector] private MemberInfo _targetMember;

        public Component TargetComponent => _targetComponent;
        public MemberInfo TargetMember => _targetMember;

        [SerializeField, HideInInspector] private MemberTypes _serializedTargetMemberType;
        [SerializeField, HideInInspector] private byte[] _serializedTargetMemberData;
        [SerializeField, HideInInspector] private List<Object> _serializedUnityObjects;

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _serializedUnityObjects ??= new List<Object>();
            _serializedUnityObjects.Clear();
            _serializedUnityObjects.Add(_targetObject);
            _serializedUnityObjects.Add(_targetComponent);

            if (_targetMember == null)
            {
                _serializedTargetMemberType = MemberTypes.None;
                _serializedTargetMemberData = new byte[]{};
            }
            else if (_targetMember is MethodInfo method)
            {
                _serializedTargetMemberType = MemberTypes.Method;
                _serializedTargetMemberData = SerializationUtility.SerializeValue(method, DataFormat.Binary);
            }
            else if (_targetMember is PropertyInfo property)
            {
                _serializedTargetMemberType = MemberTypes.Property;
                _serializedTargetMemberData = SerializationUtility.SerializeValue(property, DataFormat.Binary);
            }
            else if (_targetMember is FieldInfo field)
            {
                _serializedTargetMemberType = MemberTypes.Field;
                _serializedTargetMemberData = SerializationUtility.SerializeValue(field, DataFormat.Binary);
            }

            OnBeforeSerialize();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            switch (_serializedTargetMemberType)
            {
                case MemberTypes.None:
                    _targetMember = null;
                    break;
                case MemberTypes.Field:
                    _targetMember = SerializationUtility.DeserializeValue<FieldInfo>(
                        _serializedTargetMemberData,
                        DataFormat.Binary);
                    break;
                case MemberTypes.Property:
                    _targetMember = SerializationUtility.DeserializeValue<PropertyInfo>(
                        _serializedTargetMemberData,
                        DataFormat.Binary);
                    break;
                case MemberTypes.Method:
                    _targetMember = SerializationUtility.DeserializeValue<MethodInfo>(
                        _serializedTargetMemberData,
                        DataFormat.Binary);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (_serializedUnityObjects.IsNotNullOrEmpty())
            {
                _targetObject = _serializedUnityObjects[0] as GameObject;
                if (_serializedUnityObjects.Count > 1)
                {
                    _targetComponent = _serializedUnityObjects[1] as Component;
                }
            }
            OnAfterDeserialize();
        }

        protected virtual void OnBeforeSerialize()
        {
        }

        protected virtual void OnAfterDeserialize()
        {
        }
    }
}
