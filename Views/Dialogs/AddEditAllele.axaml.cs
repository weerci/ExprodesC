using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ExprodesC.ViewModels;
using ExprodesC.Wrappers;
using FluentAvalonia.UI.Controls;

namespace ExprodesC;

public partial class AddEditAllele : UserControl
{
    BaseVM _baseVM;

    public AddEditAllele()
    {
        InitializeComponent();
        tbName.TextChanged += TbTextChanged;
        nbFreq.ValueChanged += NbValueChanged;

        this.DataContext = _baseVM = new BaseVM();

    }

    public AddEditAllele(AlleleWR awr) : this()
    {
        if (awr != null)
        {
            tbName.Text = awr.Allele.Name;
            nbFreq.Value = awr.Freq;
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

    private bool canSave()
    {
        return tbName.Text?.Length > 0 &&
              nbFreq.Value > 0 && nbFreq.Value < 1;
    }

    private void UserControl_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        tbName.Focus();
    }
}