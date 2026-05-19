using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Neighborhoods;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Services.Neighborhoods;
using CommunityHub.Ui.Converters;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;
using System.Threading.Tasks;
using System.Windows;

namespace CommunityHub.Ui.Views.CitizenViews.Dialogs;

public partial class NeighborhoodAccessRequestDialog : Window
{
    private readonly User _user;
    private readonly NeighborhoodAccessRequestDialogViewModel _viewModel;

    public NeighborhoodAccessRequestDialog(User user, NeighborhoodDto neighborhood)
    {
        InitializeComponent();
        _user = user;

        NeighborhoodAccessRequestService service = Injector.CreateInstance<NeighborhoodAccessRequestService>();
        _viewModel = new NeighborhoodAccessRequestDialogViewModel(service, user.Id, neighborhood);

        NeighborhoodNameText.Text = _viewModel.NeighborhoodName;
        DescriptionText.Text = _viewModel.Description;
        AddressText.Text = _viewModel.AddressText;
        LocationText.Text = _viewModel.Location;

        if (!string.IsNullOrWhiteSpace(_viewModel.ImagePath))
            NeighborhoodImage.Source = ImagePathConverter.LoadImage(_viewModel.ImagePath);

        CloseButton.Click += CloseButton_Click;
        StartRequestFlow();
    }

    private async void StartRequestFlow()
    {
        await Task.Delay(3000);
        AccessRequestResult result = _viewModel.RequestAccess();
        Close();
        HandleRequestResult(result);
    }

    private void HandleRequestResult(AccessRequestResult result)
    {
        switch (result)
        {
            case AccessRequestResult.RequestCreated:
                new NeighborhoodRequestCreatedDialog(_user, _viewModel.NeighborhoodName).ShowDialog();
                break;
            case AccessRequestResult.AlreadyPending:
                MessageBox.Show("You already have a pending request for this neighborhood.", "Request already exists",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                break;
            case AccessRequestResult.AlreadyMember:
                MessageBox.Show("You are already a member of a neighborhood.", "Already a member",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                break;
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
}