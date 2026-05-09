using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.Services;
using CommunityHub.Application.Services.Ads;
using CommunityHub.Ui.ViewModels.TenantViewModels.Ads;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class BookSlotsPage : Page
{
    private readonly User _user;
    private readonly BuildingMembership _membership;
    private readonly Ad? _myAd;
    private readonly BookSlotsViewModel _viewModel;

    public BookSlotsPage(User user, BuildingMembership membership, Ad theirAd, Ad? myAd = null)
    {
        InitializeComponent();
        _user = user;
        _membership = membership;
        _myAd = myAd;

        AdService adService = ServiceFactory.CreateAdService();
        _viewModel = new BookSlotsViewModel(adService, theirAd, myAd);
        DataContext = _viewModel;

        UserNameTextBlock.Text = _user.DisplayName;
        AppMenu.Initialize(_user);
    }

    private void SlotChip_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is Border border && border.DataContext is SelectableSlotChipViewModel slot)
            _viewModel.ToggleSlot(slot);
    }

    private void ConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        bool booked = _viewModel.BookSelectedSlots();
        if (!booked) return;
        NavigationService.Navigate(new AdDetailsPage(_user, _membership, _myAd));
    }

    private void GoBackButton_Click(object sender, RoutedEventArgs e) =>
        NavigationService.GoBack();

    private void MenuButton_Click(object sender, RoutedEventArgs e) => AppMenu.Open();
}