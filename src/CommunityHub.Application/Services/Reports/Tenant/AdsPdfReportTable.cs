using CommunityHub.Application.Domain.Entities.Buildings.Ads;
using CommunityHub.Application.DTOs.Buildings.Ads;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

using QuestPdfContainer = QuestPDF.Infrastructure.IContainer;

namespace CommunityHub.Application.Services.Reports;

internal static class AdsPdfReportTable
{
    public static void AddAdsTable(ColumnDescriptor column, List<AdDto> ads)
    {
        if (ads.Count == 0)
        {
            AddEmptyTableMessage(column);
            return;
        }

        column.Item().Table(table =>
        {
            DefineColumns(table);
            AddHeader(table);

            foreach (AdDto ad in ads)
            {
                AddRow(table, ad);
            }
        });
    }

    private static void AddEmptyTableMessage(ColumnDescriptor column)
    {
        column.Item()
            .Background(AdsPdfReportStyles.Surface)
            .Border(1)
            .BorderColor(AdsPdfReportStyles.Border)
            .Padding(14)
            .Text("No ads were found for the selected period.")
            .Italic()
            .FontColor(AdsPdfReportStyles.TextSecondary);
    }

    private static void DefineColumns(TableDescriptor table)
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
        table.Cell().Element(TypeCell).Text(AdsPdfReportFormatter.FormatType(ad.Type));
        table.Cell().Element(BodyCell).Text(AdsPdfReportFormatter.FormatCategory(ad.Category));

        table.Cell().Element(BodyCell)
            .Text($"{AdsPdfReportFormatter.FormatDate(ad.DateFrom)} - {AdsPdfReportFormatter.FormatDate(ad.DateTo)}");

        table.Cell()
            .Element(container => StatusCell(container, ad.Status))
            .AlignCenter()
            .Text(AdsPdfReportFormatter.FormatStatus(ad.Status));

        table.Cell().Element(BodyCell).Text(ad.Description);
    }

    private static QuestPdfContainer HeaderCell(QuestPdfContainer container)
    {
        return container
            .Background(AdsPdfReportStyles.Primary)
            .Border(1)
            .BorderColor(AdsPdfReportStyles.PrimaryDark)
            .Padding(7)
            .DefaultTextStyle(text => text.Bold().FontColor(Colors.White));
    }

    private static QuestPdfContainer BodyCell(QuestPdfContainer container)
    {
        return container
            .ShowEntire()
            .Background(AdsPdfReportStyles.Surface)
            .BorderBottom(1)
            .BorderColor(AdsPdfReportStyles.Border)
            .Padding(7)
            .DefaultTextStyle(text => text.FontColor(AdsPdfReportStyles.TextPrimary));
    }

    private static QuestPdfContainer TypeCell(QuestPdfContainer container)
    {
        return container
            .ShowEntire()
            .Background(AdsPdfReportStyles.PrimarySoft)
            .BorderBottom(1)
            .BorderColor(AdsPdfReportStyles.Border)
            .Padding(7)
            .DefaultTextStyle(text => text.SemiBold().FontColor(AdsPdfReportStyles.Primary));
    }

    private static QuestPdfContainer StatusCell(QuestPdfContainer container, AdStatus status)
    {
        return container
            .ShowEntire()
            .Background(GetStatusBackground(status))
            .BorderBottom(1)
            .BorderColor(AdsPdfReportStyles.Border)
            .Padding(7)
            .DefaultTextStyle(text => text.SemiBold().FontColor(GetStatusTextColor(status)));
    }

    private static string GetStatusBackground(AdStatus status)
    {
        return status switch
        {
            AdStatus.Active => AdsPdfReportStyles.ActiveSoft,
            AdStatus.Archived => AdsPdfReportStyles.ArchivedSoft,
            _ => AdsPdfReportStyles.SurfaceSoft
        };
    }

    private static string GetStatusTextColor(AdStatus status)
    {
        return status switch
        {
            AdStatus.Active => AdsPdfReportStyles.Active,
            AdStatus.Archived => AdsPdfReportStyles.Archived,
            _ => AdsPdfReportStyles.TextPrimary
        };
    }
}