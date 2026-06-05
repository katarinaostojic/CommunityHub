using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Entities.Buildings;
using CommunityHub.Ui.ViewModels.TenantViewModels.Dialogs.Buildings;
using System.Windows;

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
        if (!_viewModel.SubmitRequest())
            return;

        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}