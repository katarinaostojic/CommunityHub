using CommunityHub.Application.Domain;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Services.Neighborhoods;

namespace CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

public class NeighborhoodAccessRequestDialogViewModel
{
    private readonly NeighborhoodAccessRequestService _service;
    private readonly User _user;
    private readonly NeighborhoodDto _neighborhood;

    public NeighborhoodAccessRequestDialogViewModel(
        NeighborhoodAccessRequestService service,
        User user,
        NeighborhoodDto neighborhood)
    {
        _service = service;
        _user = user;
        _neighborhood = neighborhood;
    }

    public string NeighborhoodName => _neighborhood.Name;
    public string Description => _neighborhood.Description;
    public string Location => $"Location: {_neighborhood.Location}";
    public string ImagePath => _neighborhood.ImagePaths.FirstOrDefault() ?? string.Empty;

    public string AddressText
    {
        get
        {
            if (_neighborhood.Streets == null || !_neighborhood.Streets.Any())
                return "Address: No street information available";
            return "Address: " + string.Join(", ",
                _neighborhood.Streets.Select(s => s.Display));
        }
    }

    public AccessRequestResult RequestAccess()
        => _service.RequestAccessById(_user, _neighborhood.Id);
}