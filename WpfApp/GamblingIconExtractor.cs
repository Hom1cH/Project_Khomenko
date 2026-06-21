using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace WpfApp;

public static class GamblingIconExtractor
{
    public static BitmapSource? Extract(string exePath)
    {
        if (!File.Exists(exePath))
            return null;

        try
        {
            using Icon? icon = Icon.ExtractAssociatedIcon(exePath);
            if (icon == null) return null;

            BitmapSource bitmap = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            bitmap.Freeze();
            return bitmap;
        }
        catch
        {
            return null;
        }
    }
}