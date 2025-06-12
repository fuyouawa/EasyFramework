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
        string Name { get; }

        /// <summary>
        /// 对象池中存储的对象类型
        /// </summary>
        Type ObjectType { get; }

        /// <summary>
        /// 当前对象池中的对象数量
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 对象池的容量上限
        /// </summary>
        int Capacity { get; set; }

        internal void AddSpawnCallback(Action<object> callback);
        internal void AddRecycleCallback(Action<object> callback);
        
        /// <summary>
        /// 异步尝试从对象池中获取一个对象
        /// </summary>
        /// <returns>获取到的对象；如果获取失败（比如对象池已满），则返回 null。</returns>
        UniTask<object> TrySpawnAsync();

        /// <summary>
        /// 尝试从对象池中获取一个对象
        /// </summary>
        /// <returns>获取到的对象；如果获取失败（比如对象池已满），则返回 null。</returns>
        object TrySpawn();

        /// <summary>
        /// 尝试将对象回收到对象池中
        /// </summary>
        /// <param name="instance">要回收的对象</param>
        /// <returns>回收是否成功</returns>
        bool TryRecycle(object instance);
    }
}
