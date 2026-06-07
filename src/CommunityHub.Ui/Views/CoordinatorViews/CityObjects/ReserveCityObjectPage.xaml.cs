using CommunityHub.Application.Database.Repositories.Neighborhoods;
using CommunityHub.Application.DTOs.Neighborhoods.CityObjects;
using CommunityHub.Application.Services.Entities.Neighborhoods.CityObjects;
using CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;
using System.Windows;
using System.Windows.Controls;
using CommunityHub.Application.Database.Repositories.Neighborhoods.CityObjects;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class ReserveCityObjectPage : Page
{
    private readonly ReserveCityObjectViewModel _viewModel;

    public ReserveCityObjectPage(CityObjectItemViewModel cityObject, long neighborhoodId)
    {
        InitializeComponent();
        var cityObjectService = new CityObjectService(new CityObjectDbRepository());
        _viewModel = new ReserveCityObjectViewModel(cityObject, cityObjectService, neighborhoodId);
        DataContext = _viewModel;
    }

    private void SearchSlotButton_Click(object sender, RoutedEventArgs e)
    {
        string? error = _viewModel.SearchForSlot();
        if (error != null)
        {
            ErrorText.Text = error;
            ErrorBanner.Visibility = Visibility.Visible;
            return;
        }
        ErrorBanner.Visibility = Visibility.Collapsed;
    }

    private void AlternativeSlot_Checked(object sender, RoutedEventArgs e)
    {
        SlotSuggestion slot = (SlotSuggestion)((RadioButton)sender).Tag;
        _viewModel.SelectedSlot = slot;
    }

    private void ConfirmReservationButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _viewModel.ConfirmReservation();
            CoordinatorMainWindow.Instance.GoBack();
        }
        catch (Exception ex)
        {
            ErrorText.Text = ex.Message;
            ErrorBanner.Visibility = Visibility.Visible;
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.GoBack();
    }
}