using CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.Ads.NoticeBoard;
using CommunityHub.Ui.Views.TenantViews;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Helpers.Tenant.NoticeBoard;

public class NoticeBoardReportController
{
    private readonly NoticeBoardViewModel _viewModel;
    private readonly Page _page;
    private readonly Border _exportSuccessBanner;
    private readonly TextBlock _exportSuccessTextBlock;

    public NoticeBoardReportController(
        NoticeBoardViewModel viewModel,
        Page page,
        Border exportSuccessBanner,
        TextBlock exportSuccessTextBlock)
    {
        _viewModel = viewModel;
        _page = page;
        _exportSuccessBanner = exportSuccessBanner;
        _exportSuccessTextBlock = exportSuccessTextBlock;
    }

    public void ExportPdf()
    {
        ExportReportDialog dialog = new ExportReportDialog
        {
            Owner = Window.GetWindow(_page)
        };

        if (dialog.ShowDialog() != true)
            return;

        string? filePath = SelectPdfFilePath(dialog.DateFrom, dialog.DateTo);

        if (filePath == null)
            return;

        _viewModel.ExportReport(filePath, dialog.DateFrom, dialog.DateTo);

        NotificationBanner.ShowSuccess(
            _exportSuccessBanner,
            _exportSuccessTextBlock,
            "✔ PDF exported successfully.");

        PdfViewer.Open(filePath);
    }

    public void DismissExportSuccess()
    {
        _exportSuccessBanner.Visibility = Visibility.Collapsed;
    }

    private static string? SelectPdfFilePath(DateOnly dateFrom, DateOnly dateTo)
    {
        SaveFileDialog dialog = new SaveFileDialog
        {
            Filter = "PDF files (*.pdf)|*.pdf",
            FileName = $"tenant-ads-report-{dateFrom:yyyy-MM-dd}-{dateTo:yyyy-MM-dd}.pdf"
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}