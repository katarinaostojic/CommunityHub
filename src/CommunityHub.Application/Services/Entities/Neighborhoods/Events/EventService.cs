using CommunityHub.Application.Domain.Entities.Neighborhoods.Events;
using CommunityHub.Application.DTOs.Neighborhoods.Events;
using CommunityHub.Application.Mappings.Neighborhoods;
using CommunityHub.Application.DTOs.Neighborhoods.Events;
using CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods.Events;

namespace CommunityHub.Application.Services.Entities.Neighborhoods.Events;

public class EventService
{
    private readonly IEventRepository _repository;

    public EventService(IEventRepository repository)
    {
        _repository = repository;
    }

    public List<EventDto> GetByNeighborhood(long neighborhoodId, long currentUserId = 0)
        => _repository.GetByNeighborhood(neighborhoodId).ToDtoList(currentUserId);

    public long CreateEvent(CreateEventRequest req)
    {
        long eventId = _repository.Create(req.OrganizerId, req.NeighborhoodId,
            req.Name, req.Description, req.EventDate, req.StartTime,
            req.DurationMinutes, req.MinVolunteers);

        foreach (string itemName in req.ItemNames)
            _repository.CreateItem(eventId, itemName);

        return eventId;
    }

    public void RegisterVolunteer(long eventId, long citizenId, List<long> selectedItemIds)
    {
        _repository.AddRegistration(eventId, citizenId, selectedItemIds);

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