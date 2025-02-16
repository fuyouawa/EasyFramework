namespace EasyFramework
{
    public static class SinkExtension
    {
        public static LoggerConfiguration UnityConsole(this LoggerSinkConfiguration configuration)
        {
            return configuration.Sink(new UnityConsoleLogEventSink(UnityEngine.Debug.unityLogger));
        }
    }
}
