using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CommunityHub.Ui.Helpers;

public class FilterPanelAnimationHelper
{
    private readonly UIElement _overlay;
    private readonly TranslateTransform _panelTransform;
    private bool _isOpen;

    public FilterPanelAnimationHelper(UIElement overlay, TranslateTransform panelTransform)
    {
        _overlay = overlay;
        _panelTransform = panelTransform;
    }

    public bool IsOpen => _isOpen;

    public void Toggle()
    {
        if (_isOpen)
            Close();
        else
            Open();
    }

    public void Open()
    {
        if (_isOpen)
            return;

        _overlay.Visibility = Visibility.Visible;
        Animate(-300, 0, EasingMode.EaseOut);
        _isOpen = true;
    }

    public void Close()
    {
        if (!_isOpen)
            return;

        DoubleAnimation animation = CreateAnimation(0, -300, EasingMode.EaseIn);
        animation.Completed += (_, _) => _overlay.Visibility = Visibility.Collapsed;

        _panelTransform.BeginAnimation(TranslateTransform.XProperty, animation);
        _isOpen = false;
    }

    private void Animate(double from, double to, EasingMode easingMode)
    {
        _panelTransform.BeginAnimation(
            TranslateTransform.XProperty,
            CreateAnimation(from, to, easingMode));
    }

    private static DoubleAnimation CreateAnimation(double from, double to, EasingMode easingMode)
    {
        return new DoubleAnimation
        {
            From = from,
            To = to,
            Duration = TimeSpan.FromMilliseconds(250),
            EasingFunction = new CubicEase { EasingMode = easingMode }
        };
    }
}