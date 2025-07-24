using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Calc.Data;
using Calc.Imp;
using Calc.Models;
using ExprodesC.Imp;
using ExprodesC.Models;
using ExprodesC.Services;
using ExprodesC.ViewModels;
using ExprodesC.Views;
using Func.Impl;
using Func.Services;
using Splat;
using System.Globalization;
using System.Text.Json.Serialization.Metadata;

namespace ExprodesC;

public partial class App : Application
{
    public static ServiceProvider Services { get; private set; } = null!;
    public static MainWindow? MainWindow { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Dispatcher.UIThread.UnhandledException += (sender, e) =>
        {
            Program.GlobalErrorHandler(e.Exception);
            e.Handled = true;
        };

        Lang.Resources.Culture = new CultureInfo("ru-RU");

        IServiceCollection? services = new ServiceCollection()
            .AddSingleton<ISerializationService, SerializationService>()
            .AddSingleton<IJsonTypeInfoResolver, SerializeContext>()
            .AddSingleton<ISettingsProvider<AppSettingsData>, SettingsProvider<AppSettingsData>>()
            .AddSingleton<ISettingsProvider<ExSettingData>, SettingsProvider<ExSettingData>>()
            .AddSingleton<IDialogService, DialogService>()
            .AddSingleton<ICalcDb, CalcDb>()
            .AddSingleton<IGenotypeStore, GenotypeStore>()
            .AddSingleton<IProject, Project>()
            .AddPagesVM()
            .AddSingleton(p => new MainViewVM(p.GetService<IDialogService>()!, p.GetService<IProject>()!))
            .AddSingleton<IAppHost, AppHost>();

        Services = services.BuildServiceProvider();
        HL.Initialize(Services.GetRequiredService<ICalcDb>(), Services.GetRequiredService<ISettingsProvider<AppSettingsData>>());

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = MainWindow = new MainWindow
            {
                DataContext = Services.GetService<MainViewVM>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

}