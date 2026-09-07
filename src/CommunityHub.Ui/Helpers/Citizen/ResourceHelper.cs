using System.Windows;

namespace CommunityHub.Ui.Helpers.Citizen;

public static class ResourceHelper
{
    public static string Get(string key, string fallback = "")
    {
        foreach (var dict in System.Windows.Application.Current.Resources.MergedDictionaries)
        {
            if (dict.Contains(key))
                return dict[key]?.ToString() ?? fallback;
        }
        return fallback;
    }
}