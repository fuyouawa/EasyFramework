using UnityEngine;

namespace EasyFramework.ToolKit
{
    public interface IPooledUnityObject : IPooledObject
    {
        float Lifetime { get; set; }
        Transform Transform { get; }

        void Tick(float interval);
    }
}
