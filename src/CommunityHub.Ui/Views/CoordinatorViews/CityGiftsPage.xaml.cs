using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class CityGiftsPage : Page
{
    private readonly CityGiftsViewModel _viewModel;

    public CityGiftsPage(long coordinatorId)
    {
        InitializeComponent();
        var cityGiftService = Injector.CreateInstance<CityGiftService>();
        var neighborhoodService = Injector.CreateInstance<NeighborhoodService>();
        _viewModel = new CityGiftsViewModel(cityGiftService, neighborhoodService, coordinatorId);
        DataContext = _viewModel;
    }

    private async void ApplyButton_Click(object sender, RoutedEventArgs e)
    {
        CityGiftItemViewModel gift = (CityGiftItemViewModel)((Button)sender).Tag;
        var (success, error) = _viewModel.Apply(gift.Id);

        if (!success)
        {
            ErrorText.Text = error;
            ErrorBanner.Visibility = Visibility.Visible;
            return;
        }

        ErrorBanner.Visibility = Visibility.Collapsed;
        SuccessBanner.Visibility = Visibility.Visible;
        await Task.Delay(3000);
        SuccessBanner.Visibility = Visibility.Collapsed;
    }
}