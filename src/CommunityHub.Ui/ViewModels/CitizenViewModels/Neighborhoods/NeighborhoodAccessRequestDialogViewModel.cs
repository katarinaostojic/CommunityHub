using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Neighborhoods;
using CommunityHub.Application.Services.Neighborhoods;

namespace CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

public class NeighborhoodAccessRequestDialogViewModel
{
    private readonly NeighborhoodAccessRequestService _service;
    private readonly User _user;
    private readonly Neighborhood _neighborhood;

    public NeighborhoodAccessRequestDialogViewModel(
        NeighborhoodAccessRequestService service,
        User user,
        Neighborhood neighborhood)
    {
        _service = service;
        _user = user;
        _neighborhood = neighborhood;
    }

    public string NeighborhoodName => _neighborhood.Name;
    public string Description => _neighborhood.Description;
    public string Location => $"Location: {_neighborhood.Location.CityName}, {_neighborhood.Location.CountryName}";

    public string ImagePath
    {
        get
        {
            var firstImage = _neighborhood.Images?.FirstOrDefault();
            return firstImage?.Path ?? string.Empty;
        }
    }

    public string AddressText
    {
        get
        {
            if (_neighborhood.Streets == null || !_neighborhood.Streets.Any())
                return "Address: No street information available";
            return "Address: " + string.Join(", ",
                _neighborhood.Streets.Select(s => $"{s.StreetName} {s.StartNumber} - {s.EndNumber}"));
        }
    }

    public AccessRequestResult RequestAccess() => _service.RequestAccess(_user, _neighborhood);
}