using System;

namespace EasyToolKit.ToolKit
{
    public static class IObjectPoolManagerExtension
    {
        public static IObjectPool GetPool<TObject>(this IObjectPoolManager manager, string poolName)
        {
            return manager.GetPool(poolName, typeof(TObject));
        }

        public static void CreatePool<TObject>(this IObjectPoolManager manager, string poolName)
        {
            manager.CreatePool(poolName, typeof(TObject));
        }

        public static IObjectPool GetOrCreatePool(this IObjectPoolManager manager, string poolName, Type objectType)
        {
            try
            {
                return manager.GetPool(poolName, objectType);
            }
            catch (InvalidOperationException)
            {
                manager.CreatePool(poolName, objectType);
                return manager.GetPool(poolName, objectType);
            }
        }

        public static IObjectPool GetOrCreatePool<TObject>(this IObjectPoolManager manager, string poolName)
        {
            return manager.GetOrCreatePool(poolName, typeof(TObject));
        }
    }
}
