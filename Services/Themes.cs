using Avalonia.Styling;

namespace ExprodesC.Services;

public class Themes
{
    public int Id { get; set; }
    public string Name { get; set; } = Lang.Resources.theme_sys;
    public ThemeVariant Theme { get; set; } = ThemeVariant.Default;
    public static Themes Default { get; } = new() { Id = 0, Name = Lang.Resources.theme_sys, Theme = ThemeVariant.Default };

    public static List<Themes> Items = [
        new(){ Id = 0, Name = Lang.Resources.theme_sys, Theme = ThemeVariant.Default},
        new(){ Id = 1, Name = Lang.Resources.theme_light, Theme = ThemeVariant.Light},
        new(){ Id = 2, Name = Lang.Resources.theme_dark, Theme = ThemeVariant.Dark},
        ];
}
