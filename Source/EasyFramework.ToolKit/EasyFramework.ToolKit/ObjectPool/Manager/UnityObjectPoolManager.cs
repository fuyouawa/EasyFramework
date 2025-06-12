using System;
using UnityEngine;
using System.Collections.Generic;
using EasyFramework.Core;

namespace EasyFramework.ToolKit
{
    /// <summary>
    /// Unity对象池管理器，负责创建和管理多个Unity对象池
    /// </summary>
    public class UnityObjectPoolManager : MonoSingleton<UnityObjectPoolManager>, IUnityObjectPoolManager
    {
        // 使用元组作为键，存储Unity对象池实例
        private readonly Dictionary<(string poolName, Type objectType), IUnityObjectPool> _pools = new Dictionary<(string poolName, Type objectType), IUnityObjectPool>();

        [SerializeField]
        private UnityObjectPoolManagerSettings _settings = new UnityObjectPoolManagerSettings()
        {
            PoolType = typeof(UnityObjectPool)
        };

        /// <summary>
        /// Unity对象池管理器的配置设置
        /// </summary>
        public UnityObjectPoolManagerSettings Settings
        {
            get => _settings;
            set => _settings = value;
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
            var node = new GameObject($"UnityObjectPool_{poolName}_{objectType.Name}");
            node.transform.SetParent(this.transform, false);
            pool.Transform = node.transform;

            _pools.Add(key, pool);

            return true;
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
        private static IUnityObjectPool CreateUnityObjectPool(string poolName, Type objectType,
            GameObject original)
        {
            if (Instance.Settings.PoolType == null)
            {
                throw new InvalidOperationException("PoolType in Settings cannot be null.");
            }

            if (!typeof(IUnityObjectPool).IsAssignableFrom(Instance.Settings.PoolType))
            {
                throw new InvalidOperationException($"PoolType '{Instance.Settings.PoolType}' must implement '{typeof(IUnityObjectPool)}'.");
            }

            if (Instance.Settings.PoolType.IsAbstract || Instance.Settings.PoolType.IsInterface)
            {
                throw new InvalidOperationException($"PoolType '{Instance.Settings.PoolType}' cannot be abstract or interface.");
            }

            try
            {
                return (IUnityObjectPool)Activator.CreateInstance(Instance.Settings.PoolType, poolName, objectType, original);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create unity object pool instance of type '{Instance.Settings.PoolType}'.", ex);
            }
        }
    }
}
