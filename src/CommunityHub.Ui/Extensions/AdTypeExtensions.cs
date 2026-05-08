using CommunityHub.Application.Domain.Ads;

namespace CommunityHub.Ui.Extensions;

public static class AdTypeExtensions
{
    public static string ToDisplayString(this AdType type) => type switch
    {
        AdType.Offering => "↑ Offering",
        AdType.Seeking => "↓ Seeking",
        _ => type.ToString()
    };
}