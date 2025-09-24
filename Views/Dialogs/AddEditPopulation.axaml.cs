using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Calc.Models;
using ExprodesC.Imp;
using ExprodesC.ViewModels;
using Func.Services;

namespace ExprodesC;

public partial class AddEditPopulation : UserControl
{
    LibraryPageVM _baseVM;
    Population? _population;
    public AddEditPopulation()
    {
        InitializeComponent();
        DataContext = _baseVM = App.Services.GetRequiredService<LibraryPageVM>();
        tbName.TextChanging += TbName_TextChanging;
        cbBase.SelectionChanged += CbBase_SelectionChanged;
    }
    public AddEditPopulation(Population population) : this()
    {
        if (population != null)
        {
            tbName.Text = population.Name;
            cbBase.IsEnabled = false;
            _population = population;
        }
    }

    //TODO AddEditPopulation - создание популяции без основании ее на базовой популяции (combobox с пустым значением)
    private void CbBase_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _baseVM.CanSave = canSave();
    }

    private void TbName_TextChanging(object? sender, TextChangingEventArgs e)
    {
        _baseVM.CanSave = canSave();
    }



    bool canSave()
    {
        return tbName.Text?.Length > 0 && !String.IsNullOrWhiteSpace(tbName.Text);
    }

    private void UserControl_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        tbName.Focus();
        
        if (_population != null)
        {
            cbBase.IsVisible = false;
            tbBase.IsVisible = true;
            sePopulation.Header = Lang.Resources.cap_based_population;
            tbBase.Text = _population.BaseOn;
        }

    }
}