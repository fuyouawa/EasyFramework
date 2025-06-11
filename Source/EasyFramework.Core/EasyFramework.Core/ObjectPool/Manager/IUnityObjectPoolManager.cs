using System;
using UnityEngine;

namespace EasyFramework.Core
{
    public delegate IUnityObjectPool UnityObjectPoolAllocator(string poolName, Type objectType, GameObject original);

    public class UnityObjectPoolManagerSettings
    {
        public UnityObjectPoolAllocator PoolAllocator { get; set; }
    }

    public interface IUnityObjectPoolManager
    {
        UnityObjectPoolManagerSettings DefaultSettings { get; set; }

        /// <summary>
        /// 尝试创建 Unity 对象池。
        /// 返回 true 表示创建成功；false 表示已存在同名同类型的池。
        /// </summary>
        bool TryAllocatePool(string poolName, Type objectType, GameObject original);
        IUnityObjectPool TryGetPool(string poolName, Type objectType);
    }
}
