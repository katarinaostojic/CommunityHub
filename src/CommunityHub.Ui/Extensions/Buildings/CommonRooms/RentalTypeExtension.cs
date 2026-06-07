using CommunityHub.Application.Domain.Entities.Buildings.CommonRooms;

namespace CommunityHub.Ui.Extensions.Buildings.CommonRooms;

public static class RentalTypeExtensions
{
    public static string ToDisplayString(this RentalType type) => type switch
    {
        RentalType.PerDay => "Per day",
        RentalType.MultiDay => "Multi day",
        _ => type.ToString()
    };
}