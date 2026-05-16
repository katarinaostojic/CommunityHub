using System;
using System.Collections.Generic;
using System.Text;

using CommunityHub.Application.Domain;
using CommunityHub.Application.Services;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

public class MeetingsViewModel : BaseViewModel
{
    private readonly MeetingService _service;
    private readonly long _neighborhoodId;
    private readonly long _citizenId;

    private ObservableCollection<MeetingViewModel> _meetings = new();
    private string _resultsText = string.Empty;

    public MeetingsViewModel(MeetingService service, long neighborhoodId, long citizenId)
    {
        _service = service;
        _neighborhoodId = neighborhoodId;
        _citizenId = citizenId;
        LoadMeetings();
    }

    public ObservableCollection<MeetingViewModel> Meetings
    {
        get => _meetings;
        private set => SetProperty(ref _meetings, value);
    }

    public string ResultsText
    {
        get => _resultsText;
        private set => SetProperty(ref _resultsText, value);
    }

    public void LoadMeetings()
    {
        var items = _service.GetByNeighborhoodForCitizen(_neighborhoodId, _citizenId)
            .Select(dto => new MeetingViewModel(dto))
            .ToList();

        Meetings = new ObservableCollection<MeetingViewModel>(items);
        ResultsText = $"Showing {items.Count} meetings";
    }

    public void Vote(long meetingId, DateOnly votedDate)
    {
        _service.AddVote(meetingId, _citizenId, votedDate);
        LoadMeetings();
    }

    public void ChangeVote(long voteId, DateOnly newDate)
    {
        _service.UpdateVote(voteId, newDate);
        LoadMeetings();
    }
}