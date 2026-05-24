using CommunityHub.Application.Database.Repositories.Neighborhoods;
using CommunityHub.Application.Domain.Neighborhoods;
using CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Mappings.Neighborhoods;

namespace CommunityHub.Application.Services;

public class MeetingService
{
    private readonly IMeetingRepository _repository;

    public MeetingService(IMeetingRepository repository)
    {
        _repository = repository;
    }

    public long CreateMeeting(Meeting meeting)
        => _repository.Create(meeting);

    public List<Meeting> GetMeetingsByCoordinator(long coordinatorId)
        => _repository.GetByCoordinator(coordinatorId);

    public void CheckAndFinalizeVoting(long meetingId)
    {
        Meeting? meeting = _repository.GetById(meetingId);
        if (meeting == null) return;

        var voteCounts = _repository.GetVoteCounts(meetingId);

        if (voteCounts.Count == 0)
            meeting.Cancel();
        else
        {
            DateOnly winningDate = voteCounts.OrderByDescending(v => v.Value).First().Key;
            meeting.Schedule(winningDate);
        }

        _repository.Update(meeting);
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
    public bool HasTiedVotes(long meetingId)
    {
        var voteCounts = _repository.GetVoteCounts(meetingId);
        if (voteCounts.Count < 2) return false;

        var sorted = voteCounts.OrderByDescending(v => v.Value).ToList();
        return sorted[0].Value == sorted[1].Value;
    }

    public Dictionary<DateOnly, int> GetVoteCounts(long meetingId)
        => _repository.GetVoteCounts(meetingId);

    public void ScheduleWithDate(long meetingId, DateOnly date)
    {
        Meeting? meeting = _repository.GetById(meetingId);
        if (meeting == null) return;
        meeting.Schedule(date);
        _repository.Update(meeting);
    }
}