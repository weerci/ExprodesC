using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Threading;
using ExprodesC.ViewModels;
using ExprodesC.Wrappers;
using Func.Extensions;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace ExprodesC.Views.Pages;

public partial class CalcPage : UserControl
{

    CalcPageVM _calcPageVM;

    public CalcPage()
    {
        InitializeComponent();
        this.DataContext = _calcPageVM = App.Services.GetRequiredService<CalcPageVM>();
        tvGenotypes.DoubleTapped += OnTreeDoubleTapped;
        this.AttachedToVisualTree += OnAttachedToVisualTree;
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

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (_calcPageVM?.Items is INotifyCollectionChanged coll)
        {
            coll.CollectionChanged += async (s, ev) => await PlayUpdateAnimation();
        }
    }

    private async Task PlayUpdateAnimation()
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            try
            {
                if (ResultsGridContainer == null) return;

                // «атухание и смещение
                ResultsGridContainer.Opacity = 0.2;
                ResultsGridContainer.Margin = new Thickness(12, 0, 0, 0);
                await Task.Delay(150); // столько, сколько длитс€ DoubleTransition

                // ѕо€вление и возврат
                ResultsGridContainer.Opacity = 1.0;
                ResultsGridContainer.Margin = new Thickness(0);
                await Task.Delay(150); // столько, сколько длитс€ ThicknessTransition
            }
            catch { }
        });
    }
}