using System;
using System.Linq;
using System.Windows;

namespace CommunityHub.Ui.Helpers.Citizen;

public static class LanguageManager
{
    public static event Action? LanguageChanged;

    private static bool _isSerbianActive = true;

    public static void ToggleLanguage()
    {
        _isSerbianActive = !_isSerbianActive;
        ApplyLanguage();
    }

    public static void ApplyLanguage()
    {
        string langPath = _isSerbianActive
            ? "Helpers/Citizen/Languages/Sr.xaml"
            : "Helpers/Citizen/Languages/En.xaml";

        var newDict = new ResourceDictionary
        {
            Source = new Uri(langPath, UriKind.Relative)
        };

        ReplaceDict(System.Windows.Application.Current.Resources.MergedDictionaries, newDict);

        LanguageChanged?.Invoke();
    }

    private static void ReplaceDict(
        System.Collections.ObjectModel.Collection<ResourceDictionary> dicts,
        ResourceDictionary newDict)
    {
        var toRemove = dicts.FirstOrDefault(d =>
            d.Source != null && (
            d.Source.ToString().Contains("Sr.xaml", StringComparison.OrdinalIgnoreCase) ||
            d.Source.ToString().Contains("En.xaml", StringComparison.OrdinalIgnoreCase)));

        if (toRemove != null)
            dicts.Remove(toRemove);

        dicts.Add(new ResourceDictionary { Source = newDict.Source });
    }

    public static bool IsSerbianActive => _isSerbianActive;
}
