using Avalonia.Controls;
using ExprodesC.Views.Pages;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Media.Animation;

namespace ExprodesC.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void NavigationView_SelectionChanged(object? sender, FluentAvalonia.UI.Controls.NavigationViewSelectionChangedEventArgs e)
    {
        if (e.SelectedItem is NavigationViewItem si)
        {
            if (e.SelectedItemContainer.Name == "nviSettings")
                contentFrame.Navigate(typeof(SettingsPage), null, new EntranceNavigationTransitionInfo() { FromHorizontalOffset = 200, FromVerticalOffset = 0 }); 
            else if (e.SelectedItemContainer.Name == "nviCalc")
                contentFrame.Navigate(typeof(CalcPage), null, new EntranceNavigationTransitionInfo() { FromHorizontalOffset = 200, FromVerticalOffset = 0 });
            else if (e.SelectedItemContainer.Name == "nviHome")
                contentFrame.Navigate(typeof(MainPage), null, new EntranceNavigationTransitionInfo() { FromHorizontalOffset = 200, FromVerticalOffset = 0 });
            else if (e.SelectedItemContainer.Name == "nviLib")
                contentFrame.Navigate(typeof(LibraryPage), null, new EntranceNavigationTransitionInfo() { FromHorizontalOffset = 200, FromVerticalOffset = 0 });
        }
    }
}