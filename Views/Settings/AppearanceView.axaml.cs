using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ExprodesC.Imp;
using ExprodesC.Models;
using ExprodesC.Services;
using Func;
using Func.Impl;
using Func.Services;
using System.Linq;

namespace ExprodesC;

public partial class AppearanceView : UserControl
{
    ISettingsProvider<ExSettingData> _settings = null!;
    public AppearanceView()
    {
        InitializeComponent();
    }

    public AppearanceView(ISettingsProvider<ExSettingData> settings) : this()
    {
        _settings = settings;

        cbTheme.ItemsSource = ExSettingData.Themes;

        cbTheme.ItemTemplate = new FuncDataTemplate<Themes>((theme, _) =>
        {
            return new TextBlock { Text = theme.Name };
        });

        cbTheme.SelectedItem = _settings.Value.CurrentTheme;
        cbTheme.SelectionChanged += CbTheme_SelectionChanged;   
    }

    private void CbTheme_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (cbTheme.SelectedItem is Themes selectedTheme)
        {
            App.Current!.RequestedThemeVariant = selectedTheme.Theme();
            _settings.Value.CurrentTheme = selectedTheme;
            _settings.Save(_settings.Value.GetSettingFile);
        }
    }

}