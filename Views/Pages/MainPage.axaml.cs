using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ExprodesC.ViewModels;

namespace ExprodesC.Views.Pages;

public partial class MainPage : UserControl
{
    MainPageVM _mainPageVM = App.Services.GetRequiredService<MainPageVM>();

    public MainPage()
    {
        InitializeComponent();
        DataContext = _mainPageVM;
    }

    private void TextBlock_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        _mainPageVM.SelectGenotypeCommand.Execute().Subscribe();
    }
}