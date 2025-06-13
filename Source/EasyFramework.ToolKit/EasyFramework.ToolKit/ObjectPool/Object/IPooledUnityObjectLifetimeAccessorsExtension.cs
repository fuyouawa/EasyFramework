namespace EasyFramework.ToolKit
{
    /// <summary>
    /// 为 <see cref="IPooledUnityObjectLifetimeAccessors"/> 提供链式调用的扩展方法。
    /// </summary>
    public static class IPooledUnityObjectLifetimeAccessorsExtension
    {
        /// <summary>
        /// 设置激活状态下的最大存活时间。
        /// </summary>
        /// <param name="accessors">生命周期访问器。</param>
        /// <param name="lifetime">存活时间（秒），小于0表示无限制。</param>
        /// <returns>配置后的访问器实例，支持链式调用。</returns>
        public static IPooledUnityObjectLifetimeAccessors SetActiveLifetime(
            this IPooledUnityObjectLifetimeAccessors accessors, float lifetime)
        {
            accessors.ActiveLifetime = lifetime;
            return accessors;
        }

        /// <summary>
        /// 设置空闲状态下的最大存活时间。
        /// </summary>
        /// <param name="accessors">生命周期访问器。</param>
        /// <param name="lifetime">存活时间（秒），小于0表示无限制。</param>
        /// <returns>配置后的访问器实例，支持链式调用。</returns>
        public static IPooledUnityObjectLifetimeAccessors SetIdleLifetime(
            this IPooledUnityObjectLifetimeAccessors accessors, float lifetime)
        {
            accessors.IdleLifetime = lifetime;
            return accessors;
        }

        /// <summary>
        /// 设置当前计时器累计时间。
        /// </summary>
        /// <param name="accessors">生命周期访问器。</param>
        /// <param name="elapsedTime">累计时间（秒）。</param>
        /// <returns>配置后的访问器实例，支持链式调用。</returns>
        public static IPooledUnityObjectLifetimeAccessors SetElapsedTime(
            this IPooledUnityObjectLifetimeAccessors accessors, float elapsedTime)
        {
            accessors.ElapsedTime = elapsedTime;
            return accessors;
        }
    }
}
