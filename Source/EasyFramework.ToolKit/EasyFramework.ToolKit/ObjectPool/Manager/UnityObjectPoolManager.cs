using System;
using UnityEngine;
using System.Collections.Generic;
using EasyFramework.Core;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace EasyFramework.ToolKit
{
    /// <summary>
    /// Unity对象池管理器，负责创建和管理多个Unity对象池
    /// </summary>
    [ShowOdinSerializedPropertiesInInspector]
    public class UnityObjectPoolManager : MonoSingleton<UnityObjectPoolManager>, IUnityObjectPoolManager, ISerializationCallbackReceiver
    {
        // 使用元组作为键，存储Unity对象池实例
        private readonly Dictionary<(string poolName, Type objectType), IUnityObjectPool> _pools =
            new Dictionary<(string poolName, Type objectType), IUnityObjectPool>();


        [SerializeField] private string _poolNodeName = "{{ object_type }}_{{ pool_name }}";

        public string PoolNodeName
        {
            get => _poolNodeName;
            set => _poolNodeName = value;
        }
        
        [NonSerialized, OdinSerialize] private Type _poolType = typeof(UnityObjectPool);

        /// <summary>
        /// Unity对象池类型，用于创建Unity对象池实例
        /// </summary>
        /// <exception cref="InvalidOperationException">当要设置的value为null、是抽象或者接口、没有继承自IUnityObjectPool时抛出</exception>
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
                
                if (!typeof(IUnityObjectPool).IsAssignableFrom(_poolType))
                {
                    throw new InvalidOperationException(
                        $"PoolType '{_poolType}' must implement '{typeof(IUnityObjectPool)}'.");
                }

                _poolType = value;
            }
        }

        UnityObjectPoolManager()
        {
        }

        void FixedUpdate()
        {
            foreach (var pool in _pools.Values)
            {
                pool.Update(Time.fixedDeltaTime);
            }
        }

        /// <summary>
        /// 尝试创建Unity对象池
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <param name="objectType">对象池中存储的对象类型</param>
        /// <param name="original">原始预制体</param>
        /// <returns>true 表示创建成功；false 表示已存在同名同类型的池</returns>
        /// <exception cref="ArgumentNullException">当objectType或original为null时抛出</exception>
        public bool TryAllocatePool(string poolName, Type objectType, GameObject original)
        {
            if (objectType == null) throw new ArgumentNullException(nameof(objectType));
            if (original == null) throw new ArgumentNullException(nameof(original));

            var key = (poolName, objectType);

            if (_pools.ContainsKey(key))
            {
                return false;
            }

            var pool = CreateUnityObjectPool(poolName, objectType, original);

            // 创建一个节点作为此 Pool 的父级，用于层级管理
            var node = new GameObject(GetPoolNodeName(poolName, objectType));
            node.transform.SetParent(this.transform, false);
            pool.Transform = node.transform;

            _pools.Add(key, pool);

            return true;
        }

        private string GetPoolNodeName(string poolName, Type objectType)
        {
            return TemplateEngine.Render(_poolNodeName, new
            {
                ObjectType = objectType,
                PoolName = poolName,
            });
        }

        /// <summary>
        /// 尝试获取指定名称和类型的Unity对象池
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <param name="objectType">对象池中存储的对象类型</param>
        /// <returns>找到的对象池，如果不存在则返回null</returns>
        /// <exception cref="ArgumentNullException">当objectType为null时抛出</exception>
        public IUnityObjectPool TryGetPool(string poolName, Type objectType)
        {
            if (objectType == null) throw new ArgumentNullException(nameof(objectType));

            _pools.TryGetValue((poolName, objectType), out var pool);
            return pool;
        }

        /// <summary>
        /// 创建新的Unity对象池实例
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <param name="objectType">对象池中存储的对象类型</param>
        /// <param name="original">原始预制体</param>
        /// <returns>新创建的Unity对象池实例</returns>
        /// <exception cref="InvalidOperationException">当无法创建对象池实例时抛出</exception>
        private IUnityObjectPool CreateUnityObjectPool(string poolName, Type objectType,
            GameObject original)
        {
            try
            {
                var pool = (IUnityObjectPool)Activator.CreateInstance(_poolType);
                pool.Name = poolName;
                pool.ObjectType = objectType;
                pool.Original = original;
                return pool;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to create unity object pool instance of type '{_poolType}'.", ex);
            }
        }

        [SerializeField, HideInInspector] private SerializationData _serializationData;
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            UnitySerializationUtility.SerializeUnityObject(this, ref _serializationData);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            UnitySerializationUtility.DeserializeUnityObject(this, ref _serializationData);
        }
    }
}
