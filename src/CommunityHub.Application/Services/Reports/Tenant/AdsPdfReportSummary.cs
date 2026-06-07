using CommunityHub.Application.DTOs.Reports;
using QuestPDF.Fluent;

namespace CommunityHub.Application.Services.Reports;

internal static class AdsPdfReportSummary
{
    public static void AddSummary(ColumnDescriptor column, AdsReportDto report)
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

            AddSummaryCell(table, "Total ads", report.TotalAds, AdsPdfReportStyles.Primary, AdsPdfReportStyles.Surface);
            AddSummaryCell(table, "Offering", report.OfferingAds, AdsPdfReportStyles.Primary, AdsPdfReportStyles.Surface);
            AddSummaryCell(table, "Seeking", report.SeekingAds, AdsPdfReportStyles.Primary, AdsPdfReportStyles.Surface);
            AddSummaryCell(table, "Active", report.ActiveAds, AdsPdfReportStyles.Active, AdsPdfReportStyles.ActiveSoft);
            AddSummaryCell(table, "Archived", report.ArchivedAds, AdsPdfReportStyles.Archived, AdsPdfReportStyles.ArchivedSoft);
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
            .BorderColor(AdsPdfReportStyles.Border)
            .Padding(10)
            .Column(column =>
            {
                column.Item().Text(label)
                    .FontSize(9)
                    .SemiBold()
                    .FontColor(AdsPdfReportStyles.TextSecondary);

                column.Item().PaddingTop(4)
                    .Text(value.ToString())
                    .FontSize(18)
                    .Bold()
                    .FontColor(valueColor);
            });
    }
}