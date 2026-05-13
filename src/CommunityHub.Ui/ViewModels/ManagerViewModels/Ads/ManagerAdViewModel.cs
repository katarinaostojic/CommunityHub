using CommunityHub.Application.Domain.Ads;

namespace CommunityHub.Ui.ViewModels.ManagerViewModels.Ads;

public class ManagerAdViewModel
{
    private readonly Ad _ad;

    public ManagerAdViewModel(Ad ad)
    {
        _ad = ad;
    }

    public string TenantName => $"{_ad.Author.Name} {_ad.Author.Surname}";

    public string TypeDisplay => _ad.Type == AdType.Offering ? "Offering" : "Seeking";

    public string TypeBadgeColor => _ad.Type == AdType.Offering ? "#27AE60" : "#2980B9";

    public string CategoryDisplay => _ad.Category switch
    {
        AdCategory.Moving => "Moving",
        AdCategory.ApplianceRepair => "Appliance Repair",
        AdCategory.Lending => "Lending",
        AdCategory.Cleaning => "Cleaning",
        AdCategory.Other => "Other",
        _ => _ad.Category.ToString()
    };

    public string DateRangeDisplay =>
        $"{_ad.DateFrom:dd.MM.yyyy} - {_ad.DateTo:dd.MM.yyyy}";

    public string StatusDisplay => _ad.Status == AdStatus.Active ? "Active" : "Archived";

    public string StatusBadgeColor => _ad.Status == AdStatus.Active ? "#27AE60" : "#95A5A6";
}