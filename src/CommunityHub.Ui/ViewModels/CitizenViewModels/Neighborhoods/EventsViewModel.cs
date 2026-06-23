using CommunityHub.Application.DTOs.Neighborhoods.Events;
using CommunityHub.Application.Services.Entities.Neighborhoods.Events;
using CommunityHub.Ui.Helpers.Citizen;
using System.Collections.ObjectModel;
using CommunityHub.Application.DTOs.Neighborhoods.Events;

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
        LanguageManager.LanguageChanged += () => { foreach (var e in Events) e.RefreshStatus(); LoadEvents(); };
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
        ResultsText = $"{ResourceHelper.Get("Lbl_Showing", "Showing")} {items.Count} {ResourceHelper.Get("Lbl_Events", "neighborhood events")}";
    }

    public long CreateEvent(CreateEventRequest req)
    {
        long eventId = _service.CreateEvent(req);
        LoadEvents();
        return eventId;
    }

    public void RegisterVolunteer(long eventId, long citizenId, List<long> selectedItemIds)
    {
        _service.RegisterVolunteer(eventId, citizenId, selectedItemIds);
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