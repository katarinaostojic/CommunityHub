using System.Windows.Media;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

public class TrustStatItem
{
    public string Label { get; }
    public int Count { get; }
    public SolidColorBrush Color { get; }

    public TrustStatItem(string label, int count, SolidColorBrush color)
    {
        Label = label;
        Count = count;
        Color = color;
    }
}