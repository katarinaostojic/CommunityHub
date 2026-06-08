using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public class ImagePreviewController
{
    private readonly System.Windows.Controls.Image _previewImage;
    private readonly Button _removeButton;
    private readonly Button _prevButton;
    private readonly Button _nextButton;
    private int _currentIndex = 0;

    public List<string> ImagePaths { get; } = new();

    public ImagePreviewController(
        System.Windows.Controls.Image previewImage,
        Button removeButton,
        Button prevButton,
        Button nextButton)
    {
        _previewImage = previewImage;
        _removeButton = removeButton;
        _prevButton = prevButton;
        _nextButton = nextButton;
    }

    public void AddImage(string path)
    {
        if (!ImagePaths.Contains(path))
            ImagePaths.Add(path);
        Refresh();
    }

    public void RemoveCurrentImage()
    {
        if (ImagePaths.Count == 0) return;
        ImagePaths.RemoveAt(_currentIndex);
        Refresh();
    }

    public void PrevImage()
    {
        if (ImagePaths.Count == 0) return;
        _currentIndex = (_currentIndex - 1 + ImagePaths.Count) % ImagePaths.Count;
        Refresh();
    }

    public void NextImage()
    {
        if (ImagePaths.Count == 0) return;
        _currentIndex = (_currentIndex + 1) % ImagePaths.Count;
        Refresh();
    }

    private void Refresh()
    {
        if (ImagePaths.Count == 0)
        {
            _previewImage.Source = null;
            _removeButton.Visibility = Visibility.Collapsed;
            _prevButton.Visibility = Visibility.Collapsed;
            _nextButton.Visibility = Visibility.Collapsed;
            return;
        }

        if (_currentIndex >= ImagePaths.Count)
            _currentIndex = ImagePaths.Count - 1;

        _previewImage.Source = new BitmapImage(new Uri(ImagePaths[_currentIndex]));
        _removeButton.Visibility = Visibility.Visible;
        _prevButton.Visibility = ImagePaths.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
        _nextButton.Visibility = ImagePaths.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
    }
}