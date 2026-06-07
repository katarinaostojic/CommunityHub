using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using CommunityHub.Application.DTOs.Neighborhoods.CityObjects;

namespace CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

public class CityObjectItemViewModel : BaseViewModel
{
    private readonly CitizenCityObjectDto _dto;
    private bool _hasVoted;

    public CityObjectItemViewModel(CitizenCityObjectDto dto)
    {
        _dto = dto;
        _hasVoted = dto.HasVoted;
    }

    public long Id => _dto.Id;
    public string Name => _dto.Name;
    public string Description => _dto.Description;
    public int VoteCount => _dto.VoteCount;

    public string LastVisit => _dto.LastVisit
        ?? System.Windows.Application.Current.Resources["CityObjects_NoVisit"]?.ToString()
        ?? "No visits yet";

    public bool HasVoted
    {
        get => _hasVoted;
        set
        {
            _hasVoted = value;
            OnPropertyChanged(nameof(HasVoted));
            OnPropertyChanged(nameof(VoteButtonText));
        }
    }

    public string VoteButtonText => _hasVoted
        ? System.Windows.Application.Current.Resources["CityObjects_Unvote"]?.ToString() ?? "Remove vote"
        : System.Windows.Application.Current.Resources["CityObjects_Vote"]?.ToString() ?? "Vote";
}