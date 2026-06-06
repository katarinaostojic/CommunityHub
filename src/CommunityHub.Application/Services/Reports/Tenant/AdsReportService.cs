using CommunityHub.Application.DTOs.Buildings.Ads;
using CommunityHub.Application.DTOs.Reports;
using CommunityHub.Application.Services.Entities.Buildings.Ads;

namespace CommunityHub.Application.Services.Reports;

public class AdsReportService
{
    private readonly AdService _adService;

    public AdsReportService(AdService adService)
    {
        _adService = adService;
    }

    public AdsReportDto Create(
        long buildingId,
        string buildingSubtitle,
        string tenantName,
        DateOnly dateFrom,
        DateOnly dateTo)
    {
        List<AdDto> ads = _adService.GetReportAds(buildingId, dateFrom, dateTo);

        return new AdsReportDto(
            buildingSubtitle,
            tenantName,
            dateFrom,
            dateTo,
            ads);
    }
}