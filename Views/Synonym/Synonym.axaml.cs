using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ExprodesC.ViewModels;
using ExprodesC.Views.Synonym;
using ExprodesC.Wrappers;
using Func.Extensions;

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
        if (sender is DataGrid lb)
            _synonymVM.EditSynonymCommand?.ExecuteIfPossible(_synonymVM.SelectedSynonym);
    }
}