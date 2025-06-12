namespace EasyFramework.ToolKit
{
    public class AbstractPooledObject : IPooledObject
    {
        IObjectPool IPooledObject.OwningPool
        {
            get => OwningPool;
            set => OwningPool = value;
        }

        public IObjectPool OwningPool { get; private set; }

        void IPooledObject.OnSpawn()
        {
            OnSpawn();
        }

        void IPooledObject.OnRecycle()
        {
            OnRecycle();
        }

        protected virtual void OnSpawn()
        {
        }

        protected virtual void OnRecycle()
        {
        }
    }
}
