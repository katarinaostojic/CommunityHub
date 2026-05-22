using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Mappings.Buildings;
using CommunityHub.Application.Services.Interfaces.Buildings;

namespace CommunityHub.Application.Services.Entities.Buildings;

public class BuildingMembershipService : IBuildingMembershipService
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