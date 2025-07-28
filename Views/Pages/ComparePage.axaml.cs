using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ExprodesC.ViewModels;
using ExprodesC.ViewModels.Settings;

namespace ExprodesC;

public partial class ComparePage : UserControl
{
    public ComparePage()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<ComparePageVM>();

    }
}