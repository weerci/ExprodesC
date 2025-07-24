using Avalonia.Controls;
using ExprodesC.ViewModels;

namespace ExprodesC.Views.Pages;

public partial class CalcPage : UserControl
{
    public CalcPage()
    {
        InitializeComponent();
        this.DataContext = App.Services.GetRequiredService<CalcPageVM>();

    }

}