using System.Windows;

namespace CommunityHub.Ui.Helpers.Tenant.Demo.Buildings;

public sealed record BrowseBuildingsDemoState(
    string SearchText,
    string StreetText,
    string NeighborhoodText,
    string CityText,
    string CountryText,
    Visibility SuccessBannerVisibility,
    string SuccessText,
    Visibility ViewRequestsButtonVisibility);