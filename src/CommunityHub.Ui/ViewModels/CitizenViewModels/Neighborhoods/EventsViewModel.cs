using CommunityHub.Application.Domain;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Services.Neighborhoods;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

public class EventsViewModel : BaseViewModel
{
    private readonly EventService _service;
    private readonly long _neighborhoodId;
    private readonly long _currentUserId;

    private ObservableCollection<EventViewModel> _events = new();
    private string _resultsText = string.Empty;

    public EventsViewModel(EventService service, long neighborhoodId, long currentUserId)
    {
        _service = service;
        _neighborhoodId = neighborhoodId;
        _currentUserId = currentUserId;
        _service.CheckAndUpdateStatuses();
        LoadEvents();
    }

    public ObservableCollection<EventViewModel> Events
    {
        get => _events;
        private set => SetProperty(ref _events, value);
    }

    public string ResultsText
    {
        get => _resultsText;
        private set => SetProperty(ref _resultsText, value);
    }

    public void LoadEvents()
    {
        var items = _service.GetByNeighborhood(_neighborhoodId, _currentUserId)
            .Select(e => new EventViewModel(e))
            .ToList();

        Events = new ObservableCollection<EventViewModel>(items);
        ResultsText = $"Showing {items.Count} events";
    }

    public long CreateEvent(User organizer, string name, string description,
        DateOnly eventDate, TimeOnly startTime, int durationMinutes,
        int minVolunteers, List<string> itemNames)
    {
        long eventId = _service.CreateEvent(organizer, _neighborhoodId, name, description,
            eventDate, startTime, durationMinutes, minVolunteers, itemNames);
        LoadEvents();
        return eventId;
    }

    public void RegisterVolunteer(long eventId, User citizen, List<long> selectedItemIds)
    {
        _service.RegisterVolunteer(eventId, citizen, selectedItemIds);
        LoadEvents();
    }

    public bool IsAlreadyRegistered(long eventId, long citizenId)
        => _service.IsAlreadyRegistered(eventId, citizenId);

    public void MarkAttendance(long registrationId, bool attended)
    {
        _service.MarkAttendance(registrationId, attended);
        LoadEvents();
    }
}