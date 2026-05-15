using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Buildings;
using CommunityHub.Application.DependencyInjection;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.ManagerViewModels.Buildings;

public class CommonRoomRequestViewModel : BaseViewModel
{
    private readonly CommonRoomRequestService _requestService;
    private readonly long _commonRoomId;
    private List<(DateTime, DateTime)> _rawAlternatives = new();

    public string RoomTitle { get; }

    private ObservableCollection<CommonRoomRequestRowViewModel> _requests = new();
    public ObservableCollection<CommonRoomRequestRowViewModel> Requests
    {
        get => _requests;
        private set => SetProperty(ref _requests, value);
    }

    private ObservableCollection<DateTime> _freeDays = new();
    public ObservableCollection<DateTime> FreeDays
    {
        get => _freeDays;
        private set => SetProperty(ref _freeDays, value);
    }

    private ObservableCollection<string> _alternativeRanges = new();
    public ObservableCollection<string> AlternativeRanges
    {
        get => _alternativeRanges;
        private set => SetProperty(ref _alternativeRanges, value);
    }

    public CommonRoomRequestViewModel(long commonRoomId, string roomName)
    {
        _commonRoomId = commonRoomId;
        RoomTitle = $"Requests for {roomName}";
        _requestService = Injector.CreateInstance<CommonRoomRequestService>();
        LoadRequests();
    }

    public void LoadRequests()
    {
        List<CommonRoomRequestDto> requests = _requestService.GetRequestsByCommonRoom(_commonRoomId);
        Requests = new ObservableCollection<CommonRoomRequestRowViewModel>(
            requests.Select(r => new CommonRoomRequestRowViewModel(r)).ToList()
        );
    }

    public void LoadFreeDays(long requestId)
    {
        CommonRoomRequest? request = _requestService.GetById(requestId);
        if (request == null) return;
        List<DateTime> freeDays = _requestService.GetFreeDaysInRange(request);
        FreeDays = new ObservableCollection<DateTime>(freeDays);
    }

    public void LoadAlternativeRanges(long requestId)
    {
        CommonRoomRequest? request = _requestService.GetById(requestId);
        if (request == null) return;
        _rawAlternatives = _requestService.FindAlternativeRanges(request);
        AlternativeRanges = new ObservableCollection<string>(
            _rawAlternatives.Select(r => $"{r.Item1:dd.MM.yyyy} - {r.Item2:dd.MM.yyyy}").ToList()
        );
    }

    public void ApproveWithDate(long requestId, DateTime selectedDate)
    {
        CommonRoomRequest? request = _requestService.GetById(requestId);
        if (request == null) return;
        _requestService.ApproveWithDate(request, selectedDate);
        LoadRequests();
    }

    public void ProposeAlternative(long requestId, int alternativeIndex)
    {
        CommonRoomRequest? request = _requestService.GetById(requestId);
        if (request == null) return;
        var (newFrom, newTo) = _rawAlternatives[alternativeIndex];
        request.ProposeNewDateRange(newFrom, newTo);
        _requestService.UpdateRequest(request);
        LoadRequests();
    }

    public void RejectRequest(long requestId)
    {
        CommonRoomRequest? request = _requestService.GetById(requestId);
        if (request == null) return;
        _requestService.RejectRequest(request);
        LoadRequests();
    }
}