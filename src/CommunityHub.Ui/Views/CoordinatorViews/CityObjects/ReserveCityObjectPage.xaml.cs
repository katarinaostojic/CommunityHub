using CommunityHub.Application.Database.Repositories.Neighborhoods;
using CommunityHub.Application.Database.Repositories.Neighborhoods.CityObjects;
using CommunityHub.Application.DTOs.Neighborhoods.CityObjects;
using CommunityHub.Application.Services.Entities.Neighborhoods.CityObjects;
using CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

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

        FromCalendar.BlackoutDates.Add(
        new CalendarDateRange(DateTime.MinValue, DateTime.Today.AddDays(-1)));
        ToCalendar.BlackoutDates.Add(
            new CalendarDateRange(DateTime.MinValue, DateTime.Today.AddDays(-1)));
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
            string objectName = _viewModel.SelectedCityObject.Name;
            _viewModel.ConfirmReservation();

            CoordinatorMainWindow.Instance.Navigated += OnNavigatedBack;
            CoordinatorMainWindow.Instance.GoBack();

            void OnNavigatedBack(object s, System.Windows.Navigation.NavigationEventArgs args)
            {
                CoordinatorMainWindow.Instance.Navigated -= OnNavigatedBack;
                if (args.Content is CityObjectsPage cityObjectsPage)
                    cityObjectsPage.ShowReservationSuccess(objectName);
            }
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