using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityHub.Application.DTOs.Neighborhoods.Events;

public class EventDto
{
    public long Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public DateOnly EventDate { get; init; }
    public TimeOnly StartTime { get; init; }
    public int DurationMinutes { get; init; }
    public int MinVolunteers { get; init; }
    public int VolunteerCount { get; init; }
    public string OrganizerFullName { get; init; }
    public string Status { get; init; }
    public List<EventItemDto> Items { get; init; }
    public bool IsOrganizer { get; init; }
    public List<EventRegistrationDto> Registrations { get; init; }

    public EventDto(long id, string name, string description, DateOnly eventDate,
        TimeOnly startTime, int durationMinutes, int minVolunteers, int volunteerCount,
        string organizerFullName, string status, List<EventItemDto> items,
        bool isOrganizer = false, List<EventRegistrationDto>? registrations = null)
    {
        Id = id;
        Name = name;
        Description = description;
        EventDate = eventDate;
        StartTime = startTime;
        DurationMinutes = durationMinutes;
        MinVolunteers = minVolunteers;
        VolunteerCount = volunteerCount;
        OrganizerFullName = organizerFullName;
        Status = status;
        Items = items;
        IsOrganizer = isOrganizer;
        Registrations = registrations ?? new List<EventRegistrationDto>();
    }

    public string EventDateFormatted => EventDate.ToString("dd/MM/yyyy");
    public string StartTimeFormatted => StartTime.ToString("HH:mm");
    public bool CanRegister => Status == "preparation";
    public bool IsFinished => Status == "finished";
}

public class EventItemDto
{
    public long Id { get; init; }
    public string Name { get; init; }
    public bool IsTaken { get; init; }

    public EventItemDto(long id, string name, bool isTaken)
    {
        Id = id;
        Name = name;
        IsTaken = isTaken;
    }

    public string StatusDisplay => IsTaken ? "Already taken" : "Available";
    public bool IsAvailable => !IsTaken;
}

public class EventRegistrationDto
{
    public long Id { get; init; }
    public string CitizenFullName { get; init; }
    public bool? Attended { get; init; }

    public EventRegistrationDto(long id, string citizenFullName, bool? attended)
    {
        Id = id;
        CitizenFullName = citizenFullName;
        Attended = attended;
    }

    public string AttendanceDisplay => Attended == null ? "Not marked" :
        Attended.Value ? "✔ Attended" : "✕ Did not attend";
}