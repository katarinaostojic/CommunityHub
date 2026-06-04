using CommunityHub.Application.Domain.Entities.Ads;
using CommunityHub.Application.DTOs.Ads;
using CommunityHub.Application.DTOs.Reports;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

using QuestPdfDocument = QuestPDF.Fluent.Document;
using QuestPdfContainer = QuestPDF.Infrastructure.IContainer;

namespace CommunityHub.Application.Services.Reports;

public class AdsPdfExporter
{
    private const string BackgroundLight = "#F7F8F6";
    private const string Surface = "#FFFFFF";
    private const string SurfaceSoft = "#F4FBF7";

    private const string Primary = "#314B40";
    private const string PrimaryDark = "#22362E";
    private const string PrimarySoft = "#E4EDE8";

    private const string Active = "#2F6B4F";
    private const string ActiveSoft = "#E6F4EA";

    private const string Archived = "#9A4A61";
    private const string ArchivedSoft = "#FBE8EE";

    private const string TextPrimary = "#1F2F29";
    private const string TextSecondary = "#5F756B";
    private const string Border = "#DDE8E2";

    public void Export(string filePath, AdsReportDto report)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        QuestPdfDocument.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);
                page.PageColor(BackgroundLight);
                page.DefaultTextStyle(text => text.FontSize(10).FontColor(TextPrimary));

                page.Header().Column(column =>
                {
                    AddAppBanner(column, report);
                    AddReportHeader(column, report);
                });

                page.Content().PaddingTop(18).Column(column =>
                {
                    AddSummary(column, report);
                    AddAdsTable(column, report.Ads);
                });

                page.Footer()
                    .PaddingTop(12)
                    .Row(row =>
                    {
                        row.RelativeItem()
                            .AlignLeft()
                            .Text(text =>
                            {
                                text.DefaultTextStyle(style => style.FontSize(9).FontColor(TextSecondary));
                                text.Span("Generated on ");
                                text.Span(DateTime.Now.ToString("dd.MM.yyyy. HH:mm")).SemiBold();
                            });

                        row.RelativeItem()
                            .AlignRight()
                            .Text(text =>
                            {
                                text.DefaultTextStyle(style => style.FontSize(9).FontColor(TextSecondary));
                                text.Span("Page ");
                                text.CurrentPageNumber();
                                text.Span(" of ");
                                text.TotalPages();
                            });
                    });
            });
        }).GeneratePdf(filePath);
    }

    private static void AddAppBanner(ColumnDescriptor column, AdsReportDto report)
    {
        column.Item()
            .Background(Primary)
            .Padding(14)
            .Row(row =>
            {
                row.RelativeItem().Column(left =>
                {
                    left.Item().Text("CommunityHub")
                        .FontSize(18)
                        .Bold()
                        .FontColor(Colors.White);

                    left.Item().PaddingTop(2).Text("Tenant module / Notice board")
                        .FontSize(9)
                        .FontColor(PrimarySoft);
                });

                row.ConstantItem(260).AlignRight().Column(right =>
                {
                    right.Item().AlignRight().Text("Generated for")
                        .FontSize(8)
                        .FontColor(PrimarySoft);

                    right.Item().AlignRight().Text(report.TenantName)
                        .FontSize(13)
                        .Bold()
                        .FontColor(Colors.White);
                });
            });
    }

    private static void AddReportHeader(ColumnDescriptor column, AdsReportDto report)
    {
        column.Item()
            .Background(Surface)
            .Border(1)
            .BorderColor(Border)
            .Padding(16)
            .Column(header =>
            {
                header.Item().Text("Tenant Ads Report")
                    .FontSize(24)
                    .Bold()
                    .FontColor(Primary);

                header.Item().PaddingTop(4).Text($"Building: {report.BuildingSubtitle}")
                    .FontSize(12)
                    .FontColor(TextSecondary);

                header.Item().PaddingTop(2).Text($"Period: {FormatDate(report.DateFrom)} - {FormatDate(report.DateTo)}")
                    .FontSize(12)
                    .FontColor(TextSecondary);
            });
    }

    private static void AddSummary(ColumnDescriptor column, AdsReportDto report)
    {
        column.Item().PaddingBottom(16).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
            });

            AddSummaryCell(table, "Total ads", report.TotalAds, Primary, Surface);
            AddSummaryCell(table, "Offering", report.OfferingAds, Primary, Surface);
            AddSummaryCell(table, "Seeking", report.SeekingAds, Primary, Surface);
            AddSummaryCell(table, "Active", report.ActiveAds, Active, ActiveSoft);
            AddSummaryCell(table, "Archived", report.ArchivedAds, Archived, ArchivedSoft);
        });
    }

    private static void AddSummaryCell(
        TableDescriptor table,
        string label,
        int value,
        string valueColor,
        string background)
    {
        table.Cell()
            .Background(background)
            .Border(1)
            .BorderColor(Border)
            .Padding(10)
            .Column(column =>
            {
                column.Item().Text(label)
                    .FontSize(9)
                    .SemiBold()
                    .FontColor(TextSecondary);

                column.Item().PaddingTop(4).Text(value.ToString())
                    .FontSize(18)
                    .Bold()
                    .FontColor(valueColor);
            });
    }

    private static void AddAdsTable(ColumnDescriptor column, List<AdDto> ads)
    {
        if (ads.Count == 0)
        {
            column.Item()
                .Background(Surface)
                .Border(1)
                .BorderColor(Border)
                .Padding(14)
                .Text("No ads were found for the selected period.")
                .Italic()
                .FontColor(TextSecondary);

            return;
        }

        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(1.5f);
                columns.RelativeColumn(1);
                columns.RelativeColumn(1.3f);
                columns.RelativeColumn(1.5f);
                columns.RelativeColumn(1);
                columns.RelativeColumn(3);
            });

            AddHeader(table);

            foreach (AdDto ad in ads)
            {
                AddRow(table, ad);
            }
        });
    }

    private static void AddHeader(TableDescriptor table)
    {
        table.Header(header =>
        {
            header.Cell().Element(HeaderCell).Text("Tenant");
            header.Cell().Element(HeaderCell).Text("Type");
            header.Cell().Element(HeaderCell).Text("Category");
            header.Cell().Element(HeaderCell).Text("Date range");
            header.Cell().Element(HeaderCell).Text("Status");
            header.Cell().Element(HeaderCell).Text("Description");
        });
    }

    private static void AddRow(TableDescriptor table, AdDto ad)
    {
        table.Cell().Element(BodyCell).Text(ad.AuthorName);
        table.Cell().Element(TypeCell).Text(FormatType(ad.Type));
        table.Cell().Element(BodyCell).Text(FormatCategory(ad.Category));
        table.Cell().Element(BodyCell).Text($"{FormatDate(ad.DateFrom)} - {FormatDate(ad.DateTo)}");

        table.Cell()
            .Element(container => StatusCell(container, ad.Status))
            .AlignCenter()
            .Text(FormatStatus(ad.Status));

        table.Cell().Element(BodyCell).Text(ad.Description);
    }

    private static QuestPdfContainer HeaderCell(QuestPdfContainer container)
    {
        return container
            .Background(Primary)
            .Border(1)
            .BorderColor(PrimaryDark)
            .Padding(7)
            .DefaultTextStyle(text => text.Bold().FontColor(Colors.White));
    }

    private static QuestPdfContainer BodyCell(QuestPdfContainer container)
    {
        return container
            .ShowEntire()
            .Background(Surface)
            .BorderBottom(1)
            .BorderColor(Border)
            .Padding(7)
            .DefaultTextStyle(text => text.FontColor(TextPrimary));
    }

    private static QuestPdfContainer TypeCell(QuestPdfContainer container)
    {
        return container
            .ShowEntire()
            .Background(PrimarySoft)
            .BorderBottom(1)
            .BorderColor(Border)
            .Padding(7)
            .DefaultTextStyle(text => text.SemiBold().FontColor(Primary));
    }

    private static QuestPdfContainer StatusCell(QuestPdfContainer container, AdStatus status)
    {
        return container
            .ShowEntire()
            .Background(GetStatusBackground(status))
            .BorderBottom(1)
            .BorderColor(Border)
            .Padding(7)
            .DefaultTextStyle(text => text.SemiBold().FontColor(GetStatusTextColor(status)));
    }

    private static string GetStatusBackground(AdStatus status)
    {
        return status switch
        {
            AdStatus.Active => ActiveSoft,
            AdStatus.Archived => ArchivedSoft,
            _ => SurfaceSoft
        };
    }

    private static string GetStatusTextColor(AdStatus status)
    {
        return status switch
        {
            AdStatus.Active => Active,
            AdStatus.Archived => Archived,
            _ => TextPrimary
        };
    }

    private static string FormatDate(DateOnly date)
    {
        return date.ToString("dd.MM.yyyy.");
    }

    private static string FormatType(AdType type)
    {
        return type switch
        {
            AdType.Offering => "Offering",
            AdType.Seeking => "Seeking",
            _ => type.ToString()
        };
    }

    private static string FormatCategory(AdCategory category)
    {
        return category switch
        {
            AdCategory.Moving => "Moving",
            AdCategory.ApplianceRepair => "Appliance repair",
            AdCategory.Lending => "Lending",
            AdCategory.Cleaning => "Cleaning",
            AdCategory.Other => "Other",
            _ => category.ToString()
        };
    }

    private static string FormatStatus(AdStatus status)
    {
        return status switch
        {
            AdStatus.Active => "Active",
            AdStatus.Archived => "Archived",
            _ => status.ToString()
        };
    }
}