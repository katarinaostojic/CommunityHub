using CommunityHub.Application.Domain.Entities.Buildings.Ads;

namespace CommunityHub.Ui.Extensions.Buildings.Ads;

public static class AdCategoryExtensions
{
    public static string ToDisplayString(this AdCategory category) => category switch
    {
        AdCategory.Moving => "Moving",
        AdCategory.ApplianceRepair => "Appliance repair",
        AdCategory.Lending => "Lending",
        AdCategory.Cleaning => "Cleaning",
        AdCategory.Other => "Other",
        _ => category.ToString()
    };
}