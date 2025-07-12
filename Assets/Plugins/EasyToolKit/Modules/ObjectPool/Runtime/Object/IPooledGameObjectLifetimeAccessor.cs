using System;

namespace EasyToolKit.ToolKit
{
    /// <summary>
    /// <para>提供对池化游戏对象的生命周期属性（激活/空闲时长、当前计时器）的访问接口。</para>
    /// <para>主要用于内部机制获取或设置如下属性：</para>
    /// <para>- 激活状态下的最大存活时间（超时将回收）</para>
    /// <para>- 空闲状态下的最大存活时间（超时将销毁）</para>
    /// <para>- 当前计时器（根据状态不同表示距离回收或销毁的时间）</para>
    /// </summary>
    public interface IPooledGameObjectLifetimeAccessor
    {
        /// <summary>
        /// 激活中对象的回收时间，小于0则无限制
        /// </summary>
        public float ActiveLifetime { get; set; }

        /// <summary>
        /// 空闲中对象的销毁时间，小于0则无限制
        /// </summary>
        public float IdleLifetime { get; set; }

        /// <summary>
        /// <para>获取当前计时器累计时间（单位：秒）。</para>
        /// <para>- 激活状态时：表示已激活时长</para>
        /// <para>- 空闲状态时：表示已空闲时长</para>
        /// </summary>
        public float ElapsedTime { get; set; }
    }

    public class PooledGameObjectLifetimeAccessor : IPooledGameObjectLifetimeAccessor
    {
        private readonly Func<float> _activeLifetimeGetter;
        private readonly Action<float> _activeLifetimeSetter;
        private readonly Func<float> _idleLifetimeGetter;
        private readonly Action<float> _idleLifetimeSetter;
        private readonly Func<float> _elapsedTimeGetter;
        private readonly Action<float> _elapsedTimeSetter;


        public float ActiveLifetime
        {
            get => _activeLifetimeGetter();
            set => _activeLifetimeSetter(value);
        }

        public float IdleLifetime
        {
            get => _idleLifetimeGetter();
            set => _idleLifetimeSetter(value);
        }
        public float ElapsedTime
        {
            get => _elapsedTimeGetter();
            set => _elapsedTimeSetter(value);
        }

        public PooledGameObjectLifetimeAccessor(Func<float> activeLifetimeGetter, Action<float> activeLifetimeSetter,
            Func<float> idleLifetimeGetter, Action<float> idleLifetimeSetter, Func<float> elapsedTimeGetter,
            Action<float> elapsedTimeSetter)
        {
            _activeLifetimeGetter = activeLifetimeGetter;
            _activeLifetimeSetter = activeLifetimeSetter;
            _idleLifetimeGetter = idleLifetimeGetter;
            _idleLifetimeSetter = idleLifetimeSetter;
            _elapsedTimeGetter = elapsedTimeGetter;
            _elapsedTimeSetter = elapsedTimeSetter;
        }
    }
}
