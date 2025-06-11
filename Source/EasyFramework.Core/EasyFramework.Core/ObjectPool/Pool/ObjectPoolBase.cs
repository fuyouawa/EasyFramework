using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace EasyFramework.Core
{
    public abstract class ObjectPoolBase : IObjectPool
    {
        public string Name { get; }
        public Type ObjectType { get; }
        public int Count => _activeInstances.Count + _unusedInstances.Count;

        private int _capacity;

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

        private readonly List<object> _activeInstances = new List<object>();
        private readonly Stack<object> _unusedInstances = new Stack<object>();

        private Action<object> _onSpawn;
        private Action<object> _onRecycle;

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

            if (instance is IPooledObject pooled)
            {
                pooled.OwningPool = this;
            }

            OnSpawn(instance);
            _onSpawn?.Invoke(instance);

            return instance;
        }

        public async UniTask<object> TrySpawnAsync()
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

                instance = await GetNewObjectAsync();
            }

            _activeInstances.Add(instance);

            if (instance is IPooledObject pooled)
            {
                pooled.OwningPool = this;
            }

            OnSpawn(instance);
            _onSpawn?.Invoke(instance);
            return instance;
        }
        

        public bool TryRecycle(object instance)
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

        protected virtual bool CanRecycle(object instance) => true;

        protected abstract object GetNewObject();
        protected abstract UniTask<object> GetNewObjectAsync();

        protected virtual void OnSpawn(object instance) {}
        protected virtual void OnRecycle(object instance) {}

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
