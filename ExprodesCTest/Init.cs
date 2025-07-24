using Calc.Data;
using Calc.Imp;
using Calc.Models;
using ExprodesC.Imp;
using ExprodesC.Models;
using ExprodesC.Services;
using ExprodesC.ViewModels;
using Func.Impl;
using Func.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization.Metadata;

namespace ExprodesCTest
{
    public static class Init
    {
        public static ServiceProvider Services;

        static Init()
        {

            IServiceCollection? services = new ServiceCollection()
                .AddTransient<ISerializationService, SerializationService>()
                .AddTransient<IJsonTypeInfoResolver, SerializeContext>()
                .AddTransient<ISettingsProvider<AppSettingsData>, SettingsProvider<AppSettingsData>>()
                .AddTransient<ISettingsProvider<ExSettingData>, SettingsProvider<ExSettingData>>()
                .AddTransient<IDialogService, DialogService>()
                .AddTransient<ICalcDb, CalcDb>()
                .AddTransient<IGenotypeStore, GenotypeStore>()
                .AddSingleton<IProject, Project>()
                .AddPagesVM()
                .AddSingleton(p => new MainViewVM(p.GetService<IDialogService>()!, p.GetService<IProject>()!));

            Services = services.BuildServiceProvider();
        }

    }


}
