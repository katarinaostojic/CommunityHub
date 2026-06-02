using CommunityHub.Application.Domain.Entities.Buildings.ProblemReports;

namespace CommunityHub.Application.Database.Mappers.Buildings.ProblemReports;

public static class ProblemPriorityMapper
{
    public static ProblemPriority FromDatabaseValue(string value) => value switch
    {
        "can wait" => ProblemPriority.CanWait,
        "soon" => ProblemPriority.Soon,
        "urgent" => ProblemPriority.Urgent,
        _ => throw new ArgumentException($"Unknown problem priority: '{value}'")
    };

    public static string ToDatabaseValue(ProblemPriority priority) => priority switch
    {
        ProblemPriority.CanWait => "can wait",
        ProblemPriority.Soon => "soon",
        ProblemPriority.Urgent => "urgent",
        _ => throw new ArgumentException($"Unknown problem priority: '{priority}'")
    };
}