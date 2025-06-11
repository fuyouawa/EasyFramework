using System;
using UnityEngine;

namespace EasyFramework.Core
{

    public interface IUnityObjectPool : IObjectPool
    {
        float DefaultObjectLifetime { get; set; }
        Type DefaultPooledComponentType { get; set; }

        float RecycleInterval { get; set; }
        GameObject Original { get; }
        Transform Transform { get; set; }

        internal void Update(float deltaTime);
    }
}
