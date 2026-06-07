using CommunityHub.Application.DTOs.Neighborhoods.Events;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CommunityHub.Ui.Views.CitizenViews.Dialogs;

public partial class JoinEventDialog : Window
{
    private readonly long _citizenId;
    private readonly EventDto _event;
    private readonly EventsViewModel _eventsViewModel;

    public JoinEventDialog(long citizenId, EventDto ev, EventsViewModel eventsViewModel)
    {
        InitializeComponent();
        _citizenId = citizenId;
        _event = ev;
        _eventsViewModel = eventsViewModel;
        LoadData();
    }

    private void LoadData()
    {
        EventNameText.Text = _event.Name;
        EventDateText.Text = _event.EventDateFormatted;
        EventTimeText.Text = _event.StartTimeFormatted;
        EventDurationText.Text = _event.DurationMinutes.ToString();

        var items = _event.Items.Select(i => new JoinEventItemViewModel(i)).ToList();
        ItemsListControl.ItemsSource = items;
    }

    private void JoinButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedItemIds = GetSelectedItemIds();
        _eventsViewModel.RegisterVolunteer(_event.Id, _citizenId, selectedItemIds);

        MessageBox.Show("You have successfully joined the event!", "Success",
            MessageBoxButton.OK, MessageBoxImage.Information);
        Close();
    }

    private List<long> GetSelectedItemIds()
    {
        var selectedIds = new List<long>();

        foreach (var item in ItemsListControl.Items)
        {
            var container = ItemsListControl.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
            if (container == null) continue;

            var checkBox = FindVisualChild<CheckBox>(container);
            if (checkBox != null && checkBox.IsChecked == true)
                selectedIds.Add((long)checkBox.Tag);
        }

        return selectedIds;
    }

    private static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T result) return result;
            var found = FindVisualChild<T>(child);
            if (found != null) return found;
        }
        return null;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();
}