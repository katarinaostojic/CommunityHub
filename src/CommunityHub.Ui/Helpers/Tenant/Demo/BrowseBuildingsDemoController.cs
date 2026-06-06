using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Ui.Helpers;
using CommunityHub.Ui.ViewModels.TenantViewModels.Buildings;
using CommunityHub.Ui.Views.TenantViews;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Helpers.Tenant.Demo;

public class BrowseBuildingsDemoController
{
    private const int ShortPause = 1600;
    private const int MediumPause = 2400;
    private const int LongPause = 3600;
    private const int TypingPause = 650;

    private readonly TenantDemoRunner _demoRunner;
    private readonly TextBox _searchTextBox;
    private readonly TextBox _streetTextBox;
    private readonly TextBox _neighborhoodTextBox;
    private readonly TextBox _cityTextBox;
    private readonly TextBox _countryTextBox;
    private readonly Border _successBanner;
    private readonly TextBlock _successTextBlock;
    private readonly Button _viewRequestsButton;
    private readonly FilterPanelAnimationHelper _filterPanel;
    private readonly BrowseBuildingsViewModel _viewModel;
    private readonly User _user;
    private readonly Func<Window?> _getOwner;

    private BrowseBuildingsDemoState? _state;
    private bool _hasSubmittedRequestInCurrentDemo;

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
        _searchTextBox = searchTextBox;
        _streetTextBox = streetTextBox;
        _neighborhoodTextBox = neighborhoodTextBox;
        _cityTextBox = cityTextBox;
        _countryTextBox = countryTextBox;
        _successBanner = successBanner;
        _successTextBlock = successTextBlock;
        _viewRequestsButton = viewRequestsButton;
        _filterPanel = filterPanel;
        _viewModel = viewModel;
        _user = user;
        _getOwner = getOwner;
    }

    public async Task ToggleAsync()
    {
        if (!_demoRunner.IsRunning)
        {
            _state = CaptureState();
            _hasSubmittedRequestInCurrentDemo = false;
        }

        await _demoRunner.ToggleAsync(RunCycleAsync, RestoreState);
    }

    private async Task RunCycleAsync(CancellationToken token)
    {
        ResetDemoFields();
        _viewModel.Search(null, null, null, null);
        await _demoRunner.DelayAsync(token, MediumPause);

        await ShowSearchDemoAsync(token);
        await ShowFilterDemoAsync(token);
        await ShowRequestAccessDemoAsync(token);

        ResetDemoFields();
        _filterPanel.Close();
        _viewModel.Search(null, null, null, null);
        await _demoRunner.DelayAsync(token, MediumPause);
    }

    private async Task ShowSearchDemoAsync(CancellationToken token)
    {
        await TypeSearchTextAsync("knez", token);
        await _demoRunner.DelayAsync(token, LongPause);

        _searchTextBox.Text = string.Empty;
        _viewModel.Search(null, null, null, null);
        await _demoRunner.DelayAsync(token, MediumPause);
    }

    private async Task ShowFilterDemoAsync(CancellationToken token)
    {
        _filterPanel.Open();
        await _demoRunner.DelayAsync(token, MediumPause);

        await TypeTextAsync(_streetTextBox, "knez", token);

        _viewModel.Search(
            OptionalText(_streetTextBox),
            null,
            null,
            null);

        await _demoRunner.DelayAsync(token, LongPause);

        _filterPanel.Close();
        await _demoRunner.DelayAsync(token, MediumPause);
    }

    private async Task ShowRequestAccessDemoAsync(CancellationToken token)
    {
        BuildingDto? building = _viewModel.CurrentPageBuildings.FirstOrDefault();

        if (building == null)
            return;

        BuildingAccessRequestDialog? dialog = null;

        try
        {
            dialog = new BuildingAccessRequestDialog(building, _user)
            {
                Owner = _getOwner()
            };

            dialog.Show();
            await _demoRunner.DelayAsync(token, MediumPause);

            string unitNumber = dialog.GetDemoUnitNumber();
            await TypeApartmentNumberAsync(dialog, unitNumber, token);

            await _demoRunner.DelayAsync(token, MediumPause);

            if (!_hasSubmittedRequestInCurrentDemo)
            {
                dialog.ClickSendRequestForDemo();
                _hasSubmittedRequestInCurrentDemo = true;

                ShowRequestSentBanner(building);
                await _demoRunner.DelayAsync(token, LongPause);
            }
        }
        finally
        {
            if (dialog?.IsVisible == true)
                dialog.Close();
        }
    }

    private async Task TypeSearchTextAsync(string text, CancellationToken token)
    {
        _searchTextBox.Text = string.Empty;

        foreach (char character in text)
        {
            _searchTextBox.Text += character;
            _searchTextBox.CaretIndex = _searchTextBox.Text.Length;
            _viewModel.Search(OptionalText(_searchTextBox), null, null, null);

            await _demoRunner.DelayAsync(token, TypingPause);
        }
    }

    private async Task TypeTextAsync(TextBox textBox, string text, CancellationToken token)
    {
        textBox.Text = string.Empty;

        foreach (char character in text)
        {
            textBox.Text += character;
            textBox.CaretIndex = textBox.Text.Length;

            await _demoRunner.DelayAsync(token, TypingPause);
        }
    }

    private async Task TypeApartmentNumberAsync(
        BuildingAccessRequestDialog dialog,
        string unitNumber,
        CancellationToken token)
    {
        string typedText = string.Empty;
        dialog.SetUnitNumberForDemo(typedText);

        foreach (char character in unitNumber)
        {
            typedText += character;
            dialog.SetUnitNumberForDemo(typedText);

            await _demoRunner.DelayAsync(token, TypingPause);
        }
    }

    private void ShowRequestSentBanner(BuildingDto building)
    {
        NotificationBanner.ShowSuccess(
            _successBanner,
            _successTextBlock,
            $"✔ Request Sent Successfully! The administrator of {building.FullAddress} has been notified.");

        _viewRequestsButton.Visibility = Visibility.Visible;
    }

    private BrowseBuildingsDemoState CaptureState()
    {
        return new BrowseBuildingsDemoState(
            _searchTextBox.Text,
            _streetTextBox.Text,
            _neighborhoodTextBox.Text,
            _cityTextBox.Text,
            _countryTextBox.Text,
            _successBanner.Visibility,
            _successTextBlock.Text,
            _viewRequestsButton.Visibility);
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
        _successBanner.Visibility = _state.SuccessBannerVisibility;
        _successTextBlock.Text = _state.SuccessText;
        _viewRequestsButton.Visibility = _state.ViewRequestsButtonVisibility;

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
        string CountryText,
        Visibility SuccessBannerVisibility,
        string SuccessText,
        Visibility ViewRequestsButtonVisibility);
}