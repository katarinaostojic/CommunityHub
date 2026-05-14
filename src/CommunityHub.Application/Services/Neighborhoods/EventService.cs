using System;
using System.Collections.Generic;
using System.Text;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Neighborhoods;
using CommunityHub.Application.Domain.Neighborhoods.NeighborhoodRepositoryInterfaces;

namespace CommunityHub.Application.Services.Neighborhoods;

public class EventService
{
    private readonly IEventRepository _repository;

    public EventService(IEventRepository repository)
    {
        _repository = repository;
    }

    public List<Event> GetByNeighborhood(long neighborhoodId)
        => _repository.GetByNeighborhood(neighborhoodId);

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

    public void RegisterVolunteer(Event ev, User citizen, List<long> selectedItemIds)
    {
        _repository.AddRegistration(ev.Id, citizen.Id, selectedItemIds);

        Event? updated = _repository.GetById(ev.Id);
        if (updated == null) return;

        if (updated.IsReadyToSchedule)
        {
            updated.Schedule();
            _repository.Update(updated);
        }
    }

    public bool IsAlreadyRegistered(Event ev, long citizenId)
        => ev.IsRegistered(citizenId);
}
