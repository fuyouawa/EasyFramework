using System;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public interface IUnityObjectPoolManager
    {
        /// <summary>
        /// 尝试创建对象池。
        /// </summary>
        /// <param name="poolName"></param>
        /// <param name="objectType"></param>
        /// <param name="original"></param>
        /// <returns> true 表示创建成功；false 表示已存在同名同类型的池。</returns>
        bool TryCreatePool(string poolName, Type objectType, GameObject original);

        IUnityObjectPool TryGetPool(string poolName, Type objectType);
    }
}
