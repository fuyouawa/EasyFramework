namespace EasyFramework
{
    public interface IController : IBelongToArchitecture, ICanSendCommand, ICanGetModel,
        ICanRegisterEvent, ICanSendQuery, ICanGetSystem
    {
    }
}
