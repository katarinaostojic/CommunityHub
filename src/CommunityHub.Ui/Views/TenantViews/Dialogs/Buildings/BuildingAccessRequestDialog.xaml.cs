using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Entities.Buildings;
using CommunityHub.Ui.ViewModels.TenantViewModels.Dialogs.Buildings;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class BuildingAccessRequestDialog : Window
{
    private readonly BuildingAccessRequestDialogViewModel _viewModel;
    private bool _isDemoSubmit;

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
        if (_isDemoSubmit)
        {
            if (!_viewModel.CanSubmitRequestForDemo())
                return;

            TrySetDialogResult(true);
            Close();
            return;
        }

        if (!_viewModel.SubmitRequest())
            return;

        TrySetDialogResult(true);
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        TrySetDialogResult(false);
        Close();
    }

    public string GetDemoUnitNumber()
    {
        return _viewModel.GetDemoUnitNumber();
    }

    public void SetUnitNumberForDemo(string unitNumber)
    {
        _viewModel.SetUnitNumberForDemo(unitNumber);
    }

    public bool ClickSendRequestForDemo()
    {
        _isDemoSubmit = true;

        try
        {
            SendRequestButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            return true;
        }
        finally
        {
            _isDemoSubmit = false;
        }
    }

    private void TrySetDialogResult(bool result)
    {
        try
        {
            DialogResult = result;
        }
        catch (InvalidOperationException)
        {
            // Demo opens the dialog with Show(), so DialogResult cannot be set.
        }
    }


}