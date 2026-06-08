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

    public event Action<BuildingDto>? BuildingSelected;

    public ProblemsPage(User user, BuildingDto? preselectedBuilding = null)
    {
        InitializeComponent();
        _currentUser = user;
        _viewModel = new ProblemsViewModel(_currentUser.Id);
        DataContext = _viewModel;

        if (preselectedBuilding != null)
        {
            Loaded += (s, e) =>
            {
                BuildingComboBox.SelectedItem = _viewModel.Buildings
                    .FirstOrDefault(b => b.Id == preselectedBuilding.Id);
            };
        }
    }

    private void BuildingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (BuildingComboBox.SelectedItem is BuildingDto building)
        {
            _viewModel.SelectedBuilding = building;
            BuildingPreview.Visibility = Visibility.Visible;
            ProblemsTable.Visibility = Visibility.Visible;
            NoBuildingText.Visibility = Visibility.Collapsed;
            BuildingSelected?.Invoke(building);
            RefreshEmptyState();
        }
        else
        {
            BuildingPreview.Visibility = Visibility.Collapsed;
            ProblemsTable.Visibility = Visibility.Collapsed;
            NoBuildingText.Visibility = Visibility.Visible;
        }
    }

    private void MarkAsResolved_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is long id)
        {
            _viewModel.MarkAsResolved(id);
            RefreshEmptyState();
        }
    }

    private void RefreshEmptyState()
    {
        NoProblemsText.Visibility = _viewModel.Problems.Count == 0
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
}