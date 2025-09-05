using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ExprodesC.Imp;
using ExprodesC.ViewModels;
using Func.Services;

namespace ExprodesC.Views.Pages;

public partial class OptionalPanel : UserControl
{
    public OptionalPanel()
    {
        InitializeComponent();
        this.DataContext = App.Services.GetRequiredService<ISettingsProvider<ExSettingData>>().Value;

    }
}