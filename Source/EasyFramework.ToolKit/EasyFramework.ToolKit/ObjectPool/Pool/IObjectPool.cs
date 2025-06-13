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

        object TryRent();
        bool TryRelease(object instance);
        bool TryRemove(object instance);
    }
}
