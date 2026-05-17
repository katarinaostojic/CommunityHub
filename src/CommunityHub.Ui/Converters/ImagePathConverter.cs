using CommunityHub.Application.Domain;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using CommunityHub.Application.Domain.Neighborhoods;

namespace CommunityHub.Ui.Converters;

public class ImagePathConverter : IValueConverter
{
    public static BitmapImage? LoadImage(string path)
    {
        string fullPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            path.Replace('/', Path.DirectorySeparatorChar)
        );
        try { return new BitmapImage(new Uri(fullPath, UriKind.Absolute)); }
        catch { return null; }
    }

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is List<string> paths && paths.Count > 0)
            return LoadImage(paths[0]);
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();


}

public class SingleImagePathConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string path)
            return ImagePathConverter.LoadImage(path);
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
public class NeighborhoodImageConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is List<Image> images && images.Count > 0)
            return ImagePathConverter.LoadImage(images[0].Path);
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}