using Cysharp.Threading.Tasks;
using System;

namespace EasyFramework.ToolKit
{
    /// <summary>
    /// 对象池接口，定义了对象池的基本操作和属性
    /// </summary>
    public interface IObjectPool
    {
        /// <summary>
        /// 对象池的名称
        /// </summary>
        string Name { get; internal set; }

        /// <summary>
        /// 对象池中存储的对象类型
        /// </summary>
        Type ObjectType { get; internal set; }

        int ActiveCount { get; }
        int IdleCount { get; }

        /// <summary>
        /// 对象池的容量上限
        /// </summary>
        int Capacity { get; set; }

        internal void AddRentCallback(Action<object> callback);
        internal void AddReleaseCallback(Action<object> callback);

        object Rent();
        
        /// <summary>
        /// 将对象实例释放回对象池
        /// </summary>
        /// <param name="instance">要释放的对象实例</param>
        /// <returns>
        /// 如果对象成功释放回对象池，则返回true；
        /// 如果对象已经处于空闲状态或不属于此对象池，则返回false
        /// </returns>
        /// <exception cref="ArgumentNullException">当实例为null时抛出</exception>
        /// <exception cref="InvalidOperationException">当因其他原因无法释放对象时抛出</exception>
        bool Release(object instance);
        
        /// <summary>
        /// 从对象池中移除对象实例
        /// </summary>
        /// <param name="instance">要移除的对象实例</param>
        /// <returns>
        /// 如果对象成功从对象池中移除，则返回true；
        /// 如果对象已经不在对象池中，则返回false
        /// </returns>
        /// <exception cref="ArgumentNullException">当实例为null时抛出</exception>
        /// <exception cref="InvalidOperationException">当因其他原因无法移除对象时抛出</exception>
        bool Remove(object instance);
    }
}
