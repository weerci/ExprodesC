using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using ExprodesC.ViewModels;
using ExprodesC.ViewModels.Settings;
using ExprodesC.Views.Wrappers;
using static ExprodesC.ViewModels.MainPageVM;

namespace ExprodesC;

public partial class ComparePage : UserControl
{
    public ComparePage()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<ComparePageVM>();
        tvGenotypes.DoubleTapped += OnTreeDoubleTapped;
    }

    private void OnTreeDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is ComparePageVM vm &&
            tvGenotypes.SelectedItem is Node genotype)
        {
            vm.AddColumnCommand?.Execute(genotype.GenotypeWR).Subscribe();
        }
    }
}