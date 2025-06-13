namespace EasyFramework.ToolKit
{
    public interface IPooledObjectCallbackReceiver
    {
        void OnSpawn(IObjectPool owningPool);
        void OnRecycle(IObjectPool owningPool);
    }
}
