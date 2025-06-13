using System;

namespace EasyFramework.ToolKit
{
    /// <summary>
    /// 对象池管理器接口，负责管理多个对象池的创建和访问
    /// </summary>
    public interface IObjectPoolManager
    {
        /// <summary>
        /// 尝试创建对象池
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <param name="objectType">对象池中存储的对象类型</param>
        /// <returns>true 表示创建成功；false 表示已存在同名同类型的池</returns>
        bool TryCreatePool(string poolName, Type objectType);

        /// <summary>
        /// 尝试获取指定名称和类型的对象池
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <param name="objectType">对象池中存储的对象类型</param>
        /// <returns>找到的对象池，如果不存在则返回null</returns>
        IObjectPool TryGetPool(string poolName, Type objectType);
    }
}
