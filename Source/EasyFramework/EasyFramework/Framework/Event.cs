namespace EasyFramework
{
    public interface ICanRegisterEvent : IBelongToArchitecture
    {
    }

    public static class CanRegisterEventExtension
    {
        public static IFromRegisterEvent RegisterEvent<T>(this ICanRegisterEvent self, EventHandlerDelegate<T> onEvent) =>
            self.GetArchitecture().RegisterEvent<T>(onEvent);

        public static void UnRegisterEvent<T>(this ICanRegisterEvent self, EventHandlerDelegate<T> onEvent) =>
            self.GetArchitecture().UnRegisterEvent<T>(onEvent);
    }

    public interface ICanSendEvent : IBelongToArchitecture
    {
    }

    public static class CanSendEventExtension
    {
        public static void SendEvent<T>(this ICanSendEvent self, T e) => self.GetArchitecture().SendEvent<T>(e);
    }

}
