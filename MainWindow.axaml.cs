using Avalonia.Controls;

namespace ExprodesC;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public string Greeting => "Welcome to Avalonia! This is my added text.";
}