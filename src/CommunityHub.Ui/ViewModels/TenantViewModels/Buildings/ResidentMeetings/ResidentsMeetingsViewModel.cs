using CommunityHub.Application.Domain.Entities.Buildings.ResidentMeetings;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.DTOs.Buildings.ResidentMeetings;
using CommunityHub.Application.Services.Entities.Buildings.ResidentMeetings;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.ResidentMeetings;

public class ResidentsMeetingsViewModel : BaseViewModel
{
    private readonly ResidentMeetingService _meetingService;
    private readonly long _tenantId;
    private readonly long _buildingId;

    private ObservableCollection<ResidentMeetingCardViewModel> _meetings = new();
    private ResidentMeetingStatus? _currentFilter = null;
    private string _resultsCountText = string.Empty;
    private int _allCount;
    private int _scheduledCount;
    private int _confirmedCount;
    private int _cancelledCount;

    public ResidentsMeetingsViewModel(
        ResidentMeetingService meetingService,
        long tenantId,
        long buildingId,
        List<BuildingMembershipDto> buildingMemberships,
        BuildingMembershipDto selectedMembership)
    {
        _meetingService = meetingService;
        _tenantId = tenantId;
        _buildingId = buildingId;

        ApartmentSelector = new ResidentMeetingApartmentSelector(
            buildingMemberships,
            selectedMembership);

        ApartmentSelector.SelectedApartmentChanged += Refresh;

        Refresh();
    }

    public ResidentMeetingApartmentSelector ApartmentSelector { get; }

    public ObservableCollection<ResidentMeetingCardViewModel> Meetings
    {
        get => _meetings;
        private set => SetProperty(ref _meetings, value);
    }

    public string ResultsCountText
    {
        get => _resultsCountText;
        private set => SetProperty(ref _resultsCountText, value);
    }

    public string AllButtonLabel => $"All ({_allCount})";
    public string ScheduledButtonLabel => $"Scheduled ({_scheduledCount})";
    public string ConfirmedButtonLabel => $"Confirmed ({_confirmedCount})";
    public string CancelledButtonLabel => $"Cancelled ({_cancelledCount})";

    public void FilterAll() => ApplyFilter(null);

    public void FilterScheduled() =>
        ApplyFilter(ResidentMeetingStatus.Scheduled);

    public void FilterConfirmed() =>
        ApplyFilter(ResidentMeetingStatus.Confirmed);

    public void FilterCancelled() =>
        ApplyFilter(ResidentMeetingStatus.Cancelled);

    public void Attend(long meetingId)
    {
        _meetingService.Attend(meetingId, _tenantId, ApartmentSelector.SelectedUnitNumber);
        Refresh();
    }

    public void CancelAttendance(long meetingId)
    {
        _meetingService.CancelAttendance(meetingId, _tenantId, ApartmentSelector.SelectedUnitNumber);
        Refresh();
    }

    public void SuggestTopic(long meetingId, string topic)
    {
        CreateResidentMeetingTopicSuggestionDto request = new(
            meetingId,
            _tenantId,
            topic);

        _meetingService.SuggestTopic(request);
        Refresh();
    }

    public void Refresh()
    {
        UpdateCounts();
        UpdateMeetings();
    }

    private void ApplyFilter(ResidentMeetingStatus? status)
    {
        _currentFilter = status;
        Refresh();
    }

    private void UpdateMeetings()
    {
        List<ResidentMeetingCardViewModel> meetings = _meetingService
            .GetByTenantAndBuilding(
                _tenantId,
                _buildingId,
                ApartmentSelector.SelectedUnitNumber,
                _currentFilter)
            .Select(m => new ResidentMeetingCardViewModel(
                m,
                ApartmentSelector.SelectedUnitNumber))
            .ToList();

        Meetings = new ObservableCollection<ResidentMeetingCardViewModel>(meetings);
        ResultsCountText = $"Showing {meetings.Count} residents' meeting{(meetings.Count == 1 ? "" : "s")}";
    }

    private void UpdateCounts()
    {
        _allCount = _meetingService.CountByTenantAndBuilding(_tenantId, _buildingId);

        _scheduledCount = _meetingService.CountByTenantAndBuilding(
            _tenantId,
            _buildingId,
            ResidentMeetingStatus.Scheduled);

        _confirmedCount = _meetingService.CountByTenantAndBuilding(
            _tenantId,
            _buildingId,
            ResidentMeetingStatus.Confirmed);

        _cancelledCount = _meetingService.CountByTenantAndBuilding(
            _tenantId,
            _buildingId,
            ResidentMeetingStatus.Cancelled);

        OnPropertyChanged(nameof(AllButtonLabel));
        OnPropertyChanged(nameof(ScheduledButtonLabel));
        OnPropertyChanged(nameof(ConfirmedButtonLabel));
        OnPropertyChanged(nameof(CancelledButtonLabel));
    }
}