using Avalonia.Styling;
using Calc.Imp;

namespace ExprodesC.Imp;

public class ExSettingData : AppSettingsData
{
    /// <summary>
    /// Возвращает значение текущей популяции
    /// </summary>
    [Reactive] public bool ConfirmDeleteGenotype { get; set; } = true;
    [Reactive] public ThemeVariant CurrentTheme { get; set; } = ThemeVariant.Default;

}
