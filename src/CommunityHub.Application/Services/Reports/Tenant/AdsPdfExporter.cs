using CommunityHub.Application.DTOs.Reports;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

using QuestPdfDocument = QuestPDF.Fluent.Document;

namespace CommunityHub.Application.Services.Reports;

public class AdsPdfExporter
{
    public void Export(string filePath, AdsReportDto report)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        QuestPdfDocument.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);
                page.PageColor(AdsPdfReportStyles.BackgroundLight);
                page.DefaultTextStyle(text => text
                    .FontSize(10)
                    .FontColor(AdsPdfReportStyles.TextPrimary));

                page.Header().Column(column =>
                {
                    AdsPdfReportHeader.AddAppBanner(column, report);
                    AdsPdfReportHeader.AddReportHeader(column, report);
                });

                page.Content().PaddingTop(18).Column(column =>
                {
                    AdsPdfReportSummary.AddSummary(column, report);
                    AdsPdfReportTable.AddAdsTable(column, report.Ads);
                });

                page.Footer()
                    .PaddingTop(12)
                    .Row(row =>
                    {
                        AddGeneratedInfo(row);
                        AddPageNumbers(row);
                    });
            });
        }).GeneratePdf(filePath);
    }

    private static void AddGeneratedInfo(RowDescriptor row)
    {
        row.RelativeItem()
            .AlignLeft()
            .Text(text =>
            {
                text.DefaultTextStyle(style => style
                    .FontSize(9)
                    .FontColor(AdsPdfReportStyles.TextSecondary));

                text.Span("Generated on ");
                text.Span(DateTime.Now.ToString("dd.MM.yyyy. HH:mm")).SemiBold();
            });
    }

    private static void AddPageNumbers(RowDescriptor row)
    {
        row.RelativeItem()
            .AlignRight()
            .Text(text =>
            {
                text.DefaultTextStyle(style => style
                    .FontSize(9)
                    .FontColor(AdsPdfReportStyles.TextSecondary));

                text.Span("Page ");
                text.CurrentPageNumber();
                text.Span(" of ");
                text.TotalPages();
            });
    }
}