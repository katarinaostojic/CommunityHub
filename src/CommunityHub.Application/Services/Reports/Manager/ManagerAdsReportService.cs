using CommunityHub.Application.Domain.Entities.Buildings.Ads;
using CommunityHub.Application.DTOs.Buildings.Ads;
using CommunityHub.Application.DTOs.Reports;
using CommunityHub.Application.Services.Entities.Buildings.Ads;

namespace CommunityHub.Application.Services.Reports;

public class ManagerAdsReportService
{
    private readonly AdStatisticsService _statisticsService;

    public ManagerAdsReportService(AdStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    public ManagerAdsReportDto Create(
        long buildingId,
        string buildingSubtitle,
        string managerName,
        AdType type)
    {
        List<AdDto> activeAdsOfType = _statisticsService
            .GetAllByBuilding(buildingId)
            .Where(ad => ad.Status == AdStatus.Active && ad.Type == type)
            .ToList();

        return new ManagerAdsReportDto(
            buildingSubtitle,
            managerName,
            type,
            activeAdsOfType);
    }
}