using System;

namespace EasyFramework.ToolKit
{
    public static class IObjectPoolManagerExtension
    {
        public static IObjectPool TryGetPool<TObject>(this IObjectPoolManager manager, string poolName)
        {
            return manager.TryGetPool(poolName, typeof(TObject));
        }

        public static IObjectPool GetPool(this IObjectPoolManager manager, string poolName, Type objectType)
        {
            var pool = manager.TryGetPool(poolName, objectType);
            if (pool == null)
            {
                throw new InvalidOperationException(
                    $"Object pool with name '{poolName}' and object type '{objectType}' does not exist.");
            }
            return pool;
        }

        public static void CreatePool(this IObjectPoolManager manager, string poolName, Type objectType)
        {
            if (!manager.TryCreatePool(poolName, objectType))
            {
                throw new InvalidOperationException(
                    $"Failed to create object pool '{poolName}' with object type '{objectType}'. It may already exist.");
            }
        }

        public static IObjectPool GetOrCreatePool(this IObjectPoolManager manager, string poolName, Type objectType)
        {
            var pool = manager.TryGetPool(poolName, objectType);
            if (pool == null)
            {
                manager.CreatePool(poolName, objectType);
                pool = manager.GetPool(poolName, objectType);
            }
            return pool;
        }

        public static IObjectPool GetOrCreatePool<TObject>(this IObjectPoolManager manager, string poolName)
        {
            return manager.GetOrCreatePool(poolName, typeof(TObject));
        }

        public static IObjectPool GetPool<TObject>(this IObjectPoolManager manager, string poolName)
        {
            return manager.GetPool(poolName, typeof(TObject));
        }

        public static void CreatePool<TObject>(this IObjectPoolManager manager, string poolName)
        {
            manager.CreatePool(poolName, typeof(TObject));
        }
    }
}
