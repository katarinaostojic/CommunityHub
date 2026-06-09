using CommunityHub.Application.Domain.Entities.Buildings.Ads;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Ui.Helpers.Tenant.NoticeBoard;
using CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.Ads.NoticeBoard;
using CommunityHub.Ui.Views.TenantViews;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Helpers.Tenant.Demo.Building.Ads;

public class NoticeBoardDemoController
{
    private const int ShortPause = 1400;
    private const int MediumPause = 2000;
    private const int LongPause = 2400;

    private readonly TenantDemoRunner _demoRunner;
    private readonly NoticeBoardViewModel _viewModel;
    private readonly NoticeBoardFilterController _filterController;
    private readonly User _user;
    private readonly BuildingMembershipDto _membership;
    private readonly Page _page;
    private readonly ComboBox _categoryComboBox;
    private readonly ScrollViewer _adsScrollViewer;
    private readonly Button _restoreAdButton;

    private NoticeBoardDemoState? _state;

    public NoticeBoardDemoController(
        Button demoButton,
        NoticeBoardViewModel viewModel,
        NoticeBoardFilterController filterController,
        User user,
        BuildingMembershipDto membership,
        Page page,
        ComboBox categoryComboBox,
        ScrollViewer adsScrollViewer,
        Button restoreAdButton)
    {
        _demoRunner = new TenantDemoRunner(demoButton);
        _viewModel = viewModel;
        _filterController = filterController;
        _user = user;
        _membership = membership;
        _page = page;
        _categoryComboBox = categoryComboBox;
        _adsScrollViewer = adsScrollViewer;
        _restoreAdButton = restoreAdButton;
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

        _filterController.SelectCategoryIfAvailable(3);
        await _demoRunner.DelayAsync(token, ShortPause);

        _adsScrollViewer.ScrollToEnd();
        await _demoRunner.DelayAsync(token, LongPause);

        _adsScrollViewer.ScrollToHome();
        await _demoRunner.DelayAsync(token, MediumPause);

        _filterController.ApplyTypeFilter("Offering");
        await _demoRunner.DelayAsync(token, LongPause);

        _demoRunner.Stop();
        _page.NavigationService?.Navigate(new NewAdPage(_user, _membership, startDemo: true));
    }

    private void ResetDemoView()
    {
        _filterController.ApplyTypeFilter("All");
        _filterController.SelectCategoryIfAvailable(0);
        _adsScrollViewer.ScrollToHome();
        _restoreAdButton.Visibility = Visibility.Collapsed;
    }

    private NoticeBoardDemoState CaptureState()
    {
        string typeFilter = _viewModel.Filters.TypeFilter switch
        {
            null => "All",
            AdType.Offering => "Offering",
            AdType.Seeking => "Seeking",
            _ => "All"
        };

        return new NoticeBoardDemoState(
            typeFilter,
            _categoryComboBox.SelectedIndex,
            _restoreAdButton.Visibility,
            _adsScrollViewer.VerticalOffset);
    }

    private void RestoreState()
    {
        if (_state == null)
            return;

        _filterController.ApplyTypeFilter(_state.TypeFilter);
        _filterController.SelectCategoryIfAvailable(_state.CategoryIndex);

        _restoreAdButton.Visibility = _state.RestoreAdButtonVisibility;
        _adsScrollViewer.ScrollToVerticalOffset(_state.ScrollOffset);

        _state = null;
    }

    private sealed record NoticeBoardDemoState(
        string TypeFilter,
        int CategoryIndex,
        Visibility RestoreAdButtonVisibility,
        double ScrollOffset);
}