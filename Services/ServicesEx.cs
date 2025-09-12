using Calc;
using ExprodesC.Imp;
using ExprodesC.Models;
using ExprodesC.ViewModels;
using ExprodesC.ViewModels.Settings;
using ExprodesC.Views.Pages;
using ExprodesC.Views.Synonym;
using Func.Services;
using System.Diagnostics.Metrics;

namespace ExprodesC.Services;

public static class ServicesEx
{
    public static IServiceCollection AddPagesVM(this IServiceCollection sc) =>
        sc.AddSingleton<SettingsPageVM>()
        .AddSingleton<MainPageVM>(p => new MainPageVM(
            p.GetRequiredService<IProject>(), 
            p.GetRequiredService<IDialogService>(),
            p.GetRequiredService<ISettingsProvider<ExSettingData>>()))
        .AddSingleton<CalcPageVM>()
        .AddSingleton<LibraryPageVM>()
        .AddSingleton<SynonymVM>();

}
