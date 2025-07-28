using Avalonia.Controls;
using ExprodesC.Views.Pages;
using FluentAvalonia.UI.Controls;

namespace ExprodesC.Views;

public partial class MainView : UserControl
{
    MainPage _mainPage = null!;
    CalcPage _calcPage = null!;
    ComparePage _comparePage = null!;
    //LibraryPage _libraryPage = null!;
    SettingsPage _settingsPage = null!;
    //HelpPage _helpPage = null!;

    public MainView()
    {
        InitializeComponent();
    }

    private void NavigationView_SelectionChanged(object? sender, FluentAvalonia.UI.Controls.NavigationViewSelectionChangedEventArgs e)
    {
        if (e.SelectedItem is NavigationViewItem si)
        {
            nvMain.Content = si.Name switch
            {
                "nviSettings" => _settingsPage ??= new SettingsPage(),
                "nviCalc" => _calcPage ??= new CalcPage(),
                "nviCompare" => _comparePage ??= new ComparePage(),
                //"nviLib" => _libraryPage ??= new LibraryPage(),
                //"nviHelp" => _helpPage ??= new HelpPage(),
                _ => _mainPage ??= new MainPage(),
            };
        }
    }
}