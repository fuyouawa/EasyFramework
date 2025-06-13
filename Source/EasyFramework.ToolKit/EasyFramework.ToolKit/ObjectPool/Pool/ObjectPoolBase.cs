using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EasyFramework.Core;

namespace EasyFramework.ToolKit
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

        public object TryRent()
        {
            if (_capacity >= 0)
            {
                if (TotalCount >= _capacity && IdleCount == 0)
                {
                    return null;
                }
            }

            var instance = TryRentFromIdle();
            OnRent(instance);
            _onRent?.Invoke(instance);
            return instance;
        }

        public virtual bool TryRelease(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (!TryReleaseToIdle(instance))
            {
                return false;
            }
            OnRelease(instance);
            _onRelease?.Invoke(instance);
            return true;
        }

        public bool TryRemove(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            return TryRemoveFromActive(instance);
        }

        protected abstract object TryRentFromIdle();
        protected abstract bool TryReleaseToIdle(object instance);
        protected abstract bool TryRemoveFromActive(object instance);

        /// <summary>
        /// 根据容量限制收缩
        /// </summary>
        protected abstract void ShrinkIdleObjectsToFitCapacity(int shrinkCount);

        protected virtual void OnRent(object instance) {}
        protected virtual void OnRelease(object instance) {}
    }
}
