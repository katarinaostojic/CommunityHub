namespace CommunityHub.Ui.ViewModels.TenantViewModels.Buildings;

public class CommonRoomRequestDialogViewModel : BaseViewModel
{
    private string _dateError = string.Empty;
    private bool _hasDateError;

    public CommonRoomRequestDialogViewModel(
        string buildingInfo,
        string roomName,
        string floorDisplay,
        string rentalTypeDisplay)
    {
        BuildingInfo = buildingInfo;
        RoomName = roomName;
        FloorDisplay = floorDisplay;
        RentalTypeDisplay = rentalTypeDisplay;
    }

    public string BuildingInfo { get; }
    public string RoomName { get; }
    public string FloorDisplay { get; }
    public string RentalTypeDisplay { get; }

    public string DateError
    {
        get => _dateError;
        private set => SetProperty(ref _dateError, value);
    }

    public bool HasDateError
    {
        get => _hasDateError;
        private set => SetProperty(ref _hasDateError, value);
    }

    public string WhatHappensNextText => RentalTypeDisplay == "1 day only"
        ? "Your request will be sent to the building administrator. The administrator will select an available day within your requested date range."
        : "Your request will be sent to the building administrator. If your requested dates are available, the request will be automatically approved. If not, the administrator may suggest alternative dates.";

    public bool Validate(DateTime? dateFrom, DateTime? dateTo)
    {
        string? error = GetValidationError(dateFrom, dateTo);

        if (error != null)
        {
            ShowDateError(error);
            return false;
        }

        ClearDateError();
        return true;
    }

    private static string? GetValidationError(DateTime? dateFrom, DateTime? dateTo)
    {
        if (DatesAreMissing(dateFrom, dateTo))
            return "Please select both start and end date.";

        DateTime startDate = dateFrom!.Value.Date;
        DateTime endDate = dateTo!.Value.Date;

        if (DateIsInPast(startDate))
            return "Start date cannot be in the past.";

        if (DateIsInPast(endDate))
            return "End date cannot be in the past.";

        if (EndDateIsBeforeStartDate(startDate, endDate))
            return "End date must be after start date.";

        return null;
    }

    private static bool DatesAreMissing(DateTime? dateFrom, DateTime? dateTo)
    {
        return dateFrom == null || dateTo == null;
    }

    private static bool DateIsInPast(DateTime date)
    {
        return date < DateTime.Today;
    }

    private static bool EndDateIsBeforeStartDate(DateTime startDate, DateTime endDate)
    {
        return endDate < startDate;
    }

    private void ShowDateError(string error)
    {
        HasDateError = true;
        DateError = error;
    }

    private void ClearDateError()
    {
        HasDateError = false;
        DateError = string.Empty;
    }
}