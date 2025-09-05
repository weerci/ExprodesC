using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ExprodesC.ViewModels;
using Func.Extensions;

namespace ExprodesC;

public partial class LibraryPage : UserControl
{
    LibraryPageVM _libraryPageVM;

    public LibraryPage()
    {
        InitializeComponent();
        this.DataContext = _libraryPageVM = App.Services.GetRequiredService<LibraryPageVM>();
        lbPopulations.DoubleTapped += Lb_DoubleTapped;
        lbLocuses.DoubleTapped += Lb_DoubleTapped;
        lbAlleles.DoubleTapped += Lb_DoubleTapped;
    }

    private void Lb_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (sender is ListBox lb)
        {
            switch (lb.Name)
            {
                case "lbPopulations": _libraryPageVM.EditPopulationCommand?.ExecuteIfPossible(_libraryPageVM.SelectedPopulation); break;
                case "lbLocuses": _libraryPageVM.EditLocusCommand?.ExecuteIfPossible(_libraryPageVM.SelectedLocus); break;
                case "lbAlleles": _libraryPageVM.EditAlleleCommand?.ExecuteIfPossible(_libraryPageVM.SelectedAllele); break;
                default:
                    break;
            }
        }
    }
}