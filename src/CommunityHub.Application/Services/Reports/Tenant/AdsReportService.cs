using CommunityHub.Application.DTOs.Ads;
using CommunityHub.Application.DTOs.Reports;
using CommunityHub.Application.Services.Interfaces.Ads;

namespace CommunityHub.Application.Services.Reports;

public class AdsReportService
{
    private readonly IAdService _adService;

    public AdsReportService(IAdService adService)
    {
        _adService = adService;
    }

    public AdsReportDto Create(
        long buildingId,
        string buildingSubtitle,
        DateOnly dateFrom,
        DateOnly dateTo)
    {
        List<AdDto> ads = _adService.GetReportAds(buildingId, dateFrom, dateTo);

        return new AdsReportDto(
            buildingSubtitle,
            dateFrom,
            dateTo,
            ads);
    }
}