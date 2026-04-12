using CommunityHub.Application.Domain;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace CommunityHub.Ui.Converters;

public class ImagePathConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is List<Image> images && images.Count > 0)
        {
            string fullPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                images[0].Path.Replace('/', Path.DirectorySeparatorChar)
            );
            try { return new BitmapImage(new Uri(fullPath, UriKind.Absolute)); }
            catch { return null; }
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}