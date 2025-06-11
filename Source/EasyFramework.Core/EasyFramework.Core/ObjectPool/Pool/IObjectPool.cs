using Cysharp.Threading.Tasks;
using System;

namespace EasyFramework.Core
{
    public interface IObjectPool
    {
        string Name { get; }
        Type ObjectType { get; }
        int Count { get; }
        int Capacity { get; set; }

        internal void AddSpawnCallback(Action<object> callback);
        internal void AddRecycleCallback(Action<object> callback);
        
        UniTask<object> TrySpawnAsync();
        object TrySpawn();

        bool TryRecycle(object instance);
    }
}
