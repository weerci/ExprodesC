using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using ExprodesC.ViewModels;
using ExprodesC.Wrappers;
using Func.Extensions;
using System.Reactive.Linq;
using System.Windows.Input;
using static ExprodesC.ViewModels.MainPageVM;

namespace ExprodesC.Views.Pages;

public partial class MainPage : UserControl
{
    MainPageVM _mainPageVM = App.Services.GetRequiredService<MainPageVM>();

    public MainPage()
    {
        InitializeComponent();
        DataContext = _mainPageVM;
        tvGenotypes.DoubleTapped += OnTreeDoubleTapped;
    }
    //TODO изменить стили из xaml behaviors то ли оставить как есть, то ли реалзиовать стиль в своей программе, то ли полностью перенести функционал
    private void OnTreeDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (tvGenotypes.SelectedItem is GenotypeNode node)
            _mainPageVM.SelectGenotypeCommand.ExecuteIfPossible(node.GenotypeWR);
    }
}