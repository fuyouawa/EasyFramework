using System;
using UnityEngine;

namespace EasyToolKit.ToolKit
{
    /// <summary>
    /// 游戏对象池接口，将GameObject实例池化，并通过缓存Component引用，提供快速访问组件的接口
    /// </summary>
    public interface IGameObjectPool : IObjectPool
    {
        /// <summary>
        /// 对象池中对象的原始预制体（用于实例化复制）
        /// </summary>
        GameObject Original { get; internal set; }

        /// <summary>
        /// 对象池的Transform组件，用于管理池中对象的层级关系
        /// </summary>
        Transform Transform { get; set; }

        /// <summary>
        /// 获取指定对象的生命周期访问器（包含激活时长、空闲时长与计时器控制）
        /// </summary>
        /// <param name="instance">池中 Unity 实例对象</param>
        /// <returns>生命周期访问器接口</returns>
        IPooledGameObjectLifetimeAccessor GetLifetimeAccessor(GameObject instance);

        /// <summary>
        /// 更新对象池状态，处理对象生命周期和回收
        /// </summary>
        /// <param name="deltaTime">距离上次更新的时间间隔（秒）</param>
        internal void Update(float deltaTime);
    }
}
