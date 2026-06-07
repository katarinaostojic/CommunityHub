using CommunityHub.Ui.Views.ManagerViews.Controls;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Helpers.Manager;

public static class KeyboardHelper
{
    public static void AttachToAllTextBoxes(FrameworkElement root, Window owner)
    {
        foreach (var tb in FindAllTextBoxes(root))
        {
            tb.GotFocus += (s, e) =>
            {
                if (s is TextBox textBox)
                {
                    var floatingKb = new FloatingKeyboardWindow(textBox, owner);
                    floatingKb.Show();
                    floatingKb.Closed += (_, _) => textBox.Focus();
                }
            };
        }
    }

    private static IEnumerable<TextBox> FindAllTextBoxes(DependencyObject parent)
    {
        for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
            if (child is TextBox tb)
                yield return tb;
            foreach (var descendant in FindAllTextBoxes(child))
                yield return descendant;
        }
    }
}