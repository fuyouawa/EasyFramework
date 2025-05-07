namespace EasyFramework.Core
{
    public interface IController : IBelongToArchitecture, ICanSendCommand, ICanGetModel,
        ICanRegisterEvent, ICanSendQuery, ICanGetSystem
    {
    }
}
