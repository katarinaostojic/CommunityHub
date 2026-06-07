using CommunityHub.Application.DTOs.Neighborhoods.Events;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

public class EventViewModel : BaseViewModel
{
    private readonly EventDto _event;

    public EventViewModel(EventDto ev)
    {
        _event = ev;
    }

    public long Id => _event.Id;
    public string Name => _event.Name;
    public string Description => _event.Description;
    public string EventDate => _event.EventDateFormatted;
    public string StartTime => _event.StartTimeFormatted;
    public int DurationMinutes => _event.DurationMinutes;
    public int MinVolunteers => _event.MinVolunteers;
    public int VolunteerCount => _event.VolunteerCount;
    public string OrganizerName => _event.OrganizerFullName;
    public List<EventItemDto> Items => _event.Items;
    public List<EventRegistrationDto> Registrations => _event.Registrations;
    public EventDto Event => _event;

    public List<AttendanceItemViewModel> AttendanceItems =>
        _event.Registrations.Select(r => new AttendanceItemViewModel(r)).ToList();

    public string StatusDisplay => _event.Status switch
    {
        "preparation" => "⏳ Preparation",
        "scheduled" => "✔ Scheduled",
        "cancelled" => "✕ Cancelled",
        "finished" => "✔ Finished",
        _ => _event.Status
    };
    public string StatusColor => _event.Status switch
    {
        "preparation" => "#F9A825",  // žuta
        "scheduled" => "#0D47A1",  // plava
        "cancelled" => "#B54A4A",  // crvena
        "finished" => "#1A5C2A",  // zelena
        _ => "#7F8C8D"
    };

    public bool CanRegister => _event.CanRegister;
    public bool IsOrganizer => _event.IsOrganizer;
    public bool IsFinished => _event.IsFinished;

    public Visibility OrganizerVisibility => IsOrganizer
        ? Visibility.Visible : Visibility.Collapsed;

    public Visibility JoinVisibility => CanRegister
        ? Visibility.Visible : Visibility.Collapsed;

    public Visibility AttendanceVisibility => IsOrganizer && IsFinished
        ? Visibility.Visible : Visibility.Collapsed;
}