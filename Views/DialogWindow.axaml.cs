using Avalonia.Controls;
using ExprodesC.ViewModels;
using System.Runtime.CompilerServices;

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
        if (userControl.DataContext is BaseVM bm)
            bm.Owner = this;
    }

    private bool _isVisbleButton = true;
    public bool IsVisibleButton
    {
        get { return _isVisbleButton; }
        set
        {
            btnClose.IsVisible = value;
            btnSave.IsVisible = value;
        }
    }


    private void ButtonSave_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => this.Close(DialogResult.Save);
    private void ButtonClose_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => this.Close(DialogResult.Close);
}

public enum DialogResult
{
    Close,
    Save
}