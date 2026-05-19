using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Buildings;
using CommunityHub.Ui.ViewModels.TenantViewModels.Buildings;
using System.Windows;
using CommunityHub.Application.Domain.Shared;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class BuildingAccessRequestDialog : Window
{
    private readonly BuildingAccessRequestDialogViewModel _viewModel;

    public BuildingAccessRequestDialog(BuildingDto building, User user)
    {
        InitializeComponent();

        BuildingAccessRequestService requestService = Injector.CreateInstance<BuildingAccessRequestService>();
        BuildingService buildingService = Injector.CreateInstance<BuildingService>();

        _viewModel = new BuildingAccessRequestDialogViewModel(
            requestService,
            buildingService,
            building,
            user);

        DataContext = _viewModel;
    }

    private void SendRequestButton_Click(object sender, RoutedEventArgs e)
    {
        string? errorMessage = _viewModel.SubmitRequest();

        if (errorMessage != null)
        {
            MessageBox.Show(errorMessage, "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}