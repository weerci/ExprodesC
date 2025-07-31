using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using ExprodesC.ViewModels;
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

    private void OnTreeDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is MainPageVM vm && tvGenotypes.SelectedItem is Node genotype)
            vm.AddColumnCommand?.Execute(genotype.GenotypeWR).Subscribe();
    }
}