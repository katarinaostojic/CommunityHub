using CommunityHub.Application.Domain.Entities.Neighborhoods;

namespace CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods;

public interface INeighborhoodMembershipRepository
{
    List<NeighborhoodMembership> GetByNeighborhood(long neighborhoodId);
}