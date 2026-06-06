using CommunityHub.Application.Domain.Entities.Buildings.Ads;

namespace CommunityHub.Ui.Extensions.Buildings.Ads;

public static class AdTypeExtensions
{
    public static string ToDisplayString(this AdType type) => type switch
    {
        AdType.Offering => "↑ Offering",
        AdType.Seeking => "↓ Seeking",
        _ => type.ToString()
    };
}