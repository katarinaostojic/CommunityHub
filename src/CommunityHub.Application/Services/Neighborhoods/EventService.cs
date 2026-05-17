using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Neighborhoods;
using CommunityHub.Application.Domain.Neighborhoods.NeighborhoodRepositoryInterfaces;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Mappings.Neighborhoods;

namespace CommunityHub.Application.Services.Neighborhoods;

public class EventService
{
    private readonly IEventRepository _repository;

    public EventService(IEventRepository repository)
    {
        _repository = repository;
    }

    public List<EventDto> GetByNeighborhood(long neighborhoodId, long currentUserId = 0)
        => _repository.GetByNeighborhood(neighborhoodId).ToDtoList(currentUserId);

    public Event? GetById(long eventId)
        => _repository.GetById(eventId);

    public long CreateEvent(User organizer, long neighborhoodId, string name, string description,
        DateOnly eventDate, TimeOnly startTime, int durationMinutes, int minVolunteers, List<string> itemNames)
    {
        Event ev = new Event(0, neighborhoodId, organizer, name, description,
            eventDate, startTime, durationMinutes, minVolunteers, EventStatus.Preparation);

        long eventId = _repository.Create(ev);

        foreach (string itemName in itemNames)
            _repository.CreateItem(eventId, itemName);

        return eventId;
    }

    public void RegisterVolunteer(long eventId, User citizen, List<long> selectedItemIds)
    {
        _repository.AddRegistration(eventId, citizen.Id, selectedItemIds);

        Event? updated = _repository.GetById(eventId);
        if (updated == null) return;

        if (updated.IsReadyToSchedule)
        {
            updated.Schedule();
            _repository.Update(updated);
        }
    }

    public bool IsAlreadyRegistered(long eventId, long citizenId)
    {
        Event? ev = _repository.GetById(eventId);
        return ev?.IsRegistered(citizenId) ?? false;
    }

    public void CheckAndUpdateStatuses()
    {
        DateTime now = DateTime.Now;
        var events = _repository.GetAllForStatusCheck();

        foreach (var ev in events)
        {
            ev.CheckAndCancel(now);
            ev.CheckAndFinish(now);
            _repository.Update(ev);
        }
    }

    public void MarkAttendance(long registrationId, bool attended)
        => _repository.MarkAttendance(registrationId, attended);
}