using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    /// <summary>
    /// Unity对象池管理器的配置设置类，支持Unity序列化
    /// </summary>
    [Serializable]
    public class UnityObjectPoolManagerSettings : ISerializationCallbackReceiver
    {
        [NonSerialized, ShowInInspector] private Type _poolType;

        /// <summary>
        /// Unity对象池类型，用于创建Unity对象池实例
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// 当value为null，
        /// 或者是抽象类或接口，
        /// 或者没有公开的构造函数(string name, Type objectType, GameObject original)时抛出
        /// </exception>
        public Type PoolType
        {
            get => _poolType;
            set
            {
                if (value == null)
                {
                    throw new InvalidOperationException("PoolType cannot be null.");
                }

                if (value.IsAbstract || value.IsInterface)
                {
                    throw new InvalidOperationException($"PoolType '{value}' cannot be abstract or interface.");
                }

                // 检查是否有公开的构造函数(string name, Type objectType, GameObject original)
                var constructor = value.GetConstructor(new[] { typeof(string), typeof(Type), typeof(GameObject) });
                if (constructor == null)
                {
                    throw new InvalidOperationException($"PoolType '{value}' must have a public constructor with parameters (string name, Type objectType, GameObject original).");
                }

                _poolType = value;
            }
        }

        [SerializeField, HideInInspector]
        private byte[] _serializedUnityObjectPoolType;

        /// <summary>
        /// 序列化前调用，将Type类型序列化为字节数组
        /// </summary>
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _serializedUnityObjectPoolType = _poolType != null
                ? SerializationUtility.SerializeValue(_poolType, DataFormat.Binary)
                : null;
        }

        /// <summary>
        /// 反序列化后调用，将字节数组反序列化为Type类型
        /// </summary>
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _poolType = _serializedUnityObjectPoolType != null
                ? SerializationUtility.DeserializeValue<Type>(_serializedUnityObjectPoolType, DataFormat.Binary)
                : null;
        }
    }
}
