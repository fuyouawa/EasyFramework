using System;

namespace EasyToolKit.ToolKit
{
    /// <summary>
    /// 对象池管理器接口，负责管理多个对象池的创建和访问
    /// </summary>
    public interface IObjectPoolManager
    {
        /// <summary>
        /// 创建对象池
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <param name="objectType">对象池中存储的对象类型</param>
        /// <exception cref="InvalidOperationException">当已存在同名同类型的池时抛出</exception>
        void CreatePool(string poolName, Type objectType);

        /// <summary>
        /// 获取指定名称和类型的对象池
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <param name="objectType">对象池中存储的对象类型</param>
        /// <returns>找到的对象池</returns>
        /// <exception cref="InvalidOperationException">当找不到指定的对象池时抛出</exception>
        IObjectPool GetPool(string poolName, Type objectType);
    }
}
