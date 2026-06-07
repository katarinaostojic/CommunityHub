using System;
using System.Collections.Generic;
using System.Text;
namespace CommunityHub.Application.Domain.Entities.Neighborhoods.Events;

public static class EventStatusChecker
{
    public static bool IsDeadlinePassed(DateTime eventDateTime, DateTime now)
        => now >= eventDateTime.AddHours(-12);

    public static bool IsEventOver(DateTime eventDateTime, int durationMinutes, DateTime now)
        => now >= eventDateTime.Add(TimeSpan.FromMinutes(durationMinutes));
}
