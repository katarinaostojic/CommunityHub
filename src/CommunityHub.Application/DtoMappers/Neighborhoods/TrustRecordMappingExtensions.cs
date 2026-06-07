using System;
using System.Collections.Generic;
using System.Text;
using CommunityHub.Application.Domain.Entities;
using CommunityHub.Application.DTOs.Neighborhoods.Meetings;

namespace CommunityHub.Application.Mappings.Neighborhoods;

public static class TrustRecordMappingExtensions
{
    public static TrustRecordDto ToDto(this TrustRecord record, string citizenFullName)
    {
        return new TrustRecordDto(
            citizenId: record.CitizenId,
            citizenFullName: citizenFullName,
            eventsOrganized: record.EventsOrganized,
            eventsVolunteered: record.EventsVolunteered,
            joinedAt: record.JoinedAt.ToString("dd/MM/yyyy"),
            level: record.GetLevel()
        );
    }

    public static List<TrustRecordDto> ToDtoList(this IEnumerable<(TrustRecord record, string fullName)> records)
        => records.Select(r => r.record.ToDto(r.fullName)).ToList();
}
