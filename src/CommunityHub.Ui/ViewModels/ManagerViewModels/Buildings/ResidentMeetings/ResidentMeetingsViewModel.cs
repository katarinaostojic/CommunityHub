using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Buildings.ResidentMeetings;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.DTOs.Buildings.ResidentMeetings;
using CommunityHub.Application.Services.Entities.Buildings;
using CommunityHub.Application.Services.Entities.Buildings.ResidentMeetings;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.ManagerViewModels.Buildings.ResidentMeetings;

public class ResidentMeetingsViewModel : BaseViewModel
{
    private readonly ResidentMeetingService _meetingService;
    private readonly BuildingService _buildingService;
    private readonly long _managerId;

    private ObservableCollection<BuildingDto> _buildings = new();
    public ObservableCollection<BuildingDto> Buildings
    {
        get => _buildings;
        private set => SetProperty(ref _buildings, value);
    }

    private BuildingDto? _selectedBuilding;
    public BuildingDto? SelectedBuilding
    {
        get => _selectedBuilding;
        set
        {
            SetProperty(ref _selectedBuilding, value);
            LoadMeetings();
        }
    }

    private ObservableCollection<ResidentMeetingRowViewModel> _meetings = new();
    public ObservableCollection<ResidentMeetingRowViewModel> Meetings
    {
        get => _meetings;
        private set => SetProperty(ref _meetings, value);
    }

    private ResidentMeetingStatus? _currentFilter = null;

    public string AllButtonLabel => $"All ({CountByBuilding(null)})";
    public string ScheduledButtonLabel => $"Scheduled ({CountByBuilding(ResidentMeetingStatus.Scheduled)})";
    public string ConfirmedButtonLabel => $"Confirmed ({CountByBuilding(ResidentMeetingStatus.Confirmed)})";
    public string CancelledButtonLabel => $"Cancelled ({CountByBuilding(ResidentMeetingStatus.Cancelled)})";

    public ResidentMeetingsViewModel(long managerId)
    {
        _managerId = managerId;
        _meetingService = Injector.CreateInstance<ResidentMeetingService>();
        _buildingService = Injector.CreateInstance<BuildingService>();
        LoadBuildings();
    }

    public void LoadBuildings()
    {
        var buildings = _buildingService.GetAllByManager(_managerId);
        Buildings = new ObservableCollection<BuildingDto>(buildings);
    }

    public void LoadMeetings()
    {
        if (_selectedBuilding == null)
        {
            Meetings = new ObservableCollection<ResidentMeetingRowViewModel>();
            return;
        }

        var meetings = _meetingService
            .GetAllByBuilding(_selectedBuilding.Id, _currentFilter)
            .Select(m => new ResidentMeetingRowViewModel(m))
            .ToList();

        Meetings = new ObservableCollection<ResidentMeetingRowViewModel>(meetings);
        RefreshButtonLabels();
    }

    public void FilterAll()
    {
        _currentFilter = null;
        LoadMeetings();
    }

    public void FilterScheduled()
    {
        _currentFilter = ResidentMeetingStatus.Scheduled;
        LoadMeetings();
    }

    public void FilterConfirmed()
    {
        _currentFilter = ResidentMeetingStatus.Confirmed;
        LoadMeetings();
    }

    public void FilterCancelled()
    {
        _currentFilter = ResidentMeetingStatus.Cancelled;
        LoadMeetings();
    }

    public void ScheduleMeeting(DateTime date, TimeSpan time, List<string> topics)
    {
        if (_selectedBuilding == null) return;
        _meetingService.CreateMeeting(_selectedBuilding.Id, date, time, topics);
        LoadMeetings();
    }

    public List<ResidentMeetingTopicSuggestionDto> GetTopicSuggestions(long meetingId)
    {
        return _meetingService.GetTopicSuggestions(meetingId);
    }

    public void AddTopicFromSuggestion(long meetingId, string topic)
    {
        _meetingService.AddTopicFromSuggestion(meetingId, topic);
        LoadMeetings();
    }

    public List<ResidentMeetingAttendance> GetAttendances(long meetingId)
    {
        return _meetingService.GetAttendances(meetingId);
    }

    private int CountByBuilding(ResidentMeetingStatus? status)
    {
        if (_selectedBuilding == null) return 0;
        return _meetingService.CountByBuilding(_selectedBuilding.Id, status);
    }

    private void RefreshButtonLabels()
    {
        OnPropertyChanged(nameof(AllButtonLabel));
        OnPropertyChanged(nameof(ScheduledButtonLabel));
        OnPropertyChanged(nameof(ConfirmedButtonLabel));
        OnPropertyChanged(nameof(CancelledButtonLabel));
    }
}