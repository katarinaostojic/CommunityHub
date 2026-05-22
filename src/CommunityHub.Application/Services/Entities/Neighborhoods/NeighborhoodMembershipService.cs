using CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Mappings.Neighborhoods;

namespace CommunityHub.Application.Services.Entities.Neighborhoods;

public class NeighborhoodMembershipService
{
    private readonly INeighborhoodMembershipRepository _repository;

    public NeighborhoodMembershipService(INeighborhoodMembershipRepository repository)
    {
        _repository = repository;
    }

    public List<NeighborhoodMembershipDto> GetByNeighborhood(long neighborhoodId)
        => _repository.GetByNeighborhood(neighborhoodId).ToDtoList();
}