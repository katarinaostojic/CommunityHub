using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Entities.Buildings;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Dialogs.Buildings;

public class BuildingAccessRequestDialogValidator
{
    private readonly BuildingService _buildingService;
    private readonly BuildingDto _building;
    private readonly User _user;

    public BuildingAccessRequestDialogValidator(
        BuildingService buildingService,
        BuildingDto building,
        User user)
    {
        _buildingService = buildingService;
        _building = building;
        _user = user;
    }

    public string? ValidateUnitNumber(string unitNumber)
    {
        if (string.IsNullOrEmpty(unitNumber))
            return "Please enter an apartment number.";

        if (!_buildingService.ContainsUnit(_building.Id, unitNumber))
            return "Please select a valid apartment number from the list.";

        if (_buildingService.HasExistingRequest(_building.Id, _user.Id, unitNumber))
            return "You already have a request for this apartment.";

        return null;
    }

    public bool HasOccupiedUnitWarning(string unitNumber)
    {
        if (string.IsNullOrEmpty(unitNumber))
            return false;

        return _buildingService.IsUnitOccupied(_building.Id, unitNumber);
    }

    public string GetWarningMessage(string unitNumber, bool hasWarning)
    {
        if (!hasWarning)
            return string.Empty;

        return $"Warning: Apartment {unitNumber} is already occupied by another user.\nYou can still submit a request.";
    }

    public string GetDemoUnitNumber(List<string> sortedUnitNumbers)
    {
        string? availableUnitNumber = sortedUnitNumbers
            .FirstOrDefault(unitNumber => !_buildingService.HasExistingRequest(_building.Id, _user.Id, unitNumber));

        return availableUnitNumber ?? sortedUnitNumbers.FirstOrDefault() ?? "1";
    }
}