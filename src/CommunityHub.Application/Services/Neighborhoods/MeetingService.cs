using CommunityHub.Application.Database.Repositories.Neighborhoods;
using CommunityHub.Application.Domain.Neighborhoods;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Mappings.Neighborhoods;

namespace CommunityHub.Application.Services;

public class MeetingService
{
    private readonly MeetingDbRepository _repository;

    public MeetingService(MeetingDbRepository repository)
    {
        _repository = repository;
    }

    public long CreateMeeting(Meeting meeting)
        => _repository.Create(meeting);

    public List<Meeting> GetMeetingsByCoordinator(long coordinatorId)
        => _repository.GetByCoordinator(coordinatorId);

    public void CheckAndFinalizeVoting(long meetingId)
    {
        var voteCounts = _repository.GetVoteCounts(meetingId);

        if (voteCounts.Count == 0)
        {
            _repository.UpdateStatus(meetingId, MeetingStatus.Cancelled, null);
            return;
        }

        DateOnly winningDate = voteCounts.OrderByDescending(v => v.Value).First().Key;
        _repository.UpdateStatus(meetingId, MeetingStatus.Scheduled, winningDate);
    }

    public void AddVote(long meetingId, long citizenId, DateOnly votedDate)
    {
        var vote = new MeetingVote(meetingId, citizenId, votedDate);
        _repository.AddVote(vote);
    }

    public List<MeetingDto> GetByNeighborhoodForCitizen(long neighborhoodId, long citizenId)
    {
        var meetings = _repository.GetByNeighborhood(neighborhoodId);
        return meetings.ToDtoList(
            canVoteFunc: m => CanVote(m),
            getVoteFunc: m =>
            {
                var vote = _repository.GetVoteForCitizen(m.Id, citizenId);
                return (vote?.VotedDate, vote?.Id);
            }
        );
    }

    public MeetingVote? GetVoteForCitizen(long meetingId, long citizenId)
        => _repository.GetVoteForCitizen(meetingId, citizenId);

    public void UpdateVote(long voteId, DateOnly newDate)
        => _repository.UpdateVote(voteId, newDate);

    public bool CanVote(Meeting meeting)
    {
        DateTime deadline = meeting.DateRangeStart.ToDateTime(TimeOnly.MinValue).AddHours(-24);
        return DateTime.Now < deadline && meeting.Status == MeetingStatus.InPreparation;
    }
}