using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ExprodesC.Assets.Styles;

public class PreviewVM : ReactiveObject
{
    public static PreviewVM DesignInstance { get; } = new PreviewVM();

    public ObservableCollection<PreviewItems> Items { get; } = new ObservableCollection<PreviewItems>
    {
            new PreviewItems { Name = "Calculation 1", DirectResult = 12.34, ReverseResult = 56.78, Formulas = "Очень очень длинный текст" },
            new PreviewItems { Name = "Calculation 2", DirectResult = 23.45, ReverseResult = 67.89, Formulas = " new PreviewItems { Name = \"Calculation 1\", DirectResult = 12.34, ReverseResult = 56.78, Formulas" },
            new PreviewItems { Name = "Calculation 3", DirectResult = 34.56, ReverseResult = 78.90, Formulas = "PointerPressed=\"NavigationViewItem_PointerPressed" }
    };


}

public class PreviewItems
{
    public PreviewItems()
    {
        ToggleExpandCommand = ReactiveCommand.Create(() => IsExpanded = !IsExpanded);
    }
    public string Name { get; set; }
    public double DirectResult { get; set; }
    public double ReverseResult { get; set; }
    public string Formulas { get; set; }

    [Reactive] public bool IsExpanded { get; set; }
    public ICommand ToggleExpandCommand { get; }

}
