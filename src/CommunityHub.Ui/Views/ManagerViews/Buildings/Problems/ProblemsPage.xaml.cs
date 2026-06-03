using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Ui.ViewModels.ManagerViewModels.Buildings.ProblemReports;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.ManagerViews.Buildings.Problems;

public partial class ProblemsPage : Page
{
    private readonly User _currentUser;
    private readonly ProblemsViewModel _viewModel;

    public ProblemsPage(User user)
    {
        InitializeComponent();
        _currentUser = user;
        _viewModel = new ProblemsViewModel(_currentUser.Id);
        DataContext = _viewModel;
    }

    private void BuildingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (BuildingComboBox.SelectedItem is BuildingDto building)
        {
            _viewModel.SelectedBuilding = building;
            BuildingPreview.Visibility = Visibility.Visible;
        }
        else
        {
            BuildingPreview.Visibility = Visibility.Collapsed;
        }
    }

    private void MarkAsResolved_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is long id)
        {
            _viewModel.MarkAsResolved(id);
        }
    }
}