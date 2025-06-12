using System;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public static class IUnityObjectPoolExtension
    {
        public static TObjectPool SetRecycleInterval<TObjectPool>(this TObjectPool objectPool, float interval)
            where TObjectPool : IUnityObjectPool
        {
            objectPool.RecycleInterval = interval;
            return objectPool;
        }
    }
}
