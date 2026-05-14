using CommunityHub.Application.Domain;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Buildings;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Buildings;

public class BuildingAccessRequestDialogViewModel : BaseViewModel
{
    private readonly BuildingAccessRequestService _requestService;
    private readonly BuildingService _buildingService;
    private readonly BuildingDto _building;
    private readonly User _user;

    private string _unitNumber = string.Empty;
    private string _warningMessage = string.Empty;
    private bool _hasWarning;

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

        Title = $"REQUEST ACCESS: {building.FullAddress}";
        BuildingInfo = $"Building: {building.FullAddress}, {building.CityName}, {building.Neighborhood}";
        SortedUnitNumbers = _buildingService.GetSortedUnitNumbers(_building.Id);
    }

    public string Title { get; }

    public string BuildingInfo { get; }

    public List<string> SortedUnitNumbers { get; }

    public string UnitNumber
    {
        get => _unitNumber;
        set
        {
            if (SetProperty(ref _unitNumber, value))
                UpdateWarning();
        }
    }

    public string WarningMessage
    {
        get => _warningMessage;
        private set => SetProperty(ref _warningMessage, value);
    }

    public bool HasWarning
    {
        get => _hasWarning;
        private set => SetProperty(ref _hasWarning, value);
    }

    public string? SubmitRequest()
    {
        string unitNumber = UnitNumber.Trim();
        string? validationError = ValidateUnitNumber(unitNumber);

        if (validationError != null)
            return validationError;

        _requestService.CreateForBuilding(_building.Id, _user, unitNumber);
        return null;
    }

    private string? ValidateUnitNumber(string unitNumber)
    {
        if (string.IsNullOrEmpty(unitNumber))
            return "Please enter an apartment number.";

        if (!_buildingService.ContainsUnit(_building.Id, unitNumber))
            return "Please select a valid apartment number from the list.";

        if (_buildingService.HasExistingRequest(_building.Id, _user.Id, unitNumber))
            return "You already have a request for this apartment.";

        return null;
    }

    private void UpdateWarning()
    {
        string unitNumber = UnitNumber.Trim();

        if (string.IsNullOrEmpty(unitNumber))
        {
            HasWarning = false;
            WarningMessage = string.Empty;
            return;
        }

        bool isOccupied = _buildingService.IsUnitOccupied(_building.Id, unitNumber);

        HasWarning = isOccupied;
        WarningMessage = isOccupied
            ? $"Warning: Apartment {unitNumber} is already occupied by another user.\nYou can still submit a request."
            : string.Empty;
    }
}