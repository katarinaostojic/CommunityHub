using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.DTOs.Ads;

namespace CommunityHub.Ui.ViewModels.ManagerViewModels.Ads;

public class ManagerAdRowViewModel
{
    private readonly AdDto _dto;

    public ManagerAdRowViewModel(AdDto dto)
    {
        _dto = dto;
    }

    public string TenantName => _dto.AuthorName;

    public string TypeDisplay => _dto.Type == AdType.Offering ? "Offering" : "Seeking";

    public string TypeBadgeColor => _dto.Type == AdType.Offering ? "#27AE60" : "#2980B9";

    public string CategoryDisplay => _dto.Category switch
    {
        AdCategory.Moving => "Moving",
        AdCategory.ApplianceRepair => "Appliance Repair",
        AdCategory.Lending => "Lending",
        AdCategory.Cleaning => "Cleaning",
        AdCategory.Other => "Other",
        _ => _dto.Category.ToString()
    };

    public string DateRangeDisplay =>
        $"{_dto.DateFrom:dd.MM.yyyy} - {_dto.DateTo:dd.MM.yyyy}";

    public string StatusDisplay => _dto.Status == AdStatus.Active ? "Active" : "Archived";

    public string StatusBadgeColor => _dto.Status == AdStatus.Active ? "#27AE60" : "#95A5A6";
}