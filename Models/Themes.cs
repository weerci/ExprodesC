using Avalonia.Styling;

namespace ExprodesC.Models;

public record Themes
{
    public int Id { get; set; }
    public string Name { get; set; } = Lang.Resources.theme_sys;
    public ThemeVariant Theme()
    {
        if (Name == Lang.Resources.theme_light)
            return ThemeVariant.Light;
        else if (Name == Lang.Resources.theme_dark)
            return ThemeVariant.Dark;
        else
            return ThemeVariant.Default;
    }

}
