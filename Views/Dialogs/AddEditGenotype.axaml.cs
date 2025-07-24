using Avalonia.Controls;
using Avalonia.Interactivity;
using Calc.Models;
using ExprodesC.Views.Dialogs;
using ExprodesC.Views.Wrappers;

namespace ExprodesC;

public partial class AddEditGenotype : UserControl
{

    public AddEditGenotype()
    {
        InitializeComponent();
        DataContext = new AddEditGenotypeVM();
    }

    public AddEditGenotype(GenotypeWR genotypeWR)
    {
        InitializeComponent();
        DataContext = new AddEditGenotypeVM(genotypeWR);
    }

    private void UserControl_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        tbName.Focus();
    }
}