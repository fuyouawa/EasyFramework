using Serilog;
using Serilog.Configuration;

namespace EasyToolKit.GameConsole
{
    public static class GameConsoleSerilogExtension
    {
        public static LoggerConfiguration GameConsole(this LoggerSinkConfiguration configuration)
        {
            return configuration.Sink(new GameConsoleLogEventSink());
        }
    }
}
