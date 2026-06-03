namespace CommunityHub.Application.Domain.Entities.Neighborhoods;

public class DateOnlyRange
{
    public DateOnly From { get; }
    public DateOnly To { get; }

    public DateOnlyRange(DateOnly from, DateOnly to)
    {
        From = from;
        To = to;
    }

    public string Display => $"{From:dd.MM.yyyy.} – {To:dd.MM.yyyy.}";
}
