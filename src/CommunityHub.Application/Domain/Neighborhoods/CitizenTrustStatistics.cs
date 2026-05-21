using CommunityHub.Application.Domain.Neighborhoods;

namespace CommunityHub.Application.Domain.Neighborhoods;

public class CitizenTrustStatistics
{
    private readonly Dictionary<TrustLevel, int> _counts;

    public CitizenTrustStatistics(Dictionary<TrustLevel, int> counts)
    {
        _counts = counts;
    }

    public int NewCount => _counts.GetValueOrDefault(TrustLevel.New, 0);
    public int InactiveCount => _counts.GetValueOrDefault(TrustLevel.Inactive, 0);
    public int ActiveCount => _counts.GetValueOrDefault(TrustLevel.Active, 0);
    public int DistinguishedCount => _counts.GetValueOrDefault(TrustLevel.Distinguished, 0);
    public int TrustedCount => _counts.GetValueOrDefault(TrustLevel.Trusted, 0);

    public MeetingTheme? SuggestTheme()
    {
        if (NewCount == 0 && InactiveCount == 0)
            return null;

        if (NewCount >= InactiveCount)
            return MeetingTheme.Welcome;

        return MeetingTheme.Motivation;
    }
}