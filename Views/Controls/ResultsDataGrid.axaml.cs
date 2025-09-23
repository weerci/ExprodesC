using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using ExprodesC.Extensions;
using ExprodesC.ViewModels;
using System.IO.Compression;

namespace ExprodesC.Views.Controls;

public partial class ResultsDataGrid : UserControl
{
    CalcPageVM _calcPageVM = null!;
    public ResultsDataGrid()
    {
        InitializeComponent();
        if (!Design.IsDesignMode)
            DataContext = _calcPageVM = App.Services.GetRequiredService<CalcPageVM>();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (Design.IsDesignMode)
            return;


        if (sender is Button button)
        {
            // Находим DataGridRow
            var dataGridRow = button.FindAncestorOfType<DataGridRow>();
            if (dataGridRow != null)
            {
                // Находим Border в RowDetails
                var border = dataGridRow.ExFindDescendantOfType<Border>(t=> t.Name == "brdDetails");
                if (border != null)
                {
                    border.Height = _calcPageVM.ShowReverse ? _calcPageVM.Settings.Value.MaxDetailsHeight : _calcPageVM.Settings.Value.MinDetailsHeight;
                }
            }
        }
    }
}