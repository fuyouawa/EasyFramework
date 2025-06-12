namespace EasyFramework.ToolKit
{
    public interface IPooledObject
    {
        IObjectPool OwningPool { get; internal set; }

        internal void OnSpawn();
        internal void OnRecycle();
    }
}
