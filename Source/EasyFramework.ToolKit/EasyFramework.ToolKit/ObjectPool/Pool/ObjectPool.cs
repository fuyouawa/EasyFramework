using System;
using System.Collections.Generic;
using System.Drawing;
using Cysharp.Threading.Tasks;

namespace EasyFramework.ToolKit
{
    /// <summary>
    /// 通用对象池实现，用于管理实现了IPooledObject接口的对象
    /// </summary>
    public class ObjectPool : ObjectPoolBase
    {
        // 存储活跃的对象实例
        private readonly List<object> _activeInstances = new List<object>();

        // 存储未使用的对象实例
        private readonly Stack<object> _availableInstances = new Stack<object>();

        public override int ActiveCount => _activeInstances.Count;
        public override int AvailableCount => _availableInstances.Count;

        protected override object TryRentFromAvailable()
        {
            object instance;
            if (_availableInstances.Count > 0)
            {
                instance = _availableInstances.Pop();
            }
            else
            {
                instance = GetNewObject();
            }

            _activeInstances.Add(instance);
            return instance;
        }
        
        protected object GetNewObject()
        {
            return Activator.CreateInstance(ObjectType);
        }

        protected override bool TryReleaseToAvailable(object instance)
        {
            if (!CanRecycle(instance))
            {
                return false;
            }

            if (!_activeInstances.Remove(instance))
            {
                return false;
            }
            
            _availableInstances.Push(instance);
            return true;
        }

        protected override bool TryRemoveFromActive(object instance)
        {
            return _activeInstances.Remove(instance);
        }

        protected override void ShrinkAvailableObjectsToFitCapacity(int shrinkCount)
        {
            for (int i = 0; i < shrinkCount; i++)
            {
                _availableInstances.Pop();
            }
        }

        protected bool CanRecycle(object instance)
        {
            if (instance.GetType() != ObjectType)
            {
                return false;
            }
            return true;
        }

        protected override void OnRent(object instance)
        {
            if (instance is IPooledObjectCallbackReceiver receiver)
            {
                receiver.OnRent(this);
            }
        }

        protected override void OnRelease(object instance)
        {
            if (instance is IPooledObjectCallbackReceiver receiver)
            {
                receiver.OnRelease(this);
            }
        }
    }
}
