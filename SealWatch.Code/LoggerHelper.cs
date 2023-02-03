using Serilog;

namespace SealWatch.Code;

public static class LoggerHelper
{
    public static void StartLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Log.Warning("Logger started");
    }
    
    public static void StopLogger()
    {
        Log.Warning("Logger stopped");
        Log.CloseAndFlush();
    }
}