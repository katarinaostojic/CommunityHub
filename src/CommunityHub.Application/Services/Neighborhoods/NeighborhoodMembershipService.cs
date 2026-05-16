using CommunityHub.Application.Domain.Neighborhoods.NeighborhoodRepositoryInterfaces;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Mappings.Neighborhoods;

namespace CommunityHub.Application.Services.Neighborhoods;

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