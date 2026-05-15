using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;

namespace CommunityHub.Application.Services;

public class MeetingService
{
    private readonly MeetingDbRepository _repository;

    public MeetingService(MeetingDbRepository repository)
    {
        _repository = repository;
    }

    public long CreateMeeting(Meeting meeting)
    {
        return _repository.Create(meeting);
    }

    public List<Meeting> GetMeetingsByCoordinator(long coordinatorId)
    {
        return _repository.GetByCoordinator(coordinatorId);
    }

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
}