namespace EasyToolKit.Core
{
    public interface IEasyEventTrigger
    {
    }

    public static class EasyEventTrigger
    {
        public static void TriggerEvent<TEvent>(this IEasyEventTrigger trigger, TEvent eventArg)
        {
            EasyEventManager.Instance.TriggerEvent(trigger, eventArg);
        }
    }
}