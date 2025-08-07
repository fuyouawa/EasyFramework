namespace EasyToolKit.Core
{
    public interface IEasyEventListener<TEvent>
    {
        void OnEvent(object sender, TEvent eventArg);
    }

    public static class EasyEventListener
    {
        public static IFromRegisterEvent RegisterListener<TEvent>(this IEasyEventListener<TEvent> listener)
        {
            return EasyEventManager.Instance.Register<TEvent>(listener, listener.OnEvent);
        }

        public static bool UnregisterListener<TEvent>(this IEasyEventListener<TEvent> listener)
        {
            return EasyEventManager.Instance.Unregister<TEvent>(listener, listener.OnEvent);
        }
    }
}