using CommunityHub.Application.Domain.Entities.Buildings.Ads;
using CommunityHub.Application.DTOs.Buildings.Ads;

namespace CommunityHub.Application.DTOs.Reports;

public class AdsReportDto
{
    public AdsReportDto(
        string buildingSubtitle,
        string tenantName,
        DateOnly dateFrom,
        DateOnly dateTo,
        List<AdDto> ads)
    {
        BuildingSubtitle = buildingSubtitle;
        TenantName = tenantName;
        DateFrom = dateFrom;
        DateTo = dateTo;
        Ads = ads;
    }

    public string BuildingSubtitle { get; }
    public string TenantName { get; }
    public DateOnly DateFrom { get; }
    public DateOnly DateTo { get; }
    public List<AdDto> Ads { get; }

    public int TotalAds => Ads.Count;
    public int OfferingAds => Ads.Count(ad => ad.Type == AdType.Offering);
    public int SeekingAds => Ads.Count(ad => ad.Type == AdType.Seeking);
    public int ActiveAds => Ads.Count(ad => ad.Status == AdStatus.Active);
    public int ArchivedAds => Ads.Count(ad => ad.Status == AdStatus.Archived);
}