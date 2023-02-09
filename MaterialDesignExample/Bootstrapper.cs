using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SealWatch.Code;
using SealWatch.Code.CutterLayer;
using SealWatch.Code.CutterLayer.Interfaces;
using SealWatch.Code.HistoryLayer;
using SealWatch.Code.ProjectLayer;
using SealWatch.Code.Services;
using SealWatch.Data.Database;
using SealWatch.Wpf.Config;
using SealWatch.Wpf.Service;
using SealWatch.Wpf.Service.Interfaces;
using SealWatch.Wpf.ViewModels;
using SealWatch.Wpf.ViewModels.Dialogs;
using SealWatch.Wpf.Views;
using SealWatch.Wpf.Views.Dialogs;
using Serilog;
using Serilog.Extensions.Logging;
using System;

namespace SealWatch.Wpf;

public static class Bootstrapper
{
    private static IContainer? _container;

    public static void Start()
    {
        var builder = new ContainerBuilder()
            .ConfigureLogging()
            .Setup();

        _container = builder.Build();

        Mock();
    }

    public static void Stop()
    {
        _container?.Dispose();
    }

    public static T Resolve<T>() => _container!.Resolve<T>();


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
        //  AppSettings

        var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false);
        var config = configBuilder.Build();

        var appSettingsSetup = () =>
        {
            var c = new AppSettings();
            config.GetSection(nameof(AppSettings)).Bind(c);
            return c;
        };
        AppSettings appSettings = appSettingsSetup.Invoke();
        builder.RegisterInstance(appSettings).As<AppSettings>();
        builder.RegisterInstance(config).AsImplementedInterfaces();

        //  Services

        builder.RegisterType<AnalyseService>().SingleInstance();

        builder.RegisterType<UserInputService>().AsImplementedInterfaces();
        builder.RegisterType<DesignService>().AsImplementedInterfaces();
        builder.RegisterType<GraphsService>().AsImplementedInterfaces();
        builder.RegisterType<CalendarService>().As<ICalendarService>();

        builder.RegisterType<ProjectAccessLayer>().AsImplementedInterfaces();
        builder.RegisterType<HistoryAccessLayer>().AsImplementedInterfaces();
        builder.Register<ICutterAccessLayer>(x => new CutterAccessLayer(appSettings.Accuracy));

        builder.RegisterType<Random>().SingleInstance();

        //  Windows

        builder.RegisterType<DashboardWindowViewModel>().SingleInstance();
        builder.RegisterType<DashboardWindow>().SingleInstance();

        builder.RegisterType<SplashScreenViewModel>().SingleInstance();
        builder.RegisterType<SplashScreen>().SingleInstance();

        //  UserControls

        builder.RegisterType<ProjectViewModel>().SingleInstance();
        builder.RegisterType<ProjectsView>().SingleInstance();

        builder.RegisterType<AnalyticViewModel>().InstancePerDependency();
        builder.RegisterType<AnalyticView>().InstancePerDependency();

        builder.RegisterType<CalendarPlanerViewModel>().SingleInstance();
        builder.RegisterType<CalendarPlanerView>().SingleInstance();

        //  Dialogs

        builder.RegisterType<CreateOrUpdateCutterViewModel>().SingleInstance();
        builder.RegisterType<CreateOrUpdateCutterView>().SingleInstance();

        builder.RegisterType<CreateOrUpdateProjectViewModel>().SingleInstance();
        builder.RegisterType<CreateOrUpdateView>().SingleInstance();

        builder.RegisterType<DetailsViewModel>().SingleInstance();
        builder.RegisterType<DetailsView>().SingleInstance();

        return builder;
    }

    private static void Mock() => MockHelper.Mock(SealWatchDbContext.NewContext());
}