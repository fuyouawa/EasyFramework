using System;
using UnityEngine;
using System.Collections.Generic;

namespace EasyFramework.Core
{
    public class UnityObjectPoolManager : MonoSingleton<UnityObjectPoolManager>, IUnityObjectPoolManager
    {
        // 使用 (poolName, objectType) 作为唯一键来管理对象池
        private readonly Dictionary<(string poolName, Type objectType), IUnityObjectPool> _pools = new Dictionary<(string poolName, Type objectType), IUnityObjectPool>();

        public UnityObjectPoolManagerSettings DefaultSettings { get; set; } = new UnityObjectPoolManagerSettings()
        {
            PoolAllocator = DefaultPoolAllocator
        };

        public bool TryAllocatePool(string poolName, Type objectType, GameObject original)
        {
            if (objectType == null) throw new ArgumentNullException(nameof(objectType));
            if (original == null) throw new ArgumentNullException(nameof(original));

            var key = (poolName, objectType);

            // 已存在同名同类型池，则返回 false 表示分配失败
            if (_pools.ContainsKey(key))
            {
                return false;
            }

            var allocator = DefaultSettings?.PoolAllocator ?? throw new InvalidOperationException("Pool allocator is not configured.");
            var pool = allocator.Invoke(poolName, objectType, original);

            // 创建一个节点作为此 Pool 的父级，用于层级管理
            var node = new GameObject($"UnityObjectPool_{poolName}_{objectType.Name}");
            node.transform.SetParent(this.transform, false);
            pool.Transform = node.transform;

            _pools.Add(key, pool);

            return true;
        }

        public IUnityObjectPool TryGetPool(string poolName, Type objectType)
        {
            if (objectType == null) throw new ArgumentNullException(nameof(objectType));

            _pools.TryGetValue((poolName, objectType), out var pool);
            return pool;
        }

        private static IUnityObjectPool DefaultPoolAllocator(string poolName, Type objectType,
            GameObject original)
        {
            return new UnityObjectPool(poolName, objectType, original);
        }
    }
}
