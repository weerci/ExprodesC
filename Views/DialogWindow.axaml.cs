using Avalonia.Controls;
using ExprodesC.ViewModels;

namespace ExprodesC;

public partial class DialogWindow : Window
{
    public DialogWindow()
    {
        InitializeComponent();
    }
    public DialogWindow(string title, UserControl userControl) : this()
    {
        tbTitle.Text = title;
        ccPlace.Content = userControl;
        DataContext = userControl.DataContext;
    }

    private void ButtonSave_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => this.Close(DialogResult.Save);
    private void ButtonClose_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => this.Close(DialogResult.Close);
}

public enum DialogResult
{
    Close,
    Save
}