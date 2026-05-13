using CommunityHub.Application.Domain.Neighborhoods;

namespace CommunityHub.Application.Domain.Neighborhoods.NeighborhoodRepositoryInterfaces;

public interface INeighborhoodMembershipRepository
{
    List<NeighborhoodMembership> GetByNeighborhood(long neighborhoodId);
}