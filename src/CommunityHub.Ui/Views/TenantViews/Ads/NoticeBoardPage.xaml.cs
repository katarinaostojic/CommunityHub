using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.Services;
using CommunityHub.Application.Services.Ads;
using CommunityHub.Ui.Helpers;
using CommunityHub.Ui.ViewModels.TenantViewModels.Ads;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class NoticeBoardPage : Page
{
    private readonly User _user;
    private readonly BuildingMembership _membership;
    private readonly NoticeBoardViewModel _viewModel;
    private long? _lastArchivedAdId = null;

    public NoticeBoardPage(User user, BuildingMembership membership)
    {
        InitializeComponent();
        _user = user;
        _membership = membership;

        AdService adService = Injector.CreateInstance<AdService>();
        _viewModel = new NoticeBoardViewModel(adService, membership, user.Id);
        DataContext = _viewModel;

        UserNameTextBlock.Text = _user.DisplayName;
        AppMenu.Initialize(_user);
        InitializeCategoryFilter();
    }

    private void InitializeCategoryFilter()
    {
        foreach (string option in _viewModel.CategoryOptions)
            CategoryComboBox.Items.Add(option);
        CategoryComboBox.SelectedIndex = 0;
    }

    private void FilterAllButton_Click(object sender, RoutedEventArgs e) => _viewModel.FilterAll();

    private void FilterOfferingButton_Click(object sender, RoutedEventArgs e) => _viewModel.FilterOffering();

    private void FilterSeekingButton_Click(object sender, RoutedEventArgs e) => _viewModel.FilterSeeking();

    private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
        _viewModel.FilterByCategory(CategoryComboBox.SelectedIndex);

    private void NewAdButton_Click(object sender, RoutedEventArgs e) =>
        NavigationService.Navigate(new NewAdPage(_user, _membership));

    private void ArchiveButton_Click(object sender, RoutedEventArgs e)
    {
        AdViewModel ad = (AdViewModel)((Button)sender).Tag;
        _lastArchivedAdId = ad.Id;
        _viewModel.ArchiveAd(ad.Id);
        NotificationBanner.ShowSuccess(SuccessBanner, SuccessTextBlock, "✔ Ad archived successfully.");
        RestoreAdButton.Visibility = Visibility.Visible;
    }

    private void RestoreAdButton_Click(object sender, RoutedEventArgs e)
    {
        if (_lastArchivedAdId == null) return;
        _viewModel.RestoreAd(_lastArchivedAdId.Value);
        _lastArchivedAdId = null;
        RestoreAdButton.Visibility = Visibility.Collapsed;
        SuccessBanner.Visibility = Visibility.Collapsed;
    }

    private void ViewSlotsButton_Click(object sender, RoutedEventArgs e)
    {
        AdViewModel adVm = (AdViewModel)((Button)sender).Tag;
        Ad? theirAd = _viewModel.GetAdById(adVm.Id);
        if (theirAd == null) return;
        if (adVm.MyMatchingAdId == null) return;
        Ad? myAd = _viewModel.GetAdById(adVm.MyMatchingAdId.Value);
        if (myAd == null) return;
        NavigationService.Navigate(new BookSlotsPage(_user, _membership, theirAd, myAd, this));
    }

    private void ViewBookingsButton_Click(object sender, RoutedEventArgs e)
    {
        AdViewModel adVm = (AdViewModel)((Button)sender).Tag;
        Ad? ad = _viewModel.GetAdById(adVm.Id);
        if (ad == null) return;
        NavigationService.Navigate(new AdDetailsPage(_user, _membership, ad));
    }

    private void ExportPdfButton_Click(object sender, RoutedEventArgs e)
    {
        // TODO: export PDF
    }

    private void MenuButton_Click(object sender, RoutedEventArgs e) => AppMenu.Open();

    private void DismissNotificationButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is long notifId)
            _viewModel.DismissNotification(notifId);
    }

    private void ViewSlotsFromNotificationButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        if (button.DataContext is not AdNotificationViewModel notif) return;
        Ad? myAd = _viewModel.GetAdById(notif.AdId);
        if (myAd == null) return;
        NavigationService.Navigate(new AdDetailsPage(_user, _membership, myAd));
    }

    public void ShowBookingSuccess()
    {
        NotificationBanner.ShowSuccess(SuccessBanner, SuccessTextBlock, "✔ Slots booked successfully!");
    }
}