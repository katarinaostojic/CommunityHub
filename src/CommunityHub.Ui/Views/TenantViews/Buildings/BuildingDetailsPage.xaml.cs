using CommunityHub.Application.Domain;
using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Buildings;
using CommunityHub.Ui.Converters;
using CommunityHub.Ui.Helpers;
using CommunityHub.Ui.ViewModels.TenantViewModels.Buildings;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class BuildingDetailsPage : Page
{
    private readonly User _user;
    private readonly BuildingDetailsViewModel _viewModel;

    public BuildingDetailsPage(BuildingDto buildingDto, User user)
    {
        InitializeComponent();
        _user = user;

        BuildingAccessRequestService requestService = Injector.CreateInstance<BuildingAccessRequestService>();

        _viewModel = new BuildingDetailsViewModel(buildingDto, requestService);
        DataContext = _viewModel;

        UserNameTextBlock.Text = _user.DisplayName;
        AppMenu.Initialize(_user);
        RefreshImage();
    }

    private void RefreshImage()
    {
        if (!_viewModel.HasImages)
        {
            BuildingImage.Source = null;
            SwipeText.Visibility = Visibility.Collapsed;
            return;
        }

        BuildingImage.Source = ImagePathConverter.LoadImage(_viewModel.CurrentImagePath!);
    }

    private void PrevImageButton_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.PreviousImage();
        RefreshImage();
    }

    private void NextImageButton_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.NextImage();
        RefreshImage();
    }

    private void RequestAccessButton_Click(object sender, RoutedEventArgs e)
    {
        if (!ShowBuildingRequestAccessDialog(_viewModel.BuildingDto)) return;

        _viewModel.RefreshPendingRequestsCount();
        ViewRequestsButton.Visibility = Visibility.Visible;
        NotificationBanner.ShowSuccess(SuccessBanner, SuccessTextBlock, "✔ Request Sent Successfully!");
    }

    private bool ShowBuildingRequestAccessDialog(BuildingDto building)
    {
        BuildingAccessRequestDialog dialog = new BuildingAccessRequestDialog(building, _user);
        dialog.Owner = Window.GetWindow(this);
        return dialog.ShowDialog() == true;
    }

    private void ViewRequestsButton_Click(object sender, RoutedEventArgs e) =>
        NavigationService.Navigate(new MyBuildingRequestsPage(_user));

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (NavigationService.CanGoBack)
            NavigationService.GoBack();
        else
            NavigationService.Navigate(new BrowseBuildingsPage(_user));
    }
    private void MenuButton_Click(object sender, RoutedEventArgs e) =>
        AppMenu.Open();
}
