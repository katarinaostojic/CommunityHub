using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CommunityHub.Ui.Helpers.Manager;

public static class TooltipsManager
{
    public static void Apply(DependencyObject? root)
    {
        if (root == null)
            return;

        ApplyRecursive(root, AppSession.IsTooltipsEnabled);
    }

    private static void ApplyRecursive(DependencyObject parent, bool enabled)
    {
        if (parent is FrameworkElement element && ToolTipService.GetToolTip(element) != null)
        {
            ToolTipService.SetIsEnabled(element, enabled);
        }

        int childCount = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childCount; i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(parent, i);
            ApplyRecursive(child, enabled);
        }
    }
}