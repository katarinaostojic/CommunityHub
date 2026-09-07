using System.Windows;
using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Buildings.Ads;
using CommunityHub.Application.Services.Reports;
using CommunityHub.Ui.Helpers;
using CommunityHub.Ui.Views.ManagerViews.Dialogs;
using Microsoft.Win32;

namespace CommunityHub.Ui.Helpers.Manager.NoticeBoard;

public class ManagerNoticeBoardReportController
{
    private readonly ManagerAdsReportService _reportService;
    private readonly ManagerAdsPdfExporter _pdfExporter;

    public ManagerNoticeBoardReportController()
    {
        _reportService = Injector.CreateInstance<ManagerAdsReportService>();
        _pdfExporter = Injector.CreateInstance<ManagerAdsPdfExporter>();
    }

    public void ExportReport(
        Window owner,
        long buildingId,
        string buildingSubtitle,
        string managerName,
        Action<string> onSuccess)
    {
        ExportAdsReportDialog dialog = new ExportAdsReportDialog { Owner = owner };

        if (dialog.ShowDialog() != true)
            return;

        string? filePath = AskForSavePath(dialog.SelectedType);
        if (filePath == null)
            return;

        var report = _reportService.Create(
            buildingId,
            buildingSubtitle,
            managerName,
            dialog.SelectedType);

        _pdfExporter.Export(filePath, report);

        PdfViewer.Open(filePath);
        onSuccess($"Report exported successfully to {filePath}");
    }

    private static string? AskForSavePath(AdType type)
    {
        string suggestedName = type == AdType.Offering
            ? "NoticeBoard_Offering_Report.pdf"
            : "NoticeBoard_Seeking_Report.pdf";

        SaveFileDialog saveDialog = new SaveFileDialog
        {
            Filter = "PDF files (*.pdf)|*.pdf",
            FileName = suggestedName
        };

        return saveDialog.ShowDialog() == true ? saveDialog.FileName : null;
    }
}