using CommunityHub.Application.Database.Repositories.Neighborhoods;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class CityObjectsPage : Page
{
    private readonly CityObjectsViewModel _viewModel;
    private readonly long _neighborhoodId;

    public CityObjectsPage(long neighborhoodId)
    {
        InitializeComponent();
        _neighborhoodId = neighborhoodId;
        var cityObjectService = new CityObjectService(new CityObjectDbRepository());
        _viewModel = new CityObjectsViewModel(cityObjectService, neighborhoodId);
        DataContext = _viewModel;
    }

    private void ReserveButton_Click(object sender, RoutedEventArgs e)
    {
        CityObjectItemViewModel item = (CityObjectItemViewModel)((Button)sender).Tag;
        CoordinatorMainWindow.Instance.NavigateTo(
            new ReserveCityObjectPage(item, _neighborhoodId), "Reserve City Object");
    }
}