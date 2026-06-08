using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Services.Entities.Neighborhoods.CityObjects;
using CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using CommunityHub.Application.Database.Repositories.Neighborhoods.CityObjects;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class CityObjectsPage : Page
{
    private readonly CityObjectsViewModel _viewModel;
    private string? _pendingSuccessMessage = null;

    public CityObjectsPage(long coordinatorId)
    {
        InitializeComponent();
        var cityObjectService = new CityObjectService(new CityObjectDbRepository());
        var neighborhoodService = Injector.CreateInstance<NeighborhoodService>();
        _viewModel = new CityObjectsViewModel(cityObjectService, neighborhoodService, coordinatorId);
        DataContext = _viewModel;
        this.IsVisibleChanged += (s, e) =>
        {
            if (this.IsVisible)
            {
                _viewModel.LoadCityObjects();
                if (_pendingSuccessMessage != null)
                {
                    var item = _viewModel.CityObjects.FirstOrDefault(c => c.Name == _pendingSuccessMessage);
                    if (item != null)
                    {
                        item.SuccessMessage = "✔ Successfully reserved!";
                        var timer = new System.Windows.Threading.DispatcherTimer();
                        timer.Interval = TimeSpan.FromSeconds(3);
                        timer.Tick += (ts, te) => { timer.Stop(); item.SuccessMessage = string.Empty; };
                        timer.Start();
                    }
                    _pendingSuccessMessage = null;
                }
            }
        };
    }

    public void ShowReservationSuccess(string objectName)
    {
        _pendingSuccessMessage = objectName;
    }

    private void ReserveButton_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.CurrentNeighborhoodId == null) return;
        CityObjectItemViewModel item = (CityObjectItemViewModel)((Button)sender).Tag;
        CoordinatorMainWindow.Instance.NavigateTo(
            new ReserveCityObjectPage(item, _viewModel.CurrentNeighborhoodId.Value),
            "Reserve City Object");
    }
}