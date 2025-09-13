using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ExprodesC.ViewModels;
using ExprodesC.Views.Synonym;
using ExprodesC.Wrappers;

namespace ExprodesC;

public partial class SynonymView : UserControl
{
    SynonymVM _synonymVM;

    public SynonymView()
    {
        InitializeComponent();
        this.DataContext = _synonymVM = App.Services.GetRequiredService<SynonymVM>();
        dgLocuses.DoubleTapped += DgLocuses_DoubleTapped;
    }

    private void DgLocuses_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        /*if (sender is DataGrid lb)
        {
            switch (lb.Name)
            {
                case "lbPopulations": _libraryPageVM.EditPopulationCommand?.ExecuteIfPossible(_libraryPageVM.SelectedPopulation); break;
                case "lbLocuses": _libraryPageVM.EditLocusCommand?.ExecuteIfPossible(_libraryPageVM.SelectedLocus); break;
                case "lbAlleles": _libraryPageVM.EditAlleleCommand?.ExecuteIfPossible(_libraryPageVM.SelectedAllele); break;
                default:
                    break;
            }
        }*/
    }
}