using System;
using System.Collections.Generic;
using EasyFramework.Core;

namespace EasyFramework.ToolKit
{
    /// <summary>
    /// 对象池管理器，负责创建和管理多个对象池
    /// </summary>
    public class ObjectPoolManager : Singleton<ObjectPoolManager>, IObjectPoolManager
    {
        private Type _poolType;

        /// <summary>
        /// 对象池类型，用于创建对象池实例
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// 当value为null、是抽象类或接口、没有继承自IObjectPool时抛出
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
                
                if (!typeof(IObjectPool).IsAssignableFrom(value))
                {
                    throw new InvalidOperationException($"PoolType '{value}' must implement '{typeof(IObjectPool)}'.");
                }

                _poolType = value;
            }
        }

        // 使用元组作为键，存储对象池实例
        private readonly Dictionary<(string poolName, Type objectType), IObjectPool> _pools = new Dictionary<(string poolName, Type objectType), IObjectPool>();

        ObjectPoolManager()
        {
        }

        /// <summary>
        /// 尝试创建对象池
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <param name="objectType">对象池中存储的对象类型</param>
        /// <returns>true 表示创建成功；false 表示已存在同名同类型的池</returns>
        /// <exception cref="ArgumentNullException">当objectType为null时抛出</exception>
        public bool TryAllocatePool(string poolName, Type objectType)
        {
            if (objectType == null)
                throw new ArgumentNullException(nameof(objectType));

            var key = (poolName, objectType);

            if (_pools.ContainsKey(key))
            {
                return false;
            }

            var pool = CreateObjectPool(poolName, objectType);
            _pools.Add(key, pool);

            return true;
        }

        /// <summary>
        /// 尝试获取指定名称和类型的对象池
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <param name="objectType">对象池中存储的对象类型</param>
        /// <returns>找到的对象池，如果不存在则返回null</returns>
        /// <exception cref="ArgumentNullException">当objectType为null时抛出</exception>
        public IObjectPool TryGetPool(string poolName, Type objectType)
        {
            if (objectType == null)
                throw new ArgumentNullException(nameof(objectType));

            _pools.TryGetValue((poolName, objectType), out var pool);
            return pool;
        }

        /// <summary>
        /// 创建新的对象池实例
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <param name="objectType">对象池中存储的对象类型</param>
        /// <returns>新创建的对象池实例</returns>
        /// <exception cref="InvalidOperationException">当无法创建对象池实例时抛出</exception>
        private IObjectPool CreateObjectPool(string poolName, Type objectType)
        {
            try
            {
                var pool = (IObjectPool)Activator.CreateInstance(_poolType);
                pool.Name = poolName;
                pool.ObjectType = objectType;
                return pool;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create object pool instance of type '{_poolType}'.", ex);
            }
        }
    }
}
