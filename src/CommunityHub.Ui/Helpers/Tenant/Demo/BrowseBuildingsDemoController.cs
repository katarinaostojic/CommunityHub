using CommunityHub.Ui.Helpers;
using CommunityHub.Ui.ViewModels.TenantViewModels.Buildings;
using System.Windows.Controls;

namespace CommunityHub.Ui.Helpers.Tenant.Demo;

public class BrowseBuildingsDemoController
{
    private readonly TenantDemoRunner _demoRunner;
    private readonly TextBox _searchTextBox;
    private readonly TextBox _streetTextBox;
    private readonly TextBox _neighborhoodTextBox;
    private readonly TextBox _cityTextBox;
    private readonly TextBox _countryTextBox;
    private readonly FilterPanelAnimationHelper _filterPanel;
    private readonly BrowseBuildingsViewModel _viewModel;

    private BrowseBuildingsDemoState? _state;

    public BrowseBuildingsDemoController(
        Button demoButton,
        TextBox searchTextBox,
        TextBox streetTextBox,
        TextBox neighborhoodTextBox,
        TextBox cityTextBox,
        TextBox countryTextBox,
        FilterPanelAnimationHelper filterPanel,
        BrowseBuildingsViewModel viewModel)
    {
        _demoRunner = new TenantDemoRunner(demoButton);
        _searchTextBox = searchTextBox;
        _streetTextBox = streetTextBox;
        _neighborhoodTextBox = neighborhoodTextBox;
        _cityTextBox = cityTextBox;
        _countryTextBox = countryTextBox;
        _filterPanel = filterPanel;
        _viewModel = viewModel;
    }

    public async Task ToggleAsync()
    {
        if (!_demoRunner.IsRunning)
            _state = CaptureState();

        await _demoRunner.ToggleAsync(RunCycleAsync, RestoreState);
    }

    private async Task RunCycleAsync(CancellationToken token)
    {
        _filterPanel.Close();

        _searchTextBox.Text = "Knez";
        await _demoRunner.DelayAsync(token, 1400);

        _searchTextBox.Text = string.Empty;
        await _demoRunner.DelayAsync(token, 800);

        _filterPanel.Open();
        await _demoRunner.DelayAsync(token, 700);

        _neighborhoodTextBox.Text = "Liman";
        _viewModel.Search(null, OptionalText(_neighborhoodTextBox), null, null);
        await _demoRunner.DelayAsync(token, 1500);

        _filterPanel.Close();
        ResetDemoFields();
        _viewModel.Search(null, null, null, null);
        await _demoRunner.DelayAsync(token, 1000);

        if (_viewModel.HasNextPage)
        {
            _viewModel.NextPage();
            await _demoRunner.DelayAsync(token, 1200);
        }

        if (_viewModel.HasPreviousPage)
        {
            _viewModel.PreviousPage();
            await _demoRunner.DelayAsync(token, 1200);
        }
    }

    private BrowseBuildingsDemoState CaptureState()
    {
        return new BrowseBuildingsDemoState(
            _searchTextBox.Text,
            _streetTextBox.Text,
            _neighborhoodTextBox.Text,
            _cityTextBox.Text,
            _countryTextBox.Text);
    }

    private void RestoreState()
    {
        if (_state == null)
            return;

        _searchTextBox.Text = _state.SearchText;
        _streetTextBox.Text = _state.StreetText;
        _neighborhoodTextBox.Text = _state.NeighborhoodText;
        _cityTextBox.Text = _state.CityText;
        _countryTextBox.Text = _state.CountryText;

        RestoreSearchResults();
        _filterPanel.Close();

        _state = null;
    }

    private void RestoreSearchResults()
    {
        if (!string.IsNullOrWhiteSpace(_searchTextBox.Text))
        {
            _viewModel.Search(OptionalText(_searchTextBox), null, null, null);
            return;
        }

        _viewModel.Search(
            OptionalText(_streetTextBox),
            OptionalText(_neighborhoodTextBox),
            OptionalText(_cityTextBox),
            OptionalText(_countryTextBox));
    }

    private void ResetDemoFields()
    {
        _searchTextBox.Text = string.Empty;
        _streetTextBox.Text = string.Empty;
        _neighborhoodTextBox.Text = string.Empty;
        _cityTextBox.Text = string.Empty;
        _countryTextBox.Text = string.Empty;
    }

    private static string? OptionalText(TextBox textBox)
    {
        string text = textBox.Text.Trim();
        return string.IsNullOrWhiteSpace(text) ? null : text;
    }

    private sealed record BrowseBuildingsDemoState(
        string SearchText,
        string StreetText,
        string NeighborhoodText,
        string CityText,
        string CountryText);
}