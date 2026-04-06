using System.Windows;
using System.Windows.Controls;
using CommunityHub.Application.Database.Repositories;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class MyDistrictsPage : Page
{
    private readonly long _userId;
    private readonly NeighborhoodDbRepository _repository = new();

    public MyDistrictsPage(long userId)
    {
        InitializeComponent();
        _userId = userId;
        LoadDistricts();
    }

    private void LoadDistricts()
    {
        var districts = _repository.GetByCoordinator(_userId);
        DistrictsItemsControl.ItemsSource = districts;
    }

    private void AddDistrictButton_Click(object sender, RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(new RegisterNeighborhoodPage(_userId), "Add New District");
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateToDashboard();
    }
}