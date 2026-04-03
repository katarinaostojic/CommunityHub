using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CommunityHub.Ui.Helpers;

public static class Banner
{
    public static void ShowSuccess(Border banner, TextBlock text, string message)
    {
        text.Text = message;
        banner.Visibility = Visibility.Visible;

        DispatcherTimer timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(3);
        timer.Tick += (s, args) =>
        {
            banner.Visibility = Visibility.Collapsed;
            timer.Stop();
        };
        timer.Start();
    }
}