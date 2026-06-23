using CommunityHub.Application.Domain.Entities.Buildings.Ads;
using CommunityHub.Application.DTOs.Buildings.Ads;

namespace CommunityHub.Application.DTOs.Reports;

public class ManagerAdsReportDto
{
    public ManagerAdsReportDto(
        string buildingSubtitle,
        string managerName,
        AdType type,
        List<AdDto> ads)
    {
        BuildingSubtitle = buildingSubtitle;
        ManagerName = managerName;
        Type = type;
        Ads = ads;
    }

    public string BuildingSubtitle { get; }
    public string ManagerName { get; }
    public AdType Type { get; }
    public List<AdDto> Ads { get; }

    public int TotalAds => Ads.Count;
}