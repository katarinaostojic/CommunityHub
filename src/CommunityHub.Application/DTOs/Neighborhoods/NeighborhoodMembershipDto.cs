using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityHub.Application.DTOs.Neighborhoods;

public class NeighborhoodMembershipDto
{
    public long Id { get; init; }
    public string CitizenName { get; init; }
    public string CitizenSurname { get; init; }
    public string? CitizenAddress { get; init; }
    public DateTime JoinedAt { get; init; }

    public NeighborhoodMembershipDto(long id, string citizenName, string citizenSurname,
        string? citizenAddress, DateTime joinedAt)
    {
        Id = id;
        CitizenName = citizenName;
        CitizenSurname = citizenSurname;
        CitizenAddress = citizenAddress;
        JoinedAt = joinedAt;
    }

    public string FullName => $"{CitizenName} {CitizenSurname}";
    public string Address => CitizenAddress ?? "No address";
    public string JoinedAtFormatted => $"Joined: {JoinedAt:dd.MM.yyyy}";
}
