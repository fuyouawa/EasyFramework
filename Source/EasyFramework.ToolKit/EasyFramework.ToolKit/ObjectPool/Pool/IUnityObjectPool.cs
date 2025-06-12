using UnityEngine;

namespace EasyFramework.ToolKit
{
    /// <summary>
    /// Unity对象池接口，继承自IObjectPool，提供了Unity特定的对象池功能
    /// </summary>
    public interface IUnityObjectPool : IObjectPool
    {
        /// <summary>
        /// 对象回收检查的时间间隔（秒）
        /// </summary>
        float RecycleInterval { get; set; }

        /// <summary>
        /// 对象池中对象的原始预制体
        /// </summary>
        GameObject Original { get; internal set; }

        /// <summary>
        /// 对象池的Transform组件，用于管理池中对象的层级关系
        /// </summary>
        Transform Transform { get; set; }

        /// <summary>
        /// 更新对象池状态，处理对象生命周期和回收
        /// </summary>
        /// <param name="deltaTime">距离上次更新的时间间隔（秒）</param>
        internal void Update(float deltaTime);
    }
}
