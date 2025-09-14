using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ExprodesC.ViewModels;
using System.Diagnostics;
using System.Reactive.Linq;
using FluentAvalonia.UI.Controls;
using ExprodesC.Wrappers;

namespace ExprodesC;

public partial class AddEditLocus : UserControl
{
    BaseVM _baseVM;
    public AddEditLocus()
    {
        InitializeComponent();
        this.DataContext = _baseVM = new BaseVM();
        
        tbName.TextChanged += TbTextChanged;
        nbMinFreq.ValueChanged += NbValueChanged;
        nbMutFreq.ValueChanged += NbValueChanged;
    }

    public AddEditLocus(LocusWR lwr) : this()
    {
        if (lwr != null)
        {
            tbName.Text = lwr.Name;
            nbMinFreq.Value = lwr.MinFreq;
            nbMutFreq.Value = lwr.MutFreq;
            chbIsCalculate.IsChecked = lwr.IsCalc;
        }
    }

    private void NbValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        _baseVM.CanSave = canSave();
    }

    private void TbTextChanged(object? sender, TextChangedEventArgs e)
    {
        _baseVM.CanSave = canSave();
    }

    bool canSave()
    {
        return tbName.Text?.Length > 0 && !String.IsNullOrEmpty(tbName.Text)&&
               nbMinFreq.Value > 0 && nbMinFreq.Value <= 1 &&
               nbMutFreq.Value > 0 && nbMutFreq.Value <= 1;
    }

    private void UserControl_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        tbName.Focus();
    }
}