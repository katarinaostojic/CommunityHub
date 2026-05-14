using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.Domain.Buildings.BuildingRepositoryInterfaces;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Mappings.Buildings;

namespace CommunityHub.Application.Services.Buildings;

public class BuildingMembershipService
{
    private readonly IBuildingMembershipRepository _repository;

    public BuildingMembershipService(IBuildingMembershipRepository repository)
    {
        _repository = repository;
    }

    public List<BuildingMembershipDto> GetByTenant(long tenantId)
    {
        return _repository.GetByTenant(tenantId).ToDtoList();
    }
}