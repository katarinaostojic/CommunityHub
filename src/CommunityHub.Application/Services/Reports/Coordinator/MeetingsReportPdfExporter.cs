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

        string fileName = $"MeetingsReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
        string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Column(col =>
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
                });

                page.Content().PaddingTop(16).Column(col =>
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
                            header.Cell().Background(Colors.Green.Darken3)
                                .Padding(6).Text("Topic").FontColor(Colors.White).Bold();
                            header.Cell().Background(Colors.Green.Darken3)
                                .Padding(6).Text("Date").FontColor(Colors.White).Bold();
                            header.Cell().Background(Colors.Green.Darken3)
                                .Padding(6).Text("Time").FontColor(Colors.White).Bold();
                            header.Cell().Background(Colors.Green.Darken3)
                                .Padding(6).Text("Status").FontColor(Colors.White).Bold();
                        });

                        bool alternate = false;
                        foreach (var meeting in meetings)
                        {
                            var bg = alternate ? Colors.Grey.Lighten3 : Colors.White;
                            alternate = !alternate;

                            string topic = meeting.Theme switch
                            {
                                MeetingTheme.Welcome => "Welcome Meeting",
                                MeetingTheme.Motivation => "Community Motivation",
                                MeetingTheme.Custom => meeting.CustomThemeName ?? "Custom",
                                _ => "Unknown"
                            };

                            string date = meeting.Status == MeetingStatus.Scheduled && meeting.ScheduledDate.HasValue
                                ? meeting.ScheduledDate.Value.ToString("dd.MM.yyyy")
                                : meeting.DateRangeStart.ToString("dd.MM.yyyy");

                            string status = meeting.Status switch
                            {
                                MeetingStatus.Scheduled => "Scheduled",
                                MeetingStatus.Cancelled => "Cancelled",
                                MeetingStatus.InPreparation => "In Preparation",
                                _ => meeting.Status.ToString()
                            };

                            table.Cell().Background(bg).Padding(6).Text(topic);
                            table.Cell().Background(bg).Padding(6).Text(date);
                            table.Cell().Background(bg).Padding(6).Text($"{meeting.MeetingTime:HH:mm}h");
                            table.Cell().Background(bg).Padding(6).Text(status);
                        }
                    });
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                    x.Span(" of ");
                    x.TotalPages();
                });
            });
        }).GeneratePdf(outputPath);

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = outputPath,
            UseShellExecute = true
        });
    }
}