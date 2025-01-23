namespace EasyFramework
{
    public interface ILogEventSink
    {
        public void Emit(LogEvent e);
    }
}
