using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Ui.Helpers;
using CommunityHub.Ui.ViewModels.TenantViewModels.Buildings;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Helpers.Tenant.Demo.Buildings;

public class BrowseBuildingsDemoController
{
    private const int MediumPause = 1400;

    private readonly TenantDemoRunner _demoRunner;
    private readonly BrowseBuildingsDemoContext _context;
    private readonly BrowseBuildingsSearchDemoPlayer _searchDemoPlayer;
    private readonly BrowseBuildingsRequestDemoPlayer _requestDemoPlayer;

    private BrowseBuildingsDemoState? _state;

    public BrowseBuildingsDemoController(
        Button demoButton,
        TextBox searchTextBox,
        TextBox streetTextBox,
        TextBox neighborhoodTextBox,
        TextBox cityTextBox,
        TextBox countryTextBox,
        Border successBanner,
        TextBlock successTextBlock,
        Button viewRequestsButton,
        FilterPanelAnimationHelper filterPanel,
        BrowseBuildingsViewModel viewModel,
        User user,
        Func<Window?> getOwner)
    {
        _demoRunner = new TenantDemoRunner(demoButton);
        _context = new BrowseBuildingsDemoContext(
            searchTextBox,
            streetTextBox,
            neighborhoodTextBox,
            cityTextBox,
            countryTextBox,
            successBanner,
            successTextBlock,
            viewRequestsButton,
            filterPanel,
            viewModel,
            user,
            getOwner);

        _searchDemoPlayer = new BrowseBuildingsSearchDemoPlayer(_demoRunner, _context);
        _requestDemoPlayer = new BrowseBuildingsRequestDemoPlayer(_demoRunner, _context);
    }

    public async Task ToggleAsync()
    {
        if (!_demoRunner.IsRunning)
            _state = _context.CaptureState();

        await _demoRunner.ToggleAsync(RunCycleAsync, RestoreState);
    }

    private async Task RunCycleAsync(CancellationToken token)
    {
        ResetDemoView();
        await _demoRunner.DelayAsync(token, MediumPause);

        await _searchDemoPlayer.ShowAsync(token);
        await _requestDemoPlayer.ShowAsync(token);

        ResetDemoView();
        await _demoRunner.DelayAsync(token, MediumPause);
    }

    private void ResetDemoView()
    {
        _context.ResetFields();
        _context.FilterPanel.Close();
        _context.SearchAllBuildings();
    }

    private void RestoreState()
    {
        if (_state == null)
            return;

        _context.RestoreState(_state);
        _context.RestoreSearchResults();
        _context.FilterPanel.Close();

        _state = null;
    }
}