using CommunityHub.Application.Domain.Entities.Buildings.CommonRooms;
using CommunityHub.Application.Domain.Entities.Buildings.CommonRooms;
using CommunityHub.Ui.Extensions;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Buildings;

public class CommonRoomRequestDialogViewModel : BaseViewModel
{
    private readonly RentalType _rentalType;
    private string _dateError = string.Empty;
    private bool _hasDateError;

    public CommonRoomRequestDialogViewModel(
        string buildingInfo,
        string roomName,
        string floorDisplay,
        RentalType rentalType)
    {
        BuildingInfo = buildingInfo;
        RoomName = roomName;
        FloorDisplay = floorDisplay;
        _rentalType = rentalType;
    }

    public string BuildingInfo { get; }
    public string RoomName { get; }
    public string FloorDisplay { get; }
    public string RentalTypeDisplay => _rentalType.ToDisplayString();

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

    public string WhatHappensNextText => _rentalType == RentalType.PerDay
        ? "Your request will be sent to the building administrator. The administrator will select an available day within your requested date range."
        : "Your request will be sent to the building administrator. If your requested dates are available, the request will be automatically approved. If not, the administrator may suggest alternative dates.";

    public bool Validate(DateTime? dateFrom, DateTime? dateTo)
    {
        if (dateFrom == null || dateTo == null)
        {
            ShowDateError("Please select both start and end date.");
            return false;
        }

        string? error = CommonRoomRequest.ValidateDateRange(dateFrom.Value, dateTo.Value);

        if (error != null)
        {
            ShowDateError(error);
            return false;
        }

        ClearDateError();
        return true;
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