namespace EasyToolKit.Framework
{
    public interface IController : IBelongToArchitecture, ICanSendCommand, ICanGetModel,
        ICanRegisterEvent, ICanSendQuery, ICanGetSystem
    {
    }
}
