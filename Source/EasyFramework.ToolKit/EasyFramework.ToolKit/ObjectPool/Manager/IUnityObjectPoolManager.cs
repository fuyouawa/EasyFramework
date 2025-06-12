using System;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public interface IUnityObjectPoolManager
    {
        UnityObjectPoolManagerSettings Settings { get; set; }
        
        /// <summary>
        /// 尝试创建对象池。
        /// </summary>
        /// <param name="poolName"></param>
        /// <param name="objectType"></param>
        /// <returns> true 表示创建成功；false 表示已存在同名同类型的池。</returns>
        bool TryAllocatePool(string poolName, Type objectType, GameObject original);

        IUnityObjectPool TryGetPool(string poolName, Type objectType);
    }
}
