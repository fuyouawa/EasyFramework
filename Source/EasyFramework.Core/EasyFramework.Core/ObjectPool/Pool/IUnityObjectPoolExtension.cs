using System;
using UnityEngine;

namespace EasyFramework.Core
{
    public static class IUnityObjectPoolExtension
    {
        public static TObjectPool SetRecycleInterval<TObjectPool>(this TObjectPool objectPool, float interval)
            where TObjectPool : IUnityObjectPool
        {
            objectPool.RecycleInterval = interval;
            return objectPool;
        }

        /// <summary>
        /// 设置默认对象生命周期。
        /// </summary>
        public static TObjectPool SetDefaultObjectLifetime<TObjectPool>(this TObjectPool objectPool, float lifetime)
            where TObjectPool : IUnityObjectPool
        {
            objectPool.DefaultObjectLifetime = lifetime;
            return objectPool;
        }

        /// <summary>
        /// 设置默认的 Pooled 组件类型（运行时动态类型）。
        /// </summary>
        public static TObjectPool SetDefaultPooledComponentType<TObjectPool>(this TObjectPool objectPool, Type componentType)
            where TObjectPool : IUnityObjectPool
        {
            objectPool.DefaultPooledComponentType = componentType;
            return objectPool;
        }
    }
}
