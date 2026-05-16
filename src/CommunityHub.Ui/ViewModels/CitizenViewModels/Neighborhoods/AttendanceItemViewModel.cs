using System;
using System.Collections.Generic;
using System.Text;
using CommunityHub.Application.DTOs.Neighborhoods;

namespace CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

public class AttendanceItemViewModel
{
    private readonly EventRegistrationDto _registration;

    public AttendanceItemViewModel(EventRegistrationDto registration)
    {
        _registration = registration;
    }

    public long RegistrationId => _registration.Id;
    public string CitizenFullName => _registration.CitizenFullName;
    public string AttendanceDisplay => _registration.AttendanceDisplay;
}
