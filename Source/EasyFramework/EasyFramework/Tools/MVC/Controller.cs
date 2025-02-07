namespace EasyFramework
{
    public interface IController : IBelongToArchitecture, ICanSendCommand, ICanGetSystem, ICanGetModel,
        ICanRegisterEvent, ICanGetUtility
    {
    }
}
