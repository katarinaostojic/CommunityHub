using CommunityHub.Application.Domain;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Buildings;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Buildings;

public class BuildingAccessRequestDialogViewModel
{
    private readonly BuildingAccessRequestService _requestService;
    private readonly BuildingService _buildingService;
    private readonly BuildingDto _building;
    private readonly User _user;

    public BuildingAccessRequestDialogViewModel(
        BuildingAccessRequestService requestService,
        BuildingService buildingService,
        BuildingDto building,
        User user)
    {
        _requestService = requestService;
        _buildingService = buildingService;
        _building = building;
        _user = user;
    }

    public List<string> SortedUnitNumbers => _buildingService.GetSortedUnitNumbers(_building.Id);

    public bool IsUnitOccupied(string unitNumber) =>
        _buildingService.IsUnitOccupied(_building.Id, unitNumber);

    public bool ContainsUnit(string unitNumber) =>
        _buildingService.ContainsUnit(_building.Id, unitNumber);

    public bool HasExistingRequest(string unitNumber) =>
        _buildingService.HasExistingRequest(_building.Id, _user.Id, unitNumber);

    public void SubmitRequest(string unitNumber)
    {
        _requestService.CreateForBuilding(_building.Id, _user, unitNumber);
    }
}
