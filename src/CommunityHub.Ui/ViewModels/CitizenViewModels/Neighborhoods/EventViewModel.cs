using System;
using System.Collections.Generic;
using System.Text;
using CommunityHub.Application.Domain.Neighborhoods;

namespace CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

public class EventViewModel : BaseViewModel
{
    private readonly Event _event;

    public EventViewModel(Event ev)
    {
        _event = ev;
    }

    public long Id => _event.Id;
    public string Name => _event.Name;
    public string Description => _event.Description;
    public string EventDate => _event.EventDate.ToString("dd/MM/yyyy");
    public string StartTime => _event.StartTime.ToString("HH:mm");
    public int DurationMinutes => _event.DurationMinutes;
    public int MinVolunteers => _event.MinVolunteers;
    public int VolunteerCount => _event.VolunteerCount;
    public string OrganizerName => $"{_event.Organizer.Name} {_event.Organizer.Surname}";
    public Event Event => _event;

    public string StatusDisplay => _event.Status switch
    {
        EventStatus.Preparation => "⏳ Preparation",
        EventStatus.Scheduled => "✔ Scheduled",
        EventStatus.Cancelled => "✕ Cancelled",
        EventStatus.Finished => "✔ Finished",
        _ => _event.Status.ToString()
    };

    public bool CanRegister => _event.Status == EventStatus.Preparation;
    public bool IsReadyToSchedule => _event.IsReadyToSchedule;
    public bool AllItemsTaken => _event.AllItemsTaken;
    public bool HasMinVolunteers => _event.HasMinVolunteers;

    public bool IsRegistered(long citizenId) => _event.IsRegistered(citizenId);
}
