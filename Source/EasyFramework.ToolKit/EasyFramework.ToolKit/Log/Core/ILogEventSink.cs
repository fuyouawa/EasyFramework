namespace EasyFramework.ToolKit
{
    public interface ILogEventSink
    {
        public void Emit(LogEvent e);
    }
}
