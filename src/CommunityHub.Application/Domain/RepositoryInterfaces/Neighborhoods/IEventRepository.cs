using CommunityHub.Application.Domain.Entities.Neighborhoods;

namespace CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods;

public interface IEventRepository
{
    long Create(long organizerId, long neighborhoodId, string name, string description,
        DateOnly eventDate, TimeOnly startTime, int durationMinutes, int minVolunteers);
    void CreateItem(long eventId, string itemName);
    List<Event> GetByNeighborhood(long neighborhoodId);
    Event? GetById(long eventId);
    void Update(Event ev);
    void AddRegistration(long eventId, long citizenId, List<long> itemIds);
    void MarkAttendance(long registrationId, bool attended);
    List<Event> GetAllForStatusCheck();
}