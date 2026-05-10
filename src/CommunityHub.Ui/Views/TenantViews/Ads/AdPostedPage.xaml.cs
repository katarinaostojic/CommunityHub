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

public partial class AdPostedPage : Page
{
    private readonly User _user;
    private readonly BuildingMembership _membership;
    private readonly AdPostedViewModel _viewModel;
    private readonly AdService _adService;

    public AdPostedPage(User user, BuildingMembership membership, Ad postedAd, List<Ad> matchingAds)
    {
        InitializeComponent();
        _user = user;
        _membership = membership;
        _adService = ServiceFactory.CreateAdService();

        _viewModel = new AdPostedViewModel(postedAd, matchingAds, membership);
        DataContext = _viewModel;

        UserNameTextBlock.Text = _user.DisplayName;
        AppMenu.Initialize(_user);
    }

    private void ViewSlotsButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is MatchingAdViewModel matchingAd)
        {
            Ad? theirAd = _adService.GetById(matchingAd.Id);
            if (theirAd == null) return;
            Ad? myAd = _adService.GetById(_viewModel.PostedAdId);
            if (myAd == null) return;
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