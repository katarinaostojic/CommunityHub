using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.Services.Buildings;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Buildings;

public class BuildingAccessRequestDialogViewModel
{
    private readonly BuildingAccessRequestService _requestService;
    private readonly Building _building;
    private readonly User _user;

    public BuildingAccessRequestDialogViewModel(BuildingAccessRequestService requestService, Building building, User user)
    {
        _requestService = requestService;
        _building = building;
        _user = user;
    }

    public List<string> SortedUnitNumbers => _building.GetSortedUnitNumbers();

    public bool IsUnitOccupied(string unitNumber) => _building.IsUnitOccupied(unitNumber);

    public bool ContainsUnit(string unitNumber) => _building.ContainsUnit(unitNumber);

    public bool HasExistingRequest(string unitNumber) => _building.HasExistingRequest(_user.Id, unitNumber);

    public void SubmitRequest(string unitNumber)
    {
        _requestService.Create(_user, _building, unitNumber);
    }
}