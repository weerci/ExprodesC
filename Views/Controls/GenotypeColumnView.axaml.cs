using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using ExprodesC.ViewModels;
using ExprodesC.Views;
using ExprodesC.Views.Pages;
using System.Diagnostics;
using System.Linq;

namespace ExprodesC.Views.Controls;

public partial class GenotypeColumnView : UserControl
{
    public GenotypeColumnView()
    {
        InitializeComponent();
    }
}