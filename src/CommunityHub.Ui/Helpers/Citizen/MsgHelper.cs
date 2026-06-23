using System.Windows;

namespace CommunityHub.Ui.Helpers.Citizen;

public static class MsgHelper
{
    public static string Get(string key, string fallback = "")
    {
        var res = System.Windows.Application.Current.TryFindResource(key);
        return res as string ?? fallback;
    }

    public static void Info(string msgKey, string titleKey, string msgFallback = "", string titleFallback = "")
        => MessageBox.Show(Get(msgKey, msgFallback), Get(titleKey, titleFallback),
                           MessageBoxButton.OK, MessageBoxImage.Information);

    public static void Warn(string msgKey, string titleKey, string msgFallback = "", string titleFallback = "")
        => MessageBox.Show(Get(msgKey, msgFallback), Get(titleKey, titleFallback),
                           MessageBoxButton.OK, MessageBoxImage.Warning);

    public static void Error(string msgKey, string titleKey, string msgFallback = "", string titleFallback = "")
        => MessageBox.Show(Get(msgKey, msgFallback), Get(titleKey, titleFallback),
                           MessageBoxButton.OK, MessageBoxImage.Error);

    public static MessageBoxResult Confirm(string msgKey, string titleKey, string msgFallback = "", string titleFallback = "")
        => MessageBox.Show(Get(msgKey, msgFallback), Get(titleKey, titleFallback),
                           MessageBoxButton.YesNo, MessageBoxImage.Question);
}
