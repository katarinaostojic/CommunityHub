namespace CommunityHub.Application.Domain.Neighborhoods;

public class MeetingVote
{
    public long Id { get; private set; }
    public long MeetingId { get; private set; }
    public long CitizenId { get; private set; }
    public DateOnly VotedDate { get; private set; }

    public MeetingVote(long id, long meetingId, long citizenId, DateOnly votedDate)
    {
        Id = id;
        MeetingId = meetingId;
        CitizenId = citizenId;
        VotedDate = votedDate;
    }

    public MeetingVote(long meetingId, long citizenId, DateOnly votedDate)
    {
        Id = 0;
        MeetingId = meetingId;
        CitizenId = citizenId;
        VotedDate = votedDate;
    }
}