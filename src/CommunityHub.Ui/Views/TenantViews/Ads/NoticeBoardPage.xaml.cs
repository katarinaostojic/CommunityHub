using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Shared;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Ads;
using CommunityHub.Ui.Helpers;
using CommunityHub.Ui.ViewModels.TenantViewModels.Ads.NoticeBoard;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class NoticeBoardPage : Page
{
    private readonly User _user;
    private readonly BuildingMembershipDto _membership;
    private readonly NoticeBoardViewModel _viewModel;
    private readonly NoticeBoardNavigationHelper _navigationHelper;
    private long? _lastArchivedAdId;

    public NoticeBoardPage(User user, BuildingMembershipDto membership)
    {
        InitializeComponent();

        _user = user;
        _membership = membership;
        _viewModel = CreateViewModel();
        _navigationHelper = new NoticeBoardNavigationHelper(_user, _membership, _viewModel, this);

        DataContext = _viewModel;

        InitializeHeader();
        InitializeCategoryFilter();
    }

    public void ShowBookingSuccess() =>
        NotificationBanner.ShowSuccess(SuccessBanner, SuccessTextBlock, "✔ Slots booked successfully!");

    private NoticeBoardViewModel CreateViewModel()
    {
        AdService adService = Injector.CreateInstance<AdService>();
        AdNotificationService notificationService = Injector.CreateInstance<AdNotificationService>();

        return new NoticeBoardViewModel(
            adService,
            notificationService,
            _membership,
            _user.Id);
    }

    private void InitializeHeader()
    {
        UserNameTextBlock.Text = _user.DisplayName;
        AppMenu.Initialize(_user);
    }

    private void InitializeCategoryFilter()
    {
        CategoryComboBox.ItemsSource = _viewModel.CategoryOptions;
        CategoryComboBox.SelectedIndex = 0;
    }

    private void FilterAllButton_Click(object sender, RoutedEventArgs e) =>
        _viewModel.FilterAll();

    private void FilterOfferingButton_Click(object sender, RoutedEventArgs e) =>
        _viewModel.FilterOffering();

    private void FilterSeekingButton_Click(object sender, RoutedEventArgs e) =>
        _viewModel.FilterSeeking();

    private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
        _viewModel.FilterByCategory(CategoryComboBox.SelectedIndex);

    private void NewAdButton_Click(object sender, RoutedEventArgs e) =>
        NavigationService.Navigate(new NewAdPage(_user, _membership));

    private void ArchiveButton_Click(object sender, RoutedEventArgs e)
    {
        AdViewModel ad = GetAdFromButton(sender);
        _lastArchivedAdId = ad.Id;
        _viewModel.ArchiveAd(ad.Id);
        ShowArchiveSuccess();
    }

    private void RestoreAdButton_Click(object sender, RoutedEventArgs e)
    {
        if (_lastArchivedAdId == null) return;

        _viewModel.RestoreAd(_lastArchivedAdId.Value);
        _lastArchivedAdId = null;
        HideArchiveSuccess();
    }

    private void ViewSlotsButton_Click(object sender, RoutedEventArgs e)
    {
        AdViewModel ad = GetAdFromButton(sender);
        NavigateTo(_navigationHelper.CreateBookSlotsPage(ad));
    }

    private void ViewDetailsButton_Click(object sender, RoutedEventArgs e)
    {
        AdViewModel ad = GetAdFromButton(sender);
        NavigateTo(_navigationHelper.CreateAdDetailsPage(ad));
    }

    private void DismissNotificationButton_Click(object sender, RoutedEventArgs e)
    {
        long notificationId = (long)((Button)sender).Tag;
        _viewModel.Notifications.Dismiss(notificationId);
    }

    private void ViewSlotsFromNotificationButton_Click(object sender, RoutedEventArgs e)
    {
        AdNotificationViewModel notification = (AdNotificationViewModel)((Button)sender).DataContext;
        NavigateTo(_navigationHelper.CreateAdDetailsPage(notification));
    }

    private void ExportPdfButton_Click(object sender, RoutedEventArgs e)
    {
        // TODO: export PDF
    }

    private void MenuButton_Click(object sender, RoutedEventArgs e) =>
        AppMenu.Open();

    private AdViewModel GetAdFromButton(object sender) =>
        (AdViewModel)((Button)sender).Tag;

    private void NavigateTo(Page? page)
    {
        if (page != null)
            NavigationService.Navigate(page);
    }

    private void ShowArchiveSuccess()
    {
        NotificationBanner.ShowSuccess(SuccessBanner, SuccessTextBlock, "✔ Ad archived successfully.");
        RestoreAdButton.Visibility = Visibility.Visible;
    }

    private void HideArchiveSuccess()
    {
        RestoreAdButton.Visibility = Visibility.Collapsed;
        SuccessBanner.Visibility = Visibility.Collapsed;
    }
}