using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Calc.Imp;
using ExprodesC.Imp;
using ExprodesC.Models;
using ExprodesC.Services;
using Func.Services;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime;

namespace ExprodesC;

public partial class CalculationView : UserControl
{
    ISettingsProvider<ExSettingData> _settings = null!;

    public CalculationView()
    {
        InitializeComponent();
    }
    public CalculationView(ISettingsProvider<ExSettingData> settings) : this()
    {
        _settings = settings;
        this.DataContext = _settings.Value;

        var v = _settings.Value.WhenAnyValue(
            it => it.Pmin, 
            it => it.DecPlaceCount,
            it => it.CurrentPopulation,
            it => it.CalcAverageMethod)
            .Subscribe(_ => _settings.Save(_settings.Value.GetSettingFile));
    }

}