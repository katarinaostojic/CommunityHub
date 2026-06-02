namespace CommunityHub.Application.Domain.Entities.Buildings.ResidentMeetings;

public class ResidentMeetingAttendance
{
    public long Id { get; private set; }
    public long MeetingId { get; private set; }
    public long TenantId { get; private set; }
    public string UnitNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public ResidentMeetingAttendance(
        long id,
        long meetingId,
        long tenantId,
        string unitNumber,
        DateTime createdAt)
    {
        Id = id;
        MeetingId = meetingId;
        TenantId = tenantId;
        UnitNumber = unitNumber;
        CreatedAt = createdAt;
    }

    public ResidentMeetingAttendance(long meetingId, long tenantId, string unitNumber)
    {
        Id = 0;
        MeetingId = meetingId;
        TenantId = tenantId;
        UnitNumber = unitNumber;
        CreatedAt = DateTime.UtcNow;
    }
}