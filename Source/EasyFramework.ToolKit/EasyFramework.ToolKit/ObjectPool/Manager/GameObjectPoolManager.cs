using System;
using UnityEngine;
using System.Collections.Generic;
using EasyFramework.Core;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace EasyFramework.ToolKit
{
    /// <summary>
    /// 游戏对象池管理器，负责创建和管理多个游戏对象池
    /// </summary>
    [ShowOdinSerializedPropertiesInInspector]
    public class GameObjectPoolManager : MonoSingleton<GameObjectPoolManager>, IGameObjectPoolManager, ISerializationCallbackReceiver
    {
        // 使用元组作为键，存储游戏对象池实例
        private readonly Dictionary<(string poolName, Type objectType), IGameObjectPool> _pools =
            new Dictionary<(string poolName, Type objectType), IGameObjectPool>();


        [SerializeField] private string _poolNodeName = "{{ object_type }}_{{ pool_name }}";

        public string PoolNodeName
        {
            get => _poolNodeName;
            set => _poolNodeName = value;
        }
        
        [NonSerialized, OdinSerialize] private Type _poolType = typeof(GameObjectPool);

        /// <summary>
        /// 游戏对象池类型，用于创建游戏对象池实例
        /// </summary>
        /// <exception cref="InvalidOperationException">当要设置的value为null、是抽象或者接口、没有继承自<see cref="IGameObjectPool"/>时抛出</exception>
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
                
                if (!typeof(IGameObjectPool).IsAssignableFrom(value))
                {
                    throw new InvalidOperationException(
                        $"PoolType '{value}' must implement '{typeof(IGameObjectPool)}'.");
                }

                _poolType = value;
            }
        }

        GameObjectPoolManager()
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
        /// 尝试创建游戏对象池
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <param name="objectType">对象池中存储的对象类型</param>
        /// <param name="original">原始预制体</param>
        /// <returns></returns>
        public bool TryCreatePool(string poolName, Type objectType, GameObject original)
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
        /// 尝试获取指定名称和类型的游戏对象池
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <param name="objectType">对象池中存储的对象类型</param>
        /// <returns></returns>
        public IGameObjectPool TryGetPool(string poolName, Type objectType)
        {
            if (objectType == null) throw new ArgumentNullException(nameof(objectType));

            _pools.TryGetValue((poolName, objectType), out var pool);
            return pool;
        }

        /// <summary>
        /// 创建新的游戏对象池实例
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <param name="objectType">对象池中存储的对象类型</param>
        /// <param name="original">原始预制体</param>
        /// <returns></returns>
        private IGameObjectPool CreateUnityObjectPool(string poolName, Type objectType,
            GameObject original)
        {
            try
            {
                var pool = (IGameObjectPool)Activator.CreateInstance(_poolType);
                pool.Name = poolName;
                pool.ObjectType = objectType;
                pool.Original = original;
                return pool;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to create game object pool instance of type '{_poolType}'.", ex);
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
