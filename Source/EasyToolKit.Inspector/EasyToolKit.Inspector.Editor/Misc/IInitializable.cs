namespace EasyToolKit.Inspector.Editor
{
    public interface IInitializable
    {
        bool IsInitialized { get; }
        void Initialize();
        void Deinitialize();
    }
}
