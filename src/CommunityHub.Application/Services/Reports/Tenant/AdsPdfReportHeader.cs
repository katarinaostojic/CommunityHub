using CommunityHub.Application.DTOs.Reports;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace CommunityHub.Application.Services.Reports;

internal static class AdsPdfReportHeader
{
    public static void AddAppBanner(ColumnDescriptor column, AdsReportDto report)
    {
        column.Item()
            .Background(AdsPdfReportStyles.Primary)
            .Padding(14)
            .Row(row =>
            {
                AddLogo(row);
                AddApplicationInfo(row);
                AddTenantInfo(row, report.TenantName);
            });
    }

    public static void AddReportHeader(ColumnDescriptor column, AdsReportDto report)
    {
        column.Item()
            .Background(AdsPdfReportStyles.Surface)
            .Border(1)
            .BorderColor(AdsPdfReportStyles.Border)
            .Padding(16)
            .Column(header =>
            {
                header.Item().Text("Tenant Ads Report")
                    .FontSize(20)
                    .Bold()
                    .FontColor(AdsPdfReportStyles.Primary);

                header.Item().PaddingTop(4)
                    .Text($"Building: {report.BuildingSubtitle}")
                    .FontSize(12)
                    .FontColor(AdsPdfReportStyles.TextSecondary);

                header.Item().PaddingTop(2)
                    .Text($"Period: {AdsPdfReportFormatter.FormatDate(report.DateFrom)} - {AdsPdfReportFormatter.FormatDate(report.DateTo)}")
                    .FontSize(12)
                    .FontColor(AdsPdfReportStyles.TextSecondary);
            });
    }

    private static void AddLogo(RowDescriptor row)
    {
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();

        using var stream = assembly.GetManifestResourceStream(
            "CommunityHub.Application.Resources.AppLogo.communityhubLogo.png");

        if (stream == null)
            return;

        row.ConstantItem(100).Height(60).AlignMiddle().Image(stream).FitArea();
    }

    private static void AddApplicationInfo(RowDescriptor row)
    {
        row.RelativeItem().PaddingLeft(10).Column(left =>
        {
            left.Item().Text("CommunityHub")
                .FontSize(18)
                .Bold()
                .FontColor(Colors.White);

            left.Item().PaddingTop(2)
                .Text("Notice board")
                .FontSize(9)
                .FontColor(AdsPdfReportStyles.PrimarySoft);
        });
    }

    private static void AddTenantInfo(RowDescriptor row, string tenantName)
    {
        row.ConstantItem(160).AlignRight().Column(right =>
        {
            right.Item().AlignRight()
                .Text("Generated for")
                .FontSize(8)
                .FontColor(AdsPdfReportStyles.PrimarySoft);

            right.Item().AlignRight()
                .Text(tenantName)
                .FontSize(13)
                .Bold()
                .FontColor(Colors.White);
        });
    }
}