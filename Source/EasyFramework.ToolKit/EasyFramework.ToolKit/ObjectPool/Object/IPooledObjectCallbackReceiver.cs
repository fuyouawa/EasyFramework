namespace EasyFramework.ToolKit
{
    public interface IPooledObjectCallbackReceiver
    {
        void OnRent(IObjectPool owningPool);
        void OnRelease(IObjectPool owningPool);
    }
}
