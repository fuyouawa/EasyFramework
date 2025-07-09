namespace EasyToolKit.Logging
{
    public interface ILogEventSink
    {
        public void Emit(LogEvent e);
    }
}
