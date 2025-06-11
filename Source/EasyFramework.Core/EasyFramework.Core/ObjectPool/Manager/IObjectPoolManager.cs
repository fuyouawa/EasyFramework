using System;

namespace EasyFramework.Core
{
    public delegate IObjectPool ObjectPoolAllocator(string poolName, Type objectType);

    public class ObjectPoolManagerSettings
    {
        public ObjectPoolAllocator PoolAllocator { get; set; }
    }

    public interface IObjectPoolManager
    {
        ObjectPoolManagerSettings DefaultSettings { get; set; }

        /// <summary>
        /// 尝试创建对象池。
        /// 返回 true 表示创建成功；false 表示已存在同名同类型的池。
        /// </summary>
        bool TryAllocatePool(string poolName, Type objectType);
        IObjectPool TryGetPool(string poolName, Type objectType);
    }
}
