using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SealWatch.Main.Config;
using Serilog;
using Serilog.Extensions.Logging;

namespace SealWatch.Main;

internal static class Bootstrapper
{
    private static IContainer? _container;

    public static void Start()
    {
        var builder = new ContainerBuilder()
            .Config()
            .ConfigureLogging()
            .Setup();

        _container = builder.Build();
    }

    public static T Resolve<T>() => _container.Resolve<T>();

    private static ContainerBuilder Config(this ContainerBuilder builder)
    {
        var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false);

        var config = configBuilder.Build();

        builder.Register(x =>
        {
            var c = new AppSettings();
            config.GetSection(nameof(AppSettings)).Bind(c);
            return c;
        });
        builder.RegisterInstance(config).AsImplementedInterfaces();

        return builder;
    }

    public static ContainerBuilder ConfigureLogging(this ContainerBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
            .WriteTo.Console()
            .CreateLogger();

        builder.Register(_ => new LoggerFactory(new ILoggerProvider[] { new SerilogLoggerProvider() }))
            .As<ILoggerFactory>()
            .SingleInstance();
        builder.RegisterGeneric(typeof(Logger<>))
            .As(typeof(ILogger<>));

        return builder;
    }


    private static ContainerBuilder Setup(this ContainerBuilder builder)
    {
        builder.RegisterType<MainWindow>().SingleInstance();
        return builder;
    }

    public static void Stop() => _container?.Dispose();
}