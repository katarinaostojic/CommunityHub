using CommunityHub.Application.Domain.Entities.Buildings.Ads;

namespace CommunityHub.Application.Services.Reports;

internal static class AdsPdfReportFormatter
{
    public static string FormatDate(DateOnly date)
    {
        return date.ToString("dd.MM.yyyy.");
    }

    public static string FormatType(AdType type)
    {
        return type switch
        {
            AdType.Offering => "Offering",
            AdType.Seeking => "Seeking",
            _ => type.ToString()
        };
    }

    public static string FormatCategory(AdCategory category)
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

    public static string FormatStatus(AdStatus status)
    {
        return status switch
        {
            AdStatus.Active => "Active",
            AdStatus.Archived => "Archived",
            _ => status.ToString()
        };
    }
}