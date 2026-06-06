using CommunityHub.Application.Domain.Entities.Buildings.CommonRooms;
using CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.CommonRooms;
using CommunityHub.Ui.ViewModels.TenantViewModels.Dialogs.Buildings.CommonRooms;
using CommunityHub.Ui.Views.TenantViews.Dialogs.Buildings;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Helpers.Tenant.Demo;

public class CommonRoomsDemoController
{
    private const int ShortPause = 1400;
    private const int MediumPause = 1800;
    private const int LongPause = 2800;

    private readonly TenantDemoRunner _demoRunner;
    private readonly CommonRoomsViewModel _viewModel;
    private readonly string _buildingInfo;
    private readonly ScrollViewer _roomsScrollViewer;
    private readonly Border _successBanner;
    private readonly TextBlock _successTextBlock;
    private readonly Button _viewRequestsButton;
    private readonly Func<Window?> _getOwner;

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
        _buildingInfo = buildingInfo;
        _roomsScrollViewer = roomsScrollViewer;
        _successBanner = successBanner;
        _successTextBlock = successTextBlock;
        _viewRequestsButton = viewRequestsButton;
        _getOwner = getOwner;
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
        await ShowRequestRentalAsync(token);

        await _demoRunner.DelayAsync(token, MediumPause);
    }

    private async Task ShowCommonRoomsListAsync(CancellationToken token)
    {
        _roomsScrollViewer.ScrollToEnd();
        await _demoRunner.DelayAsync(token, MediumPause);

        _roomsScrollViewer.ScrollToHome();
        await _demoRunner.DelayAsync(token, MediumPause);
    }

    private async Task ShowRequestRentalAsync(CancellationToken token)
    {
        CommonRoomCardViewModel? room = _viewModel.Rooms.FirstOrDefault();

        if (room == null)
            return;

        CommonRoomRequestDialog? dialog = null;

        try
        {
            dialog = CreateDialog(room);
            dialog.Show();

            await _demoRunner.DelayAsync(token, MediumPause);

            DateTime dateFrom = DateTime.Today.AddDays(1);
            DateTime dateTo = GetDemoDateTo(room, dateFrom);

            await PickDateFromAsync(dialog, dateFrom, token);
            await PickDateToAsync(dialog, dateTo, token);

            await _demoRunner.DelayAsync(token, ShortPause);

            if (dialog.ClickSendRequestForDemo())
                ShowDemoRequestSentBanner(room);

            await _demoRunner.DelayAsync(token, LongPause);
        }
        finally
        {
            if (dialog?.IsVisible == true)
                dialog.Close();
        }
    }

    private CommonRoomRequestDialog CreateDialog(CommonRoomCardViewModel room)
    {
        CommonRoomRequestDialogViewModel dialogViewModel = new CommonRoomRequestDialogViewModel(
            _buildingInfo,
            room.Name,
            room.FloorDisplay,
            room.RentalType);

        return new CommonRoomRequestDialog(dialogViewModel)
        {
            Owner = _getOwner()
        };
    }

    private static DateTime GetDemoDateTo(CommonRoomCardViewModel room, DateTime dateFrom)
    {
        return room.RentalType == RentalType.PerDay
            ? dateFrom.AddDays(2)
            : dateFrom.AddDays(3);
    }

    private async Task PickDateFromAsync(
        CommonRoomRequestDialog dialog,
        DateTime date,
        CancellationToken token)
    {
        dialog.OpenDateFromPickerForDemo();
        await _demoRunner.DelayAsync(token, MediumPause);

        dialog.SetDateFromForDemo(date);
        await _demoRunner.DelayAsync(token, ShortPause);

        dialog.CloseDateFromPickerForDemo();
    }

    private async Task PickDateToAsync(
        CommonRoomRequestDialog dialog,
        DateTime date,
        CancellationToken token)
    {
        dialog.OpenDateToPickerForDemo();
        await _demoRunner.DelayAsync(token, MediumPause);

        dialog.SetDateToForDemo(date);
        await _demoRunner.DelayAsync(token, ShortPause);

        dialog.CloseDateToPickerForDemo();
    }

    private void ShowDemoRequestSentBanner(CommonRoomCardViewModel room)
    {
        _successTextBlock.Text = $"✔ Demo: request for {room.Name} was prepared successfully.";
        _successBanner.Visibility = Visibility.Visible;
        _viewRequestsButton.Visibility = Visibility.Visible;
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