using UnityEngine;

namespace EasyFramework.Core
{
    public interface IPooledUnityObject : IPooledObject
    {
        float Lifetime { get; set; }
        Transform Transform { get; }

        void Tick(float interval);
    }
}
