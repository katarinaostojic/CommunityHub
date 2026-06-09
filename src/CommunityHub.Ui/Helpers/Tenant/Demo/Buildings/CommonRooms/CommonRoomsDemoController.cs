using CommunityHub.Ui.Helpers.Tenant.Demo.Buildings.CommonRooms;
using CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.CommonRooms;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Helpers.Tenant.Demo;

public class CommonRoomsDemoController
{
    private const int MediumPause = 1800;

    private readonly TenantDemoRunner _demoRunner;
    private readonly CommonRoomsViewModel _viewModel;
    private readonly ScrollViewer _roomsScrollViewer;
    private readonly Border _successBanner;
    private readonly TextBlock _successTextBlock;
    private readonly Button _viewRequestsButton;
    private readonly CommonRoomRequestDemoPlayer _requestDemoPlayer;

    private CommonRoomsDemoState? _state;

    public CommonRoomsDemoController(
        Button demoButton,
        CommonRoomsViewModel viewModel,
        string buildingInfo,
        ScrollViewer roomsScrollViewer,
        Border successBanner,
        TextBlock successTextBlock,
        Button viewRequestsButton,
        Func<Window?> getOwner)
    {
        _demoRunner = new TenantDemoRunner(demoButton);
        _viewModel = viewModel;
        _roomsScrollViewer = roomsScrollViewer;
        _successBanner = successBanner;
        _successTextBlock = successTextBlock;
        _viewRequestsButton = viewRequestsButton;
        _requestDemoPlayer = new CommonRoomRequestDemoPlayer(
            _demoRunner,
            buildingInfo,
            successBanner,
            successTextBlock,
            viewRequestsButton,
            getOwner);
    }

    public async Task ToggleAsync()
    {
        if (!_demoRunner.IsRunning)
            _state = CaptureState();

        await _demoRunner.ToggleAsync(RunCycleAsync, RestoreState);
    }

    private async Task RunCycleAsync(CancellationToken token)
    {
        ResetDemoView();
        await _demoRunner.DelayAsync(token, MediumPause);

        await ShowCommonRoomsListAsync(token);
        await _requestDemoPlayer.ShowAsync(_viewModel.Rooms.FirstOrDefault(), token);

        await _demoRunner.DelayAsync(token, MediumPause);
    }

    private async Task ShowCommonRoomsListAsync(CancellationToken token)
    {
        _roomsScrollViewer.ScrollToEnd();
        await _demoRunner.DelayAsync(token, MediumPause);

        _roomsScrollViewer.ScrollToHome();
        await _demoRunner.DelayAsync(token, MediumPause);
    }

    private void ResetDemoView()
    {
        _roomsScrollViewer.ScrollToHome();
        _successBanner.Visibility = Visibility.Collapsed;
    }

    private CommonRoomsDemoState CaptureState()
    {
        return new CommonRoomsDemoState(
            _successBanner.Visibility,
            _successTextBlock.Text,
            _viewRequestsButton.Visibility,
            _roomsScrollViewer.VerticalOffset);
    }

    private void RestoreState()
    {
        if (_state == null)
            return;

        _successBanner.Visibility = _state.SuccessBannerVisibility;
        _successTextBlock.Text = _state.SuccessText;
        _viewRequestsButton.Visibility = _state.ViewRequestsButtonVisibility;
        _roomsScrollViewer.ScrollToVerticalOffset(_state.ScrollOffset);

        _state = null;
    }

    private sealed record CommonRoomsDemoState(
        Visibility SuccessBannerVisibility,
        string SuccessText,
        Visibility ViewRequestsButtonVisibility,
        double ScrollOffset);
}