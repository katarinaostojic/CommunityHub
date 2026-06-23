using System;
using System.Linq;
using System.Windows;

namespace CommunityHub.Ui.Helpers.Citizen;

public static class ThemeManager
{
    private static bool _isDark = false;

    public static void ToggleTheme()
    {
        _isDark = !_isDark;
        ApplyTheme();
    }

    public static void ApplyTheme()
    {
        string themePath = _isDark
            ? "Themes/CitizenDarkTheme.xaml"
            : "Themes/CitizenTheme.xaml";

        var newDict = new ResourceDictionary
        {
            Source = new Uri(themePath, UriKind.Relative)
        };

        ReplaceDict(System.Windows.Application.Current.Resources.MergedDictionaries, newDict);
    }

    private static void ReplaceDict(
        System.Collections.ObjectModel.Collection<ResourceDictionary> dicts,
        ResourceDictionary newDict)
    {
        var toRemove = dicts.FirstOrDefault(d =>
            d.Source != null && (
            d.Source.ToString().Contains("CitizenTheme", StringComparison.OrdinalIgnoreCase) ||
            d.Source.ToString().Contains("CitizenDarkTheme", StringComparison.OrdinalIgnoreCase)));

        if (toRemove != null)
            dicts.Remove(toRemove);

        dicts.Add(new ResourceDictionary { Source = newDict.Source });
    }

    public static bool IsDark => _isDark;
}
