using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityHub.Application.DTOs.Neighborhoods;

public class CreateEventRequest
{
    public long OrganizerId { get; init; }
    public long NeighborhoodId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateOnly EventDate { get; init; }
    public TimeOnly StartTime { get; init; }
    public int DurationMinutes { get; init; }
    public int MinVolunteers { get; init; }
    public List<string> ItemNames { get; init; } = new();
}
