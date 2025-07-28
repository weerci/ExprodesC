using Avalonia.Controls;
using ExprodesC.Services;
using System.Linq;

namespace ExprodesC;

public partial class AppearanceView : UserControl
{
    IAppHost _appHost = null!;
    public AppearanceView()
    {
        InitializeComponent();
    }

    public AppearanceView(IAppHost appHost) : this()
    {
        _appHost = appHost;
        cbTheme.ItemsSource = Themes.Items.Select(i => i.Name);
        cbTheme.SelectedIndex = _appHost.CurrentTheme.Id;
        cbTheme.SelectionChanged += CbTheme_SelectionChanged;
    }

    private void CbTheme_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _appHost.CurrentTheme = Themes.Items.FirstOrDefault(i => i.Id == cbTheme.SelectedIndex, Themes.Default);
    }

}