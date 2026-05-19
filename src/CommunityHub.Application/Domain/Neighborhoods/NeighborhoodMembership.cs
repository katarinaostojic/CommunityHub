using CommunityHub.Application.Domain.Shared;

namespace CommunityHub.Application.Domain.Neighborhoods;

public class NeighborhoodMembership
{
    public long Id { get; private set; }
    public User Citizen { get; private set; }
    public long NeighborhoodId { get; private set; }
    public DateTime JoinedAt { get; private set; }

    public NeighborhoodMembership(long id, User citizen, long neighborhoodId, DateTime joinedAt)
    {
        Id = id;
        Citizen = citizen;
        NeighborhoodId = neighborhoodId;
        JoinedAt = joinedAt;
    }
}