using System;
using System.Collections.Generic;

namespace EasyFramework.Core
{
    public class ObjectPoolManager : Singleton<ObjectPoolManager>, IObjectPoolManager
    {
        public ObjectPoolManagerSettings DefaultSettings { get; set; } = new ObjectPoolManagerSettings()
        {
            PoolAllocator = DefaultPoolAllocator
        };

        // 使用 (poolName, objectType) 作为唯一键来管理对象池
        private readonly Dictionary<(string poolName, Type objectType), IObjectPool> _pools = new Dictionary<(string poolName, Type objectType), IObjectPool>();

        public bool TryAllocatePool(string poolName, Type objectType)
        {
            if (objectType == null)
                throw new ArgumentNullException(nameof(objectType));

            var key = (poolName, objectType);

            // 已存在同名同类型池，则返回 false 表示分配失败
            if (_pools.ContainsKey(key))
            {
                return false;
            }

            // 通过分配器创建新的对象池
            var allocator = DefaultSettings?.PoolAllocator ?? throw new InvalidOperationException("Pool allocator is not configured.");
            var pool = allocator.Invoke(poolName, objectType);

            _pools.Add(key, pool);

            return true;
        }

        public IObjectPool TryGetPool(string poolName, Type objectType)
        {
            if (objectType == null)
                throw new ArgumentNullException(nameof(objectType));

            _pools.TryGetValue((poolName, objectType), out var pool);
            return pool;
        }

        private static IObjectPool DefaultPoolAllocator(string poolName, Type objectType)
        {
            return new ObjectPool(poolName, objectType);
        }
    }
}
