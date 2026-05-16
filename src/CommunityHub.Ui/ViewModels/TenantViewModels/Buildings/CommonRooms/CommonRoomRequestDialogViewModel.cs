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
        if (dateFrom == null || dateTo == null)
        {
            HasDateError = true;
            DateError = "Please select both start and end date.";
            return false;
        }

        if (dateTo.Value.Date < dateFrom.Value.Date)
        {
            HasDateError = true;
            DateError = "End date must be after start date.";
            return false;
        }

        HasDateError = false;
        DateError = string.Empty;
        return true;
    }
}