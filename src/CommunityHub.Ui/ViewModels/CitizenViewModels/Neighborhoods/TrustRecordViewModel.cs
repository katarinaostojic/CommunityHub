using System;
using System.Collections.Generic;
using System.Text;

using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Domain;

namespace CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

public class TrustRecordViewModel : BaseViewModel
{
    private readonly TrustRecordDto _dto;

    public TrustRecordViewModel(TrustRecordDto dto)
    {
        _dto = dto;
    }

    public long CitizenId => _dto.CitizenId;
    public string CitizenFullName => _dto.CitizenFullName;
    public int EventsOrganized => _dto.EventsOrganized;
    public int EventsVolunteered => _dto.EventsVolunteered;
    public int TotalEvents => _dto.TotalEvents;
    public string JoinedAt => _dto.JoinedAt;
    public string LevelDisplay => _dto.LevelDisplay;

    public string LevelColor => _dto.Level switch
    {
        TrustLevel.New => "#888888",
        TrustLevel.Inactive => "#E74C3C",
        TrustLevel.Active => "#27AE60",
        TrustLevel.Distinguished => "#F39C12",
        TrustLevel.Trusted => "#8E44AD",
        _ => "#888888"
    };
}