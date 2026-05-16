using System;
using System.Collections.Generic;
using System.Text;

using CommunityHub.Application.Domain.Neighborhoods;
using CommunityHub.Application.DTOs.Neighborhoods;

namespace CommunityHub.Application.Mappings.Neighborhoods;

public static class EventMappingExtensions
{
    public static EventDto ToDto(this Event ev, long currentUserId = 0)
    {
        return new EventDto(
            id: ev.Id,
            name: ev.Name,
            description: ev.Description,
            eventDate: ev.EventDate,
            startTime: ev.StartTime,
            durationMinutes: ev.DurationMinutes,
            minVolunteers: ev.MinVolunteers,
            volunteerCount: ev.VolunteerCount,
            organizerFullName: $"{ev.Organizer.Name} {ev.Organizer.Surname}",
            status: ev.Status.ToString().ToLower(),
            items: ev.Items.Select(i => i.ToDto()).ToList(),
            isOrganizer: ev.IsOrganizer(currentUserId),
            registrations: ev.Registrations.Select(r => r.ToDto()).ToList()
        );
    }

    public static EventRegistrationDto ToDto(this EventRegistration registration)
    {
        return new EventRegistrationDto(
            id: registration.Id,
            citizenFullName: $"{registration.Citizen.Name} {registration.Citizen.Surname}",
            attended: registration.Attended
        );
    }

    public static List<EventDto> ToDtoList(this IEnumerable<Event> events, long currentUserId = 0)
    => events.Select(e => e.ToDto(currentUserId)).ToList();

    public static EventItemDto ToDto(this EventItem item)
    {
        return new EventItemDto(
            id: item.Id,
            name: item.Name,
            isTaken: item.IsTaken
        );
    }
}