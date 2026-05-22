using CommunityHub.Application.DTOs.Buildings;

namespace CommunityHub.Application.Services.Interfaces.Buildings;

public interface IBuildingMembershipService
{
    List<BuildingMembershipDto> GetByTenant(long tenantId);
}