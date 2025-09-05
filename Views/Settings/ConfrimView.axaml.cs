using Avalonia.Controls;
using ExprodesC.Imp;
using Func.Services;
using System.Runtime;

namespace ExprodesC;

public partial class ConfrimView : UserControl
{
    ISettingsProvider<ExSettingData> _settings = null!;

    public ConfrimView()
    {
        InitializeComponent();

    }

    public ConfrimView(ISettingsProvider<ExSettingData> settings) : this()
    {
        _settings = settings;
        this.DataContext = _settings.Value;

        var v = _settings.Value.WhenAnyValue(
            it => it.ConfirmDeleteGenotype,
            it => it.ConfirmDeletePopulation,
            it => it.ConfirmDeleteLocus,
            it => it.ConfirmDeleteAllele)
            .Subscribe(_ => _settings.Save(_settings.Value.GetSettingFile));
    }
}