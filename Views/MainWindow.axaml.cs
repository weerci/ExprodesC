using FluentAvalonia.UI.Windowing;


namespace ExprodesC.Views;

public partial class MainWindow : AppWindow
{
    public MainWindow()
    {
        InitializeComponent();
        MinWidth = 800;
        MinHeight = 600;
        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
    }
}