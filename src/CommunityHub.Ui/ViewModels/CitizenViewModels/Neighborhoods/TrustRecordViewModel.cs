using CommunityHub.Application.Domain.Entities;
using CommunityHub.Application.DTOs.Neighborhoods.Meetings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

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

    public string LevelDisplay => GetLocalizedLevel();

    private string GetLocalizedLevel()
    {
        string key = _dto.Level switch
        {
            TrustLevel.New => "TrustLevel_New",
            TrustLevel.Inactive => "TrustLevel_Inactive",
            TrustLevel.Active => "TrustLevel_Active",
            TrustLevel.Distinguished => "TrustLevel_Distinguished",
            TrustLevel.Trusted => "TrustLevel_Trusted",
            _ => "TrustLevel_New"
        };
        return System.Windows.Application.Current.Resources[key]?.ToString() ?? _dto.LevelDisplay;
    }

    public string LevelColor => _dto.Level switch
    {
        TrustLevel.New => "#888888",
        TrustLevel.Inactive => "#E74C3C",
        TrustLevel.Active => "#27AE60",
        TrustLevel.Distinguished => "#F39C12",
        TrustLevel.Trusted => "#8E44AD",
        _ => "#888888"
    };

    public SolidColorBrush LevelBrush => new SolidColorBrush(
        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(LevelColor));
}