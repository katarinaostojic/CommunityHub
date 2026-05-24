using CommunityHub.Application.DependencyInjection;
using System.Collections.ObjectModel;
using CommunityHub.Application.DTOs.Buildings.CommonRooms;
using CommunityHub.Application.Services.Entities.Buildings.CommonRooms;

namespace CommunityHub.Ui.ViewModels.ManagerViewModels.Buildings.CommonRooms;

public class CommonRoomRequestViewModel : BaseViewModel
{
    private readonly CommonRoomRequestService _requestService;
    private readonly long _commonRoomId;

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
        List<DateTime> freeDays = _requestService.GetFreeDaysInRange(requestId);
        FreeDays = new ObservableCollection<DateTime>(freeDays);
    }

    public void LoadAlternativeRanges(long requestId)
    {
        List<(DateTime, DateTime)> alternatives = _requestService.FindAlternativeRanges(requestId);
        AlternativeRanges = new ObservableCollection<string>(
            alternatives.Select(r => $"{r.Item1:dd.MM.yyyy} - {r.Item2:dd.MM.yyyy}").ToList()
        );
    }

    public void ApproveWithDate(long requestId, DateTime selectedDate)
    {
        _requestService.ApproveWithDate(requestId, selectedDate);
        LoadRequests();
    }

    public void ProposeAlternative(long requestId, int alternativeIndex)
    {
        _requestService.ProposeAlternative(requestId, alternativeIndex);
        LoadRequests();
    }

    public void RejectRequest(long requestId)
    {
        _requestService.RejectRequest(requestId);
        LoadRequests();
    }
}