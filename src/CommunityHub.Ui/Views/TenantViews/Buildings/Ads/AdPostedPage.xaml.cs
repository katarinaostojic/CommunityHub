using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Ui.Helpers;
using System.Windows;
using System.Windows.Controls;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Services.Entities.Buildings.Ads;
using CommunityHub.Application.DTOs.Buildings.Ads;
using CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.Ads.NewAd;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class AdPostedPage : Page
{
    private readonly User _user;
    private readonly BuildingMembershipDto _membership;
    private readonly AdPostedViewModel _viewModel;

    public AdPostedPage(User user, BuildingMembershipDto membership, AdDto postedAd, List<AdDto> matchingAds)
    {
        InitializeComponent();
        _user = user;
        _membership = membership;

        AdService adService = Injector.CreateInstance<AdService>();
        _viewModel = new AdPostedViewModel(postedAd, matchingAds, membership, adService);
        DataContext = _viewModel;

        UserNameTextBlock.Text = _user.DisplayName;
        NotificationBell.Initialize(_user);
        AppMenu.Initialize(_user);
    }

    private void ViewSlotsButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is MatchingAdViewModel matchingAd)
        {
            var (theirAd, myAd) = _viewModel.GetAdsForBooking(matchingAd.Id);
            if (theirAd == null || myAd == null) return;
            NavigationService.Navigate(new BookSlotsPage(_user, _membership, theirAd, myAd, this));
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e) =>
        NavigationService.Navigate(new NoticeBoardPage(_user, _membership));

    private void MenuButton_Click(object sender, RoutedEventArgs e) => AppMenu.Open();

    public void ShowBookingSuccess()
    {
        NotificationBanner.ShowSuccess(SuccessBanner, SuccessTextBlock, "✔ Slots booked successfully!");
    }
}