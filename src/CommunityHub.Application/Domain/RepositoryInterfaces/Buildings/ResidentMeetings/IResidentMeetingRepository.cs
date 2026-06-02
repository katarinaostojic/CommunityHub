using CommunityHub.Application.Domain.Entities.Buildings.ResidentMeetings;

namespace CommunityHub.Application.Domain.RepositoryInterfaces.Buildings.ResidentMeetings;

public interface IResidentMeetingRepository
{
    List<ResidentMeeting> GetByTenantAndBuilding(
        long tenantId,
        long buildingId,
        ResidentMeetingStatus? status);

    int CountByTenantAndBuilding(
        long tenantId,
        long buildingId,
        ResidentMeetingStatus? status);

    List<ResidentMeeting> GetActiveByBuilding(long buildingId);

    ResidentMeeting? GetById(long meetingId, long tenantId);

    ResidentMeetingAttendance? GetAttendance(long meetingId, string unitNumber);

    void CreateAttendance(ResidentMeetingAttendance attendance);

    void DeleteAttendance(long meetingId, string unitNumber);

    void CreateTopicSuggestion(ResidentMeetingTopicSuggestion suggestion);

    void UpdateStatus(long meetingId, ResidentMeetingStatus status);
}