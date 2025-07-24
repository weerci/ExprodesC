using ExprodesC.Services;
using System.Linq;

namespace ExprodesC.Imp;

public class AppHost : IAppHost
{
    public Themes CurrentTheme
    {
        get
        {
            if (App.Current != null)
                return Themes.Items.FirstOrDefault(i => i.Theme == App.Current.ActualThemeVariant, Themes.Default);

            return Themes.Default;
        }

        set
        {
            if (App.Current != null)
                App.Current.RequestedThemeVariant = value.Theme;
        }
    }
}
