namespace EasyFramework.ToolKit
{
    /// <summary>
    /// 为 <see cref="IPooledGameObjectLifetimeAccessor"/> 提供链式调用的扩展方法。
    /// </summary>
    public static class IPooledUnityObjectLifetimeAccessorExtension
    {
        /// <summary>
        /// 设置激活状态下的最大存活时间。
        /// </summary>
        /// <param name="accessor">生命周期访问器。</param>
        /// <param name="lifetime">存活时间（秒），小于0表示无限制。</param>
        /// <returns>配置后的访问器实例，支持链式调用。</returns>
        public static IPooledGameObjectLifetimeAccessor SetActiveLifetime(
            this IPooledGameObjectLifetimeAccessor accessor, float lifetime)
        {
            accessor.ActiveLifetime = lifetime;
            return accessor;
        }

        /// <summary>
        /// 设置空闲状态下的最大存活时间。
        /// </summary>
        /// <param name="accessor">生命周期访问器。</param>
        /// <param name="lifetime">存活时间（秒），小于0表示无限制。</param>
        /// <returns>配置后的访问器实例，支持链式调用。</returns>
        public static IPooledGameObjectLifetimeAccessor SetIdleLifetime(
            this IPooledGameObjectLifetimeAccessor accessor, float lifetime)
        {
            accessor.IdleLifetime = lifetime;
            return accessor;
        }

        /// <summary>
        /// 设置当前计时器累计时间。
        /// </summary>
        /// <param name="accessor">生命周期访问器。</param>
        /// <param name="elapsedTime">累计时间（秒）。</param>
        /// <returns>配置后的访问器实例，支持链式调用。</returns>
        public static IPooledGameObjectLifetimeAccessor SetElapsedTime(
            this IPooledGameObjectLifetimeAccessor accessor, float elapsedTime)
        {
            accessor.ElapsedTime = elapsedTime;
            return accessor;
        }
    }
}
