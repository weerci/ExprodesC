using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ExprodesC.ViewModels;
using ExprodesC.Views.Synonym;

namespace ExprodesC;

public partial class SynonymView : UserControl
{
    SynonymVM _synonymVM;

    public SynonymView()
    {
        InitializeComponent();
        this.DataContext = _synonymVM = App.Services.GetRequiredService<SynonymVM>();
        tvSynonyms.SelectionChanged += TvSynonyms_SelectionChanged;
    }

    private void TvSynonyms_SelectionChanged(object sender, SelectionChangedEventArgs args)
    {
        _synonymVM.SelectedTab = tvSynonyms.SelectedIndex;
    }
}