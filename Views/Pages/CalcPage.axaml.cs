using Avalonia.Controls;
using ExprodesC.ViewModels;

namespace ExprodesC.Views.Pages;

public partial class CalcPage : UserControl
{

    MainPageVM _mainPageVM = App.Services.GetRequiredService<MainPageVM>();

    public CalcPage()
    {
        InitializeComponent();
        this.DataContext = App.Services.GetRequiredService<CalcPageVM>();

    }

}