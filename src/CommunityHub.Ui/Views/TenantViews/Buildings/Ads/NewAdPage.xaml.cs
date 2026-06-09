using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Entities.Buildings.Ads;
using CommunityHub.Ui.Helpers.Tenant.Demo.Building.Ads;
using CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.Ads.NewAd;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class NewAdPage : Page
{
    private readonly User _user;
    private readonly BuildingMembershipDto _membership;
    private readonly NewAdViewModel _viewModel;
    private readonly bool _startDemo;
    private readonly NewAdDemoController _demoController;

    public NewAdPage(User user, BuildingMembershipDto membership, bool startDemo = false)
    {
        InitializeComponent();

        _user = user;
        _membership = membership;
        _startDemo = startDemo;

        AdService adService = Injector.CreateInstance<AdService>();
        _viewModel = new NewAdViewModel(adService, membership, user);

        DataContext = _viewModel;
        InitializePageData();

        _demoController = new NewAdDemoController(
            DescriptionTextBox,
            CategoryComboBox,
            DateFromPicker,
            DateToPicker,
            SeekingButton,
            DemoButton,
            ReturnToNoticeBoard);

        if (_startDemo)
            Loaded += StartDemoOnLoaded;
    }

    private void InitializePageData()
    {
        UserNameTextBlock.Text = _user.DisplayName;
        NotificationBell.Initialize(_user);
        AppMenu.Initialize(_user);

        DateFromPicker.DisplayDateStart = DateTime.Today;
        DateToPicker.DisplayDateStart = DateTime.Today;

        CategoryComboBox.ItemsSource = _viewModel.CategoryOptions;
        CategoryComboBox.SelectedIndex = 0;
    }

    private void OfferingButton_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.SelectOffering();
        UpdateTypeButtons();
    }

    private void SeekingButton_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.SelectSeeking();
        UpdateTypeButtons();
    }

    private void UpdateTypeButtons()
    {
        OfferingButton.Style = _viewModel.IsOfferingSelected
            ? (Style)FindResource("FilterChipButtonActive")
            : (Style)FindResource("FilterChipButton");

        SeekingButton.Style = _viewModel.IsSeekingSelected
            ? (Style)FindResource("FilterChipButtonActive")
            : (Style)FindResource("FilterChipButton");
    }

    private void PostAdButton_Click(object sender, RoutedEventArgs e)
    {
        var result = _viewModel.TryCreateAd(
            DescriptionTextBox.Text,
            DateFromPicker.SelectedDate,
            DateToPicker.SelectedDate,
            CategoryComboBox.SelectedIndex);

        if (result == null)
            return;

        NavigationService.Navigate(new AdPostedPage(
            _user,
            _membership,
            result.Value.newAd,
            result.Value.matchingAds));
    }

    private async void StartDemoOnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= StartDemoOnLoaded;
        await _demoController.StartAsync();
    }

    private void DemoButton_Click(object sender, RoutedEventArgs e)
    {
        _demoController.Stop();
    }

    private void GoBackButton_Click(object sender, RoutedEventArgs e)
    {
        GoBackToNoticeBoard();
    }

    private void GoBackToNoticeBoard()
    {
        ReturnToNoticeBoard(false);
    }

    private void ReturnToNoticeBoard(bool restartDemo)
    {
        NavigationService?.Navigate(new NoticeBoardPage(_user, _membership, restartDemo));
    }

    private void MenuButton_Click(object sender, RoutedEventArgs e)
    {
        AppMenu.Open();
    }
}