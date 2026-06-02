using CommunityHub.Application.Domain.Entities.Buildings.ProblemReports;

namespace CommunityHub.Ui.Extensions;

public static class ProblemPriorityExtensions
{
    public static string ToDisplayString(this ProblemPriority priority) => priority switch
    {
        ProblemPriority.CanWait => "Can wait",
        ProblemPriority.Soon => "Should be solved soon",
        ProblemPriority.Urgent => "Urgent",
        _ => priority.ToString()
    };
}