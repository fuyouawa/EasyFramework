namespace EasyFramework.Core
{
    public interface ICanRegisterEvent : IBelongToArchitecture
    {
    }

    public static class CanRegisterEventExtension
    {
        public static IFromRegisterEvent RegisterEvent<T>(this ICanRegisterEvent self, EasyEventHandler<T> onEvent) =>
            self.GetArchitecture().RegisterEvent<T>(onEvent);

        public static void UnregisterEvent<T>(this ICanRegisterEvent self, EasyEventHandler<T> onEvent) =>
            self.GetArchitecture().UnregisterEvent<T>(onEvent);
    }

    public interface ICanSendEvent : IBelongToArchitecture
    {
    }

    public static class CanSendEventExtension
    {
        public static void SendEvent<T>(this ICanSendEvent self, T e) => self.GetArchitecture().SendEvent<T>(e);
    }

}
