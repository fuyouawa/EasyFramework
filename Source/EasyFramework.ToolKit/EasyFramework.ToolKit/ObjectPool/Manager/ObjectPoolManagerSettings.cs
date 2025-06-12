using System;

namespace EasyFramework.ToolKit
{
    /// <summary>
    /// 对象池管理器的配置设置类
    /// </summary>
    public class ObjectPoolManagerSettings
    {
        private Type _poolType;

        /// <summary>
        /// 对象池类型，用于创建对象池实例
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// 当value为null，
        /// 或者是抽象类或接口，
        /// 或者没有公开的构造函数(string name, Type objectType)时抛出
        /// </exception>
        public Type PoolType
        {
            get => _poolType;
            set
            {
                if (value == null)
                {
                    throw new InvalidOperationException("PoolType cannot be null.");
                }

                if (value.IsAbstract || value.IsInterface)
                {
                    throw new InvalidOperationException($"PoolType '{value}' cannot be abstract or interface.");
                }

                // 检查是否有公开的构造函数(string name, Type objectType)
                var constructor = value.GetConstructor(new[] { typeof(string), typeof(Type) });
                if (constructor == null)
                {
                    throw new InvalidOperationException($"PoolType '{value}' must have a public constructor with parameters (string name, Type objectType).");
                }

                _poolType = value;
            }
        }
    }
}
