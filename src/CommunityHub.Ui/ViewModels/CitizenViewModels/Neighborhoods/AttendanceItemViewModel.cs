using System;
using System.Collections.Generic;
using System.Text;
using CommunityHub.Application.DTOs.Neighborhoods.Events;
using CommunityHub.Ui.Helpers.Citizen;

namespace CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

public class AttendanceItemViewModel : BaseViewModel
{
    private readonly EventRegistrationDto _registration;

    public AttendanceItemViewModel(EventRegistrationDto registration)
    {
        _registration = registration;
        LanguageManager.LanguageChanged += () => OnPropertyChanged(nameof(AttendanceDisplay));
    }

    public long RegistrationId => _registration.Id;
    public string CitizenFullName => _registration.CitizenFullName;
    public string AttendanceDisplay =>
        _registration.Attended == null
            ? ResourceHelper.Get("Attendance_NotMarked", "Not marked")
            : _registration.Attended.Value
                ? ResourceHelper.Get("Attendance_Attended", "Attended")
                : ResourceHelper.Get("Attendance_NotAttended", "Did not attend");
    public string AttendanceColor => _registration.Attended == true ? "#1A5C2A"
    : _registration.Attended == false ? "#B54A4A"
    : "#888888";
}
