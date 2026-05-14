using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.DTOs.TenantAds;
using CommunityHub.Application.Services.Ads;
using CommunityHub.Ui.ViewModels.TenantViewModels.Ads;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class AdDetailsPage : Page
{
    private readonly User _user;
    private readonly BuildingMembershipDto _membership;
    private readonly AdDetailsViewModel _viewModel;

    public AdDetailsPage(User user, BuildingMembershipDto membership, AdDto ad)
    {
        InitializeComponent();
        _user = user;
        _membership = membership;

        AdService adService = Injector.CreateInstance<AdService>();
        _viewModel = new AdDetailsViewModel(ad, adService);
        DataContext = _viewModel;

        UserNameTextBlock.Text = _user.DisplayName;
        AppMenu.Initialize(_user);
        _viewModel.Slots.LoadSlots();
    }

    private void ArchiveButton_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.ArchiveAd();
    }

    private void RestoreButton_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.RestoreAd();
        NavigationService.Navigate(new NoticeBoardPage(_user, _membership));
    }

    private void GoBackButton_Click(object sender, RoutedEventArgs e) =>
        NavigationService.Navigate(new NoticeBoardPage(_user, _membership));

    private void MenuButton_Click(object sender, RoutedEventArgs e) => AppMenu.Open();
}