using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace EasyFramework.ToolKit
{
    /// <summary>
    /// 对象池的基类，实现了IObjectPool接口的基本功能
    /// </summary>
    public abstract class ObjectPoolBase : IObjectPool
    {
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public Type ObjectType { get; }

        /// <summary>
        /// 当前对象池中的对象总数（活跃对象 + 未使用对象）
        /// </summary>
        public int Count => _activeInstances.Count + _unusedInstances.Count;

        private int _capacity;

        /// <summary>
        /// 对象池的容量上限。设置为0表示无限制。
        /// 当设置新的容量时，如果新容量小于当前活跃对象数量，将抛出异常。
        /// </summary>
        public int Capacity
        {
            get => _capacity;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"Capacity '{value}' cannot be negative.");
                }

                if (_capacity == value)
                {
                    return;
                }

                if (value > 0 && value < _activeInstances.Count)
                {
                    throw new InvalidOperationException($"Capacity '{value}' cannot be less than spawned object count '{_activeInstances.Count}'.");
                }

                _capacity = value;

                ShrinkUnusedObjectsToFitCapacity();
            }
        }

        // 存储活跃的对象实例
        private readonly List<object> _activeInstances = new List<object>();
        // 存储未使用的对象实例
        private readonly Stack<object> _unusedInstances = new Stack<object>();

        // 对象生成和回收时的回调函数
        private Action<object> _onSpawn;
        private Action<object> _onRecycle;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <param name="objectType">对象池中存储的对象类型</param>
        /// <exception cref="ArgumentNullException">当objectType为null时抛出</exception>
        protected ObjectPoolBase(string name, Type objectType)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException(nameof(objectType));
            }
            Name = name;
            ObjectType = objectType;
        }

        void IObjectPool.AddSpawnCallback(Action<object> callback)
        {
            _onSpawn += callback;
        }

        void IObjectPool.AddRecycleCallback(Action<object> callback)
        {
            _onRecycle += callback;
        }

        /// <summary>
        /// 尝试从对象池中获取一个对象
        /// </summary>
        /// <returns>获取到的对象，如果对象池已满则返回null</returns>
        public object TrySpawn()
        {
            object instance;
            if (_unusedInstances.Count > 0)
            {
                instance = _unusedInstances.Pop();
            }
            else
            {
                if (_capacity > 0 && Count >= _capacity)
                {
                    return null;
                }
                instance = GetNewObject();
            }

            _activeInstances.Add(instance);

            OnSpawn(instance);
            _onSpawn?.Invoke(instance);

            return instance;
        }

        // /// <summary>
        // /// 异步尝试从对象池中获取一个对象
        // /// </summary>
        // /// <returns>获取到的对象，如果对象池已满则返回null</returns>
        // public async UniTask<object> TrySpawnAsync()
        // {
        //     object instance;
        //     if (_unusedInstances.Count > 0)
        //     {
        //         instance = _unusedInstances.Pop();
        //     }
        //     else
        //     {
        //         if (_capacity > 0 && Count >= _capacity)
        //         {
        //             return null;
        //         }
        //
        //         instance = await GetNewObjectAsync();
        //     }
        //
        //     _activeInstances.Add(instance);
        //
        //     OnSpawn(instance);
        //     _onSpawn?.Invoke(instance);
        //     return instance;
        // }
        
        /// <summary>
        /// 尝试将对象回收到对象池中
        /// </summary>
        /// <param name="instance">要回收的对象</param>
        /// <returns>回收是否成功</returns>
        /// <exception cref="ArgumentNullException">当instance为null时抛出</exception>
        public virtual bool TryRecycle(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (!CanRecycle(instance))
            {
                return false;
            }

            if (!_activeInstances.Remove(instance))
            {
                return false;
            }

            OnRecycle(instance);

            _onRecycle?.Invoke(instance);

            if (_capacity == 0 || Count < _capacity)
            {
                _unusedInstances.Push(instance);
            }

            return true;
        }

        /// <summary>
        /// 检查对象是否可以被回收
        /// </summary>
        /// <param name="instance">要检查的对象</param>
        /// <returns>对象是否可以被回收</returns>
        protected virtual bool CanRecycle(object instance) => true;

        /// <summary>
        /// 创建新的对象实例
        /// </summary>
        /// <returns>新创建的对象实例</returns>
        protected abstract object GetNewObject();

        // /// <summary>
        // /// 异步创建新的对象实例
        // /// </summary>
        // /// <returns>新创建的对象实例</returns>
        // protected abstract UniTask<object> GetNewObjectAsync();

        /// <summary>
        /// 对象生成时的回调方法
        /// </summary>
        /// <param name="instance">生成的对象实例</param>
        protected virtual void OnSpawn(object instance) {}

        /// <summary>
        /// 对象回收时的回调方法
        /// </summary>
        /// <param name="instance">回收的对象实例</param>
        protected virtual void OnRecycle(object instance) {}

        /// <summary>
        /// 根据容量限制收缩未使用的对象
        /// </summary>
        private void ShrinkUnusedObjectsToFitCapacity()
        {
            if (_capacity == 0) return;

            while (Count > _capacity && _unusedInstances.Count > 0)
            {
                _unusedInstances.Pop();
            }
        }
    }
}
