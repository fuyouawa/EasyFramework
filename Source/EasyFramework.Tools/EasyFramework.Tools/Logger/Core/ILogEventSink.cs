namespace EasyFramework.Tools
{
    public interface ILogEventSink
    {
        public void Emit(LogEvent e);
    }
}
