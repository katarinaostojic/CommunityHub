using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Services.Neighborhoods;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;
using CommunityHub.Ui.Views.CitizenViews.Dialogs;
using CommunityHub.Ui.Views;

namespace CommunityHub.Ui.Views.CitizenViews;

public partial class EventsPage : Window
{
    private readonly User _user;
    private readonly long _neighborhoodId;
    private readonly EventsViewModel _viewModel;

    public EventsPage(User user, long neighborhoodId)
    {
        InitializeComponent();
        _user = user;
        _neighborhoodId = neighborhoodId;

        EventService service = Injector.CreateInstance<EventService>();
        _viewModel = new EventsViewModel(service, neighborhoodId);
        DataContext = _viewModel;

        LoggedInUserTextBlock.Text = _user.Username;
        CitizenMenu.CloseRequested += CitizenMenu_CloseRequested;
        CitizenMenu.NavigationRequested += CitizenMenu_NavigationRequested;
        CitizenMenu.LogoutRequested += CitizenMenu_LogoutRequested;
    }

    private void CreateEventButton_Click(object sender, RoutedEventArgs e)
    {
        CreateEventDialog dialog = new CreateEventDialog(_user, _neighborhoodId, _viewModel);
        dialog.Owner = this;
        dialog.ShowDialog();
    }

    private void JoinButton_Click(object sender, RoutedEventArgs e)
    {
        EventViewModel eventVm = (EventViewModel)((Button)sender).Tag;

        if (_viewModel.IsAlreadyRegistered(eventVm.Event, _user.Id))
        {
            MessageBox.Show("You are already registered for this event.", "Already Registered",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (!eventVm.CanRegister)
        {
            MessageBox.Show("This event is no longer accepting registrations.", "Cannot Join",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        JoinEventDialog dialog = new JoinEventDialog(_user, eventVm.Event, _viewModel);
        dialog.Owner = this;
        dialog.ShowDialog();
    }

    private void BurgerButton_Click(object sender, RoutedEventArgs e)
        => CitizenMenu.Visibility = Visibility.Visible;

    private void CitizenMenu_CloseRequested()
        => CitizenMenu.Visibility = Visibility.Collapsed;

    private void CitizenMenu_NavigationRequested(string destination)
    {
        CitizenMenu.Visibility = Visibility.Collapsed;
        switch (destination)
        {
            case "Neighborhoods":
                new BrowseNeighborhoodPage(_user).Show();
                Close();
                break;
            case "MyRequests":
                new MyRequestsPage(_user).Show();
                Close();
                break;
            case "Events": break;
            case "Citizens": MessageBox.Show("Go to Citizens page."); break;
            case "Meetings": MessageBox.Show("Go to Meetings page."); break;
            case "CityObjects": MessageBox.Show("Go to City Objects page."); break;
            case "Budget": MessageBox.Show("Go to Budget page."); break;
        }
    }

    private void CitizenMenu_LogoutRequested()
    {
        CitizenMenu.Visibility = Visibility.Collapsed;
        new LogInForm().Show();
        Close();
    }
}
