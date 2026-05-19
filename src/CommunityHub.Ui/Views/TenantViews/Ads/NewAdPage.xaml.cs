using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Services.Ads;
using CommunityHub.Ui.ViewModels.TenantViewModels.Ads;
using System.Windows;
using System.Windows.Controls;
using CommunityHub.Application.Domain.Shared;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class NewAdPage : Page
{
    private readonly User _user;
    private readonly BuildingMembershipDto _membership;
    private readonly NewAdViewModel _viewModel;

    public NewAdPage(User user, BuildingMembershipDto membership)
    {
        InitializeComponent();
        _user = user;
        _membership = membership;

        AdService adService = Injector.CreateInstance<AdService>();
        _viewModel = new NewAdViewModel(adService, membership, user);
        DataContext = _viewModel;

        UserNameTextBlock.Text = _user.DisplayName;
        DateFromPicker.DisplayDateStart = DateTime.Today;
        DateToPicker.DisplayDateStart = DateTime.Today;
        AppMenu.Initialize(_user);
        InitializeCategoryComboBox();
    }

    private void InitializeCategoryComboBox()
    {
        foreach (string option in _viewModel.CategoryOptions)
            CategoryComboBox.Items.Add(option);
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

        if (result == null) return;

        NavigationService.Navigate(new AdPostedPage(_user, _membership, result.Value.newAd, result.Value.matchingAds));
    }

    private void GoBackButton_Click(object sender, RoutedEventArgs e) =>
        NavigationService.Navigate(new NoticeBoardPage(_user, _membership));

    private void MenuButton_Click(object sender, RoutedEventArgs e) => AppMenu.Open();
}