using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ExprodesC.ViewModels;
using ExprodesC.Views.Synonym;
using ExprodesC.Wrappers;
using System.Linq;

namespace ExprodesC;

public partial class AddEditSynonym : UserControl
{
    SynonymVM _synonymVM = null!;
    public AddEditSynonym()
    {
        InitializeComponent();
        DataContext = _synonymVM = App.Services.GetRequiredService<SynonymVM>();

        if (_synonymVM != null && _synonymVM.LocusGroups != null && _synonymVM.SelectedSynonym != null)
            cbLocuses.SelectedItem = _synonymVM.LocusGroups.FirstOrDefault(lg => lg.LocusName == _synonymVM.SelectedSynonym.LocusName);
    }

    public AddEditSynonym(SynonymWR sWR) : this()
    {
        if (sWR != null)
            tbName.Text = sWR.Name;
    }

    private bool canSave()
    {
        return tbName.Text?.Length > 0 &&
            !String.IsNullOrWhiteSpace(tbName.Text) &&
            cbLocuses.SelectedItem != null;
    }

    private void UserControl_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        cbLocuses.Focus();
    }

    private void TextBox_TextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        _synonymVM.CanSave = canSave();
    }

    private void ComboBox_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        _synonymVM.CanSave = canSave();
    }
}