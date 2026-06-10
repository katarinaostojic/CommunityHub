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

        QuestPdfDocument
            .Create(container => AddReportPage(container, report))
            .GeneratePdf(filePath);
    }

    private static void AddReportPage(IDocumentContainer container, AdsReportDto report)
    {
        container.Page(page =>
        {
            ConfigurePage(page);
            AddHeader(page, report);
            AddContent(page, report);
            AddFooter(page);
        });
    }

    private static void ConfigurePage(PageDescriptor page)
    {
        page.Size(PageSizes.A4.Landscape());
        page.Margin(30);
        page.PageColor(AdsPdfReportStyles.BackgroundLight);
        page.DefaultTextStyle(text => text
            .FontSize(10)
            .FontColor(AdsPdfReportStyles.TextPrimary));
    }

    private static void AddHeader(PageDescriptor page, AdsReportDto report)
    {
        page.Header().Column(column =>
        {
            AdsPdfReportHeader.AddAppBanner(column, report);
            AdsPdfReportHeader.AddReportHeader(column, report);
        });
    }

    private static void AddContent(PageDescriptor page, AdsReportDto report)
    {
        page.Content().PaddingTop(18).Column(column =>
        {
            AdsPdfReportSummary.AddSummary(column, report);
            AdsPdfReportTable.AddAdsTable(column, report.Ads);
        });
    }

    private static void AddFooter(PageDescriptor page)
    {
        page.Footer()
            .PaddingTop(12)
            .Row(row =>
            {
                AddGeneratedInfo(row);
                AddPageNumbers(row);
            });
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