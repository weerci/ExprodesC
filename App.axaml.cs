using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Calc;
using Calc.Data;
using Calc.Imp;
using Calc.Models;
using ExprodesC.Imp;
using ExprodesC.Models;
using ExprodesC.Services;
using ExprodesC.ViewModels;
using ExprodesC.Views;
using Func;
using Func.Impl;
using Func.Services;
using Microsoft.Extensions.DependencyInjection;
using Splat;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
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
            .AddSingleton<IJsonTypeInfoResolver, SourceGenerationContext>()
            .AddSingleton<ISettingsProvider<AppSettingsData>, SettingsProvider<AppSettingsData>>()
            .AddSingleton<ISettingsProvider<ExSettingData>>(p => {
                var settings = new ExSettingData();
                var sp = new SettingsProvider<ExSettingData>(
                    p.GetRequiredService<ISerializationService>(),
                    p.GetRequiredService<IJsonTypeInfoResolver>());
                sp.Load(settings.GetSettingFile);
                return sp;
                })
            .AddSingleton<IDialogService, DialogService>()
            .AddSingleton<ICalcDb, CalcDb>()
            .AddSingleton<IAppDb,   AppDb>()
            .AddSingleton<IGenotypeStore, GenotypeStore>()
            .AddSingleton<IProject, Project>()
            .AddSingleton<ICounter, Counter>()
            .AddPagesVM()
            .AddSingleton(p => new MainViewVM(p.GetService<IDialogService>()!, p.GetService<IProject>()!));

        Services = services.BuildServiceProvider();

        HL.Initialize(Services.GetRequiredService<IAppDb>(), Services.GetRequiredService<ISettingsProvider<AppSettingsData>>());
        App.Current!.RequestedThemeVariant = Services.GetRequiredService<ISettingsProvider<ExSettingData>>().Value.CurrentTheme.Theme();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = MainWindow = new MainWindow
            {
                DataContext = Services.GetService<MainViewVM>(),
            };
        }
        //TODO MainView Не работает нулевой уровень  для популяций

        base.OnFrameworkInitializationCompleted();
    }

}
[JsonSerializable(typeof(AppSettingsData))]
[JsonSerializable(typeof(ExSettingData))]
public partial class SourceGenerationContext : JsonSerializerContext
{

}