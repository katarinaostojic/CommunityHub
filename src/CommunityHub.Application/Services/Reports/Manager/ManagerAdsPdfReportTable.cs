using CommunityHub.Application.DTOs.Buildings.Ads;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

using QuestPdfContainer = QuestPDF.Infrastructure.IContainer;

namespace CommunityHub.Application.Services.Reports;

internal static class ManagerAdsPdfReportTable
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
            .Background(ManagerAdsPdfReportStyles.Surface)
            .Border(1)
            .BorderColor(ManagerAdsPdfReportStyles.Border)
            .Padding(14)
            .Text("There are currently no active ads of this type for the selected building.")
            .Italic()
            .FontColor(ManagerAdsPdfReportStyles.TextSecondary);
    }

    private static void DefineColumns(TableDescriptor table)
    {
        table.ColumnsDefinition(columns =>
        {
            columns.RelativeColumn(1.5f);
            columns.RelativeColumn(1.3f);
            columns.RelativeColumn(1.5f);
            columns.RelativeColumn(3);
        });
    }

    private static void AddHeader(TableDescriptor table)
    {
        table.Header(header =>
        {
            header.Cell().Element(HeaderCell).Text("Tenant");
            header.Cell().Element(HeaderCell).Text("Category");
            header.Cell().Element(HeaderCell).Text("Date range");
            header.Cell().Element(HeaderCell).Text("Description");
        });
    }

    private static void AddRow(TableDescriptor table, AdDto ad)
    {
        table.Cell().Element(BodyCell).Text(ad.AuthorName);
        table.Cell().Element(BodyCell).Text(ManagerAdsPdfReportFormatter.FormatCategory(ad.Category));

        table.Cell().Element(BodyCell)
            .Text($"{ManagerAdsPdfReportFormatter.FormatDate(ad.DateFrom)} - {ManagerAdsPdfReportFormatter.FormatDate(ad.DateTo)}");

        table.Cell().Element(BodyCell).Text(ad.Description);
    }

    private static QuestPdfContainer HeaderCell(QuestPdfContainer container)
    {
        return container
            .Background(ManagerAdsPdfReportStyles.Primary)
            .Border(1)
            .BorderColor(ManagerAdsPdfReportStyles.PrimaryDark)
            .Padding(7)
            .DefaultTextStyle(text => text.Bold().FontColor(Colors.White));
    }

    private static QuestPdfContainer BodyCell(QuestPdfContainer container)
    {
        return container
            .ShowEntire()
            .Background(ManagerAdsPdfReportStyles.Surface)
            .BorderBottom(1)
            .BorderColor(ManagerAdsPdfReportStyles.Border)
            .Padding(7)
            .DefaultTextStyle(text => text.FontColor(ManagerAdsPdfReportStyles.TextPrimary));
    }
}