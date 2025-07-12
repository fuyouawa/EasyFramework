using System;

namespace EasyToolKit.ToolKit
{
    /// <summary>
    /// 对象池的基类，实现了IObjectPool接口的基本功能
    /// </summary>
    public abstract class ObjectPoolBase : IObjectPool
    {
        /// <inheritdoc />
        string IObjectPool.Name
        {
            get => _name;
            set => _name = value;
        }

        /// <inheritdoc />
        Type IObjectPool.ObjectType
        {
            get => _objectType;
            set
            {
                // 检查 objectType 是否可被实例化
                if (value.IsInterface || value.IsAbstract || value.IsGenericType)
                {
                    throw new InvalidOperationException(
                        $"Type '{value}' cannot be interface, abstract or generic type.");
                }

                _objectType = value;
            }
        }

        private string _name;
        private Type _objectType;

        /// <summary>
        /// 对象池的名称
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// 对象池中存储的对象类型
        /// </summary>
        public Type ObjectType => _objectType;

        public int TotalCount => ActiveCount + IdleCount;

        public abstract int ActiveCount { get; }
        public abstract int IdleCount { get; }

        private int _capacity = -1;

        /// <summary>
        /// <para>对象池的容量上限。设置小于0表示无限制</para>
        /// <para>当设置新的容量时，如果新容量小于当前活跃对象数量，将抛出异常。</para>
        /// </summary>
        public int Capacity
        {
            get => _capacity;
            set
            {
                if (_capacity == value)
                {
                    return;
                }

                if (value < 0)
                {
                    _capacity = value;
                    return;
                }

                if (value < ActiveCount)
                {
                    throw new InvalidOperationException(
                        $"Capacity '{value}' cannot be less than active object count '{ActiveCount}'.");
                }

                _capacity = value;

                if (TotalCount > _capacity)
                {
                    ShrinkIdleObjectsToFitCapacity(TotalCount - _capacity);
                }
            }
        }

        // 对象生成和回收时的回调函数
        private Action<object> _onRent;
        private Action<object> _onRelease;

        void IObjectPool.AddRentCallback(Action<object> callback)
        {
            _onRent += callback;
        }

        void IObjectPool.AddReleaseCallback(Action<object> callback)
        {
            _onRelease += callback;
        }

        public object Rent()
        {
            if (_capacity >= 0)
            {
                if (TotalCount >= _capacity && IdleCount == 0)
                {
                    throw new InvalidOperationException($"No idle object could be rented from pool '{Name}' because the pool has reached its capacity limit.");
                }
            }

            var instance = RentFromIdle();
            
            OnRent(instance);
            _onRent?.Invoke(instance);
            return instance;
        }

        public bool Release(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var result = ReleaseToIdle(instance);
            
            if (result)
            {
                OnRelease(instance);
                _onRelease?.Invoke(instance);
            }
            
            return result;
        }

        public bool Remove(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            return RemoveFromActive(instance);
        }

        /// <summary>
        /// 从空闲对象池中获取一个对象实例
        /// </summary>
        /// <returns>获取到的对象实例</returns>
        /// <exception cref="InvalidOperationException">当无法获取对象实例时抛出</exception>
        protected abstract object RentFromIdle();
        
        /// <summary>
        /// 将对象实例释放回空闲对象池
        /// </summary>
        /// <param name="instance">要释放的对象实例</param>
        /// <returns>如果对象成功释放，则返回true；如果对象已经在空闲池中或不属于此对象池，则返回false</returns>
        /// <exception cref="InvalidOperationException">当因其他原因无法释放对象时抛出</exception>
        protected abstract bool ReleaseToIdle(object instance);
        
        /// <summary>
        /// 从活跃对象列表中移除对象实例
        /// </summary>
        /// <param name="instance">要移除的对象实例</param>
        /// <returns>如果对象成功移除，则返回true；如果对象不在活跃列表中，则返回false</returns>
        /// <exception cref="InvalidOperationException">当因其他原因无法移除对象时抛出</exception>
        protected abstract bool RemoveFromActive(object instance);

        /// <summary>
        /// 根据容量限制收缩
        /// </summary>
        protected abstract void ShrinkIdleObjectsToFitCapacity(int shrinkCount);

        protected virtual void OnRent(object instance) {}
        protected virtual void OnRelease(object instance) {}
    }
}
