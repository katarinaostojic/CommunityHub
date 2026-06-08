using CommunityHub.Application.Domain.Entities.Neighborhoods.Meetings;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CommunityHub.Application.Services.Reports;

public static class MeetingsReportPdfExporter
{
    public static void Export(List<Meeting> meetings, string reportType, DateOnly startDate, DateOnly endDate)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        string outputPath = BuildOutputPath();

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Column(col => AddHeader(col, reportType, startDate, endDate));
                page.Content().PaddingTop(16).Column(col => AddContent(col, meetings));
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                    x.Span(" of ");
                    x.TotalPages();
                });
            });
        }).GeneratePdf(outputPath);

        OpenFile(outputPath);
    }

    private static string BuildOutputPath()
    {
        string fileName = $"MeetingsReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
    }

    private static void AddHeader(ColumnDescriptor col, string reportType, DateOnly startDate, DateOnly endDate)
    {
        col.Item().Text("Meetings Report")
            .FontSize(22).Bold().FontColor(Colors.Green.Darken3);
        col.Item().Text($"Report type: {reportType}")
            .FontSize(12).FontColor(Colors.Grey.Darken1);
        col.Item().Text($"Period: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}")
            .FontSize(12).FontColor(Colors.Grey.Darken1);
        col.Item().Text($"Generated: {DateTime.Now:dd.MM.yyyy HH:mm}")
            .FontSize(10).FontColor(Colors.Grey.Medium);
        col.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Green.Darken3);
    }

    private static void AddContent(ColumnDescriptor col, List<Meeting> meetings)
    {
        if (!meetings.Any())
        {
            col.Item().Text("No meetings found for the selected criteria.")
                .FontColor(Colors.Grey.Darken1).Italic();
            return;
        }

        col.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(3);
                columns.RelativeColumn(2);
                columns.RelativeColumn(2);
                columns.RelativeColumn(2);
            });

            table.Header(header =>
            {
                foreach (string title in new[] { "Topic", "Date", "Time", "Status" })
                    header.Cell().Background(Colors.Green.Darken3)
                        .Padding(6).Text(title).FontColor(Colors.White).Bold();
            });

            AddTableRows(table, meetings);
        });
    }

    private static void AddTableRows(TableDescriptor table, List<Meeting> meetings)
    {
        bool alternate = false;
        foreach (var meeting in meetings)
        {
            var bg = alternate ? Colors.Grey.Lighten3 : Colors.White;
            alternate = !alternate;

            table.Cell().Background(bg).Padding(6).Text(FormatTopic(meeting));
            table.Cell().Background(bg).Padding(6).Text(FormatDate(meeting));
            table.Cell().Background(bg).Padding(6).Text($"{meeting.MeetingTime:HH:mm}h");
            table.Cell().Background(bg).Padding(6).Text(FormatStatus(meeting.Status));
        }
    }

    private static string FormatTopic(Meeting meeting)
    {
        if (meeting.Theme == MeetingTheme.Welcome) return "Welcome Meeting";
        if (meeting.Theme == MeetingTheme.Motivation) return "Community Motivation";
        if (meeting.Theme == MeetingTheme.Custom) return meeting.CustomThemeName ?? "Custom";
        return "Unknown";
    }

    private static string FormatDate(Meeting meeting) =>
        meeting.Status == MeetingStatus.Scheduled && meeting.ScheduledDate.HasValue
            ? meeting.ScheduledDate.Value.ToString("dd.MM.yyyy")
            : meeting.DateRangeStart.ToString("dd.MM.yyyy");

    private static string FormatStatus(MeetingStatus status) => status switch
    {
        MeetingStatus.Scheduled => "Scheduled",
        MeetingStatus.Cancelled => "Cancelled",
        MeetingStatus.InPreparation => "In Preparation",
        _ => status.ToString()
    };

    private static void OpenFile(string path)
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = path,
            UseShellExecute = true
        });
    }
}