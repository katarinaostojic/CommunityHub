using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CommunityHub.Application.Database.Mappers.Users;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Domain.Entities.Neighborhoods.Events;

namespace CommunityHub.Application.Database.Mappers;

public static class EventMapper
{
    public static Event MapEvent(IDataReader reader)
    {
        return new Event(
            Convert.ToInt64(reader["id"]),
            Convert.ToInt64(reader["neighborhood_id"]),
            MapOrganizer(reader),
            reader["name"].ToString()!,
            reader["description"].ToString()!,
            (DateOnly)reader["event_date"],
            (TimeOnly)reader["start_time"],
            Convert.ToInt32(reader["duration_minutes"]),
            Convert.ToInt32(reader["min_volunteers"]),
            ParseEventStatus(reader["status"].ToString()!)
        );
    }

    public static User MapOrganizer(IDataReader reader)
    {
        return new User(
            Convert.ToInt64(reader["organizer_id"]),
            reader["username"].ToString()!,
            reader["password"].ToString()!,
            reader["organizer_name"].ToString()!,
            reader["organizer_surname"].ToString()!,
            ((DateOnly)reader["birthday"]).ToDateTime(TimeOnly.MinValue),
            UserMapper.ParseRole(reader["role"].ToString()!),
            reader.IsDBNull(reader.GetOrdinal("address")) ? null : reader["address"].ToString()
        );
    }

    public static User MapCitizen(IDataReader reader)
    {
        return new User(
            Convert.ToInt64(reader["citizen_id"]),
            reader["username"].ToString()!,
            reader["password"].ToString()!,
            reader["citizen_name"].ToString()!,
            reader["citizen_surname"].ToString()!,
            ((DateOnly)reader["birthday"]).ToDateTime(TimeOnly.MinValue),
            UserMapper.ParseRole(reader["role"].ToString()!),
            reader.IsDBNull(reader.GetOrdinal("address")) ? null : reader["address"].ToString()
        );
    }

    public static EventStatus ParseEventStatus(string status)
    {
        return status.ToLower() switch
        {
            "preparation" => EventStatus.Preparation,
            "scheduled" => EventStatus.Scheduled,
            "cancelled" => EventStatus.Cancelled,
            "finished" => EventStatus.Finished,
            _ => throw new ArgumentException($"Unknown event status: {status}")
        };
    }
}
