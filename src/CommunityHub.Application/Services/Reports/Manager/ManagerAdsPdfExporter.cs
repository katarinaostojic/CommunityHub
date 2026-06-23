using CommunityHub.Application.Domain.Entities.Buildings.Ads;
using CommunityHub.Application.DTOs.Reports;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

using QuestPdfDocument = QuestPDF.Fluent.Document;

namespace CommunityHub.Application.Services.Reports;

public class ManagerAdsPdfExporter
{
    public void Export(string filePath, ManagerAdsReportDto report)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        QuestPdfDocument
            .Create(container => AddReportPage(container, report))
            .GeneratePdf(filePath);
    }

    private static void AddReportPage(IDocumentContainer container, ManagerAdsReportDto report)
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
        page.Size(PageSizes.A4);
        page.Margin(30);
        page.PageColor(ManagerAdsPdfReportStyles.BackgroundLight);
        page.DefaultTextStyle(text => text
            .FontSize(10)
            .FontColor(ManagerAdsPdfReportStyles.TextPrimary));
    }

    private static void AddHeader(PageDescriptor page, ManagerAdsReportDto report)
    {
        page.Header().Column(column =>
        {
            ManagerAdsPdfReportHeader.AddAppBanner(column, report);
            ManagerAdsPdfReportHeader.AddReportHeader(column, report);
        });
    }

    private static void AddContent(PageDescriptor page, ManagerAdsReportDto report)
    {
        page.Content().PaddingTop(18).Column(column =>
        {
            AddSummary(column, report);
            ManagerAdsPdfReportTable.AddAdsTable(column, report.Ads);
        });
    }

    private static void AddSummary(ColumnDescriptor column, ManagerAdsReportDto report)
    {
        bool isOffering = report.Type == AdType.Offering;
        string label = isOffering ? "Offering" : "Seeking";
        string valueColor = isOffering ? ManagerAdsPdfReportStyles.Offering : ManagerAdsPdfReportStyles.Seeking;
        string background = isOffering ? ManagerAdsPdfReportStyles.OfferingSoft : ManagerAdsPdfReportStyles.SeekingSoft;

        column.Item().PaddingBottom(16)
            .Background(background)
            .Border(1)
            .BorderColor(ManagerAdsPdfReportStyles.Border)
            .Padding(10)
            .Column(summary =>
            {
                summary.Item().Text($"Currently active ads ({label})")
                    .FontSize(9)
                    .SemiBold()
                    .FontColor(ManagerAdsPdfReportStyles.TextSecondary);

                summary.Item().PaddingTop(4)
                    .Text(report.TotalAds.ToString())
                    .FontSize(18)
                    .Bold()
                    .FontColor(valueColor);
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
                    .FontColor(ManagerAdsPdfReportStyles.TextSecondary));

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
                    .FontColor(ManagerAdsPdfReportStyles.TextSecondary));

                text.Span("Page ");
                text.CurrentPageNumber();
                text.Span(" of ");
                text.TotalPages();
            });
    }
}