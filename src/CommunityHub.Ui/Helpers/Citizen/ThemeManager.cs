using System;
using System.Linq;

namespace CommunityHub.Ui.Helpers.Citizen;

public static class ThemeManager
{
    private static bool _isDark = false;

    public static void ToggleTheme()
    {
        var dictionaries = System.Windows.Application.Current.Resources.MergedDictionaries;

        var toRemove = dictionaries.FirstOrDefault(d =>
            d.Source != null && (
            d.Source.ToString().Contains("CitizenTheme") ||
            d.Source.ToString().Contains("CitizenDarkTheme")));

        if (toRemove != null)
            dictionaries.Remove(toRemove);

        _isDark = !_isDark;
        string themePath = _isDark
            ? "Themes/CitizenDarkTheme.xaml"
            : "Themes/CitizenTheme.xaml";

        dictionaries.Add(new System.Windows.ResourceDictionary
        {
            Source = new Uri(themePath, UriKind.Relative)
        });
    }

    public static bool IsDark => _isDark;
}