using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Entities.Buildings;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Dialogs.Buildings;

public class BuildingAccessRequestDialogViewModel : BaseViewModel
{
    private readonly BuildingAccessRequestService _requestService;
    private readonly BuildingAccessRequestDialogValidator _validator;
    private readonly BuildingDto _building;
    private readonly User _user;

    private string _unitNumber = string.Empty;
    private string _warningMessage = string.Empty;
    private string _validationMessage = string.Empty;
    private bool _hasWarning;
    private bool _hasValidationError;

    public BuildingAccessRequestDialogViewModel(
        BuildingAccessRequestService requestService,
        BuildingService buildingService,
        BuildingDto building,
        User user)
    {
        _requestService = requestService;
        _building = building;
        _user = user;
        _validator = new BuildingAccessRequestDialogValidator(buildingService, building, user);

        Title = $"REQUEST ACCESS: {building.FullAddress}";
        BuildingInfo = $"Building: {building.FullAddress}, {building.CityName}, {building.Neighborhood}";
        SortedUnitNumbers = buildingService.GetSortedUnitNumbers(_building.Id);
    }

    public string Title { get; }

    public string BuildingInfo { get; }

    public List<string> SortedUnitNumbers { get; }

    public string UnitNumber
    {
        get => _unitNumber;
        set
        {
            if (!SetProperty(ref _unitNumber, value))
                return;

            ClearValidationError();
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

    public string ValidationMessage
    {
        get => _validationMessage;
        private set => SetProperty(ref _validationMessage, value);
    }

    public bool HasValidationError
    {
        get => _hasValidationError;
        private set => SetProperty(ref _hasValidationError, value);
    }

    public bool SubmitRequest()
    {
        string unitNumber = UnitNumber.Trim();

        if (!CanSubmit(unitNumber))
            return false;

        _requestService.CreateForBuilding(_building.Id, _user, unitNumber);
        return true;
    }

    public string GetDemoUnitNumber()
    {
        return _validator.GetDemoUnitNumber(SortedUnitNumbers);
    }

    public void SetUnitNumberForDemo(string unitNumber)
    {
        UnitNumber = unitNumber;
    }

    public bool CanSubmitRequestForDemo()
    {
        return CanSubmit(UnitNumber.Trim());
    }

    private bool CanSubmit(string unitNumber)
    {
        string? validationError = _validator.ValidateUnitNumber(unitNumber);

        if (validationError == null)
            return true;

        ShowValidationError(validationError);
        return false;
    }

    private void ShowValidationError(string message)
    {
        ValidationMessage = message;
        HasValidationError = true;
    }

    private void ClearValidationError()
    {
        ValidationMessage = string.Empty;
        HasValidationError = false;
    }

    private void UpdateWarning()
    {
        string unitNumber = UnitNumber.Trim();
        bool hasWarning = _validator.HasOccupiedUnitWarning(unitNumber);

        HasWarning = hasWarning;
        WarningMessage = _validator.GetWarningMessage(unitNumber, hasWarning);
    }
}