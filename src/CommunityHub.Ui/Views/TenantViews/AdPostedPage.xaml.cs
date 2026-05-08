using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.Services;
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
        // TODO: navigate to ViewSlotsPage
    }

    private void BackButton_Click(object sender, RoutedEventArgs e) =>
        NavigationService.Navigate(new NoticeBoardPage(_user, _membership));

    private void MenuButton_Click(object sender, RoutedEventArgs e) => AppMenu.Open();
}