using CommunityHub.Application.Domain.Entities.Neighborhoods.Meetings;

namespace CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods.Meetings;

public interface IMeetingRepository
{
    long Create(Meeting meeting);
    List<Meeting> GetByCoordinator(long coordinatorId);
    List<Meeting> GetByNeighborhood(long neighborhoodId);
    Meeting? GetById(long meetingId);
    void Update(Meeting meeting);
    void UpdateStatus(long meetingId, MeetingStatus status, DateOnly? scheduledDate);
    void AddVote(MeetingVote vote);
    Dictionary<DateOnly, int> GetVoteCounts(long meetingId);
    MeetingVote? GetVoteForCitizen(long meetingId, long citizenId);
    void UpdateVote(long voteId, DateOnly newDate);
}