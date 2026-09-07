using CommunityHub.Application.DTOs.Neighborhoods.Budget;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CommunityHub.Application.Services.Reports.Citizen;

public static class ExpensesPdfExporter
{
    public static void Export(List<ExpenseDto> expenses, string neighborhoodName)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        string outputPath = BuildOutputPath(neighborhoodName);

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Column(col => AddHeader(col, neighborhoodName, expenses));
                page.Content().PaddingTop(16).Column(col => AddContent(col, expenses));
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Stranica ");
                    x.CurrentPageNumber();
                    x.Span(" od ");
                    x.TotalPages();
                    x.Span($"  |  Generisano: {DateTime.Now:dd.MM.yyyy HH:mm}");
                });
            });
        }).GeneratePdf(outputPath);

        OpenFile(outputPath);
    }

    private static string BuildOutputPath(string neighborhoodName)
    {
        string safe = string.Concat(neighborhoodName.Split(Path.GetInvalidFileNameChars()));
        string fileName = $"Istorija_Troskova_{safe}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
    }

    private static void AddHeader(ColumnDescriptor col, string neighborhoodName, List<ExpenseDto> expenses)
    {
        col.Item().Row(row =>
        {
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "zgrada.png");

            if (File.Exists(imagePath))
            {
                row.ConstantItem(56).Height(56).Image(imagePath).FitArea();
            }
            else
            {
                row.ConstantItem(56).Height(56)
                    .Background(Colors.Blue.Lighten4)
                    .AlignCenter().AlignMiddle()
                    .Text("OTP").FontSize(14).Bold().FontColor(Colors.Blue.Darken3);
            }

            row.ConstantItem(14);

            row.RelativeItem().Column(inner =>
            {
                inner.Item().Text("Istorija troškova")
                    .FontSize(22).Bold().FontColor(Colors.Blue.Darken3);
                inner.Item().Text($"Kvart: {neighborhoodName}")
                    .FontSize(13).FontColor(Colors.Grey.Darken1);
                decimal total = expenses.Sum(e => e.Amount);
                inner.Item().Text($"Ukupno: {total:0.##} RSD  |  Broj stavki: {expenses.Count}")
                    .FontSize(12).FontColor(Colors.Grey.Darken1);
            });
        });

        col.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Blue.Darken3);
    }

    private static void AddContent(ColumnDescriptor col, List<ExpenseDto> expenses)
    {
        if (!expenses.Any())
        {
            col.Item().PaddingTop(20).Text("Nema evidentiranih troškova.")
                .FontColor(Colors.Grey.Darken1).Italic();
            return;
        }

        col.Item().Table(table =>
        {
            table.ColumnsDefinition(cols =>
            {
                cols.RelativeColumn(2);
                cols.RelativeColumn(2);
                cols.RelativeColumn(4);
                cols.RelativeColumn(2);
            });

            table.Header(header =>
            {
                foreach (string title in new[] { "Kategorija", "Iznos (RSD)", "Opis", "Datum" })
                    header.Cell().Background(Colors.Blue.Darken3).Padding(8)
                        .Text(title).FontColor(Colors.White).Bold().FontSize(11);
            });

            bool even = false;
            foreach (var e in expenses)
            {
                string bg = even ? Colors.Grey.Lighten4 : Colors.White;
                even = !even;
                table.Cell().Background(bg).Padding(6).Text(e.CategoryName).FontSize(10);
                table.Cell().Background(bg).Padding(6).Text($"{e.Amount:0.##}").FontSize(10).Bold().FontColor(Colors.Blue.Darken2);
                table.Cell().Background(bg).Padding(6).Text(e.Description).FontSize(10);
                table.Cell().Background(bg).Padding(6).Text(e.CreatedAt).FontSize(10);
            }
        });

        decimal total = expenses.Sum(e => e.Amount);
        col.Item().PaddingTop(12).BorderTop(1).BorderColor(Colors.Blue.Darken3)
            .Row(row =>
            {
                row.RelativeItem().Text("UKUPNO:").Bold().FontSize(12);
                row.ConstantItem(120).Text($"{total:0.##} RSD").Bold().FontSize(12).FontColor(Colors.Blue.Darken3);
            });
    }

    private static void OpenFile(string path)
    {
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }
        catch { }
    }
}
