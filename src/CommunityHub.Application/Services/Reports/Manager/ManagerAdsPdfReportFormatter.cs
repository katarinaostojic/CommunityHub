using CommunityHub.Application.Domain.Entities.Buildings.Ads;

namespace CommunityHub.Application.Services.Reports;

internal static class ManagerAdsPdfReportFormatter
{
    public static string FormatDate(DateOnly date)
    {
        return date.ToString("dd.MM.yyyy.");
    }

    public static string FormatType(AdType type)
    {
        return type switch
        {
            AdType.Offering => "Offering help",
            AdType.Seeking => "Seeking help",
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
}