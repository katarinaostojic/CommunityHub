using System.Diagnostics;

namespace CommunityHub.Ui.Helpers;

public static class PdfViewer
{
    public static void Open(string filePath)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = filePath,
            UseShellExecute = true
        });
    }
}