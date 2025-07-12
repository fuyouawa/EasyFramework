using System;
using System.Collections.Generic;
using System.Drawing;
using Cysharp.Threading.Tasks;

namespace EasyToolKit.ToolKit
{
    /// <summary>
    /// 通用对象池实现，用于管理实现了IPooledObject接口的对象
    /// </summary>
    public class ObjectPool : ObjectPoolBase
    {
        // 存储活跃的对象实例
        private readonly List<object> _activeInstances = new List<object>();

        // 存储空闲的对象实例
        private readonly Stack<object> _idleInstances = new Stack<object>();

        public override int ActiveCount => _activeInstances.Count;
        public override int IdleCount => _idleInstances.Count;

        protected override object RentFromIdle()
        {
            object instance;
            if (_idleInstances.Count > 0)
            {
                instance = _idleInstances.Pop();
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

        protected override bool ReleaseToIdle(object instance)
        {
            if (!CanRecycle(instance))
            {
                throw new InvalidOperationException($"Instance type '{instance.GetType()}' does not match pool object type '{ObjectType}'.");
            }

            if (!_activeInstances.Remove(instance))
            {
                return false; // 对象已经不在活跃列表中
            }
            
            _idleInstances.Push(instance);
            return true;
        }

        protected override bool RemoveFromActive(object instance)
        {
            return _activeInstances.Remove(instance);
        }

        protected override void ShrinkIdleObjectsToFitCapacity(int shrinkCount)
        {
            for (int i = 0; i < shrinkCount; i++)
            {
                _idleInstances.Pop();
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
            if (instance is IPoolCallbackReceiver receiver)
            {
                receiver.OnRent(this);
            }
        }

        protected override void OnRelease(object instance)
        {
            if (instance is IPoolCallbackReceiver receiver)
            {
                receiver.OnRelease(this);
            }
        }
    }
}
