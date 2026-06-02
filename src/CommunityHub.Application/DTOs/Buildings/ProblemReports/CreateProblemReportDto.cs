using CommunityHub.Application.Domain.Entities.Buildings.ProblemReports;
using CommunityHub.Application.Domain.Entities.Shared;

namespace CommunityHub.Application.DTOs.Buildings.ProblemReports;

public class CreateProblemReportDto
{
    public long BuildingId { get; init; }
    public User Tenant { get; init; }
    public string Description { get; init; }
    public ProblemPriority Priority { get; init; }

    public CreateProblemReportDto(
        long buildingId,
        User tenant,
        string description,
        ProblemPriority priority)
    {
        BuildingId = buildingId;
        Tenant = tenant;
        Description = description;
        Priority = priority;
    }
}