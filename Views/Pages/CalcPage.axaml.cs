using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using ExprodesC.ViewModels;
using ExprodesC.Wrappers;
using Func.Extensions;

namespace ExprodesC.Views.Pages;

public partial class CalcPage : UserControl
{

    CalcPageVM _calcPageVM;

    public CalcPage()
    {
        InitializeComponent();
        this.DataContext = _calcPageVM = App.Services.GetRequiredService<CalcPageVM>();
        tvGenotypes.DoubleTapped += OnTreeDoubleTapped;
    }

    private void NavigationView_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        var ctl = sender as Control;
        if (ctl != null)
        {
            FlyoutBase.ShowAttachedFlyout(ctl);
        }
    }

    private void OnTreeDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (tvGenotypes.SelectedItem is GenotypeNode node)
            _calcPageVM.SelectGenotypeCommand.Execute(node.GenotypeWR).Subscribe();
    }
}