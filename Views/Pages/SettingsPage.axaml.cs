using Avalonia.Controls;
using ExprodesC.ViewModels.Settings;

namespace ExprodesC.Views.Pages;

public partial class SettingsPage : UserControl
{
    public SettingsPage()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<SettingsPageVM>();
    }

}