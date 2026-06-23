using CommunityHub.Application.Domain.Entities.Neighborhoods;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using CommunityHub.Ui.Helpers.Citizen;

namespace CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

public class NeighborhoodAccessRequestDialogViewModel
{
    private readonly NeighborhoodAccessRequestService _service;
    private readonly long _citizenId;
    private readonly NeighborhoodDto _neighborhood;

    public NeighborhoodAccessRequestDialogViewModel(
        NeighborhoodAccessRequestService service,
        long citizenId,
        NeighborhoodDto neighborhood)
    {
        _service = service;
        _citizenId = citizenId;
        _neighborhood = neighborhood;
    }

    public string NeighborhoodName => _neighborhood.Name;
    public string Description => _neighborhood.Description;
    public string ImagePath => _neighborhood.ImagePaths.FirstOrDefault() ?? string.Empty;

    public string Location =>
        $"{ResourceHelper.Get("Request_Location", "Location:")} {_neighborhood.Location}";

    public string AddressText
    {
        get
        {
            string label = ResourceHelper.Get("Request_Address", "Address:");
            if (_neighborhood.Streets == null || !_neighborhood.Streets.Any())
                return $"{label} {ResourceHelper.Get("Request_NoStreets", "No street information available")}";
            return $"{label} " + string.Join(", ", _neighborhood.Streets.Select(s => s.Display));
        }
    }

    public AccessRequestResult RequestAccess()
        => _service.RequestAccessById(_citizenId, _neighborhood.Id);
}
