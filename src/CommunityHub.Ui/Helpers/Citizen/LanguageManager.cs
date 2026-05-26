using System;
using System.Linq;

namespace CommunityHub.Ui.Helpers.Citizen;

public static class LanguageManager
{
    private static bool _isSerbianActive = true;

    public static void ToggleLanguage()
    {
        var dictionaries = System.Windows.Application.Current.Resources.MergedDictionaries;

        var toRemove = dictionaries.FirstOrDefault(d =>
            d.Source != null && (
            d.Source.ToString().Contains("sr.xaml") ||
            d.Source.ToString().Contains("en.xaml")));

        if (toRemove != null)
            dictionaries.Remove(toRemove);

        _isSerbianActive = !_isSerbianActive;
        string langPath = _isSerbianActive
         ? "Helpers/Citizen/Languages/Sr.xaml"
         : "Helpers/Citizen/Languages/En.xaml";

        dictionaries.Add(new System.Windows.ResourceDictionary
        {
            Source = new Uri(langPath, UriKind.Relative)
        });
    }

    public static bool IsSerbianActive => _isSerbianActive;
}