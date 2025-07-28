using ExprodesC.Models;
using ExprodesC.ViewModels;
using ExprodesC.ViewModels.Settings;

namespace ExprodesC.Services;

public static class ServicesEx
{
    public static IServiceCollection AddPagesVM(this IServiceCollection sc) =>
        sc.AddSingleton<CalcPageVM>()
        .AddSingleton<SettingsPageVM>()
        .AddSingleton<ComparePageVM>()
        .AddSingleton<MainPageVM>(p => new MainPageVM(p.GetRequiredService<IProject>(), p.GetRequiredService<IDialogService>()))
        .AddSingleton<SettingsPageVM>();

}
