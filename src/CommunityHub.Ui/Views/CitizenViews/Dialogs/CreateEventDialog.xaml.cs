using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.CitizenViews.Dialogs;

public partial class CreateEventDialog : Window
{
    private readonly long _organizerId;
    private readonly long _neighborhoodId;
    private readonly EventsViewModel _eventsViewModel;
    private ObservableCollection<string> _items = new();

    public CreateEventDialog(long organizerId, long neighborhoodId, EventsViewModel eventsViewModel)
    {
        InitializeComponent();
        _organizerId = organizerId;
        _neighborhoodId = neighborhoodId;
        _eventsViewModel = eventsViewModel;
        ItemsListControl.ItemsSource = _items;
    }

    private void AddItemButton_Click(object sender, RoutedEventArgs e)
    {
        AddItemDialog dialog = new AddItemDialog();
        dialog.Owner = this;
        if (dialog.ShowDialog() == true)
        {
            _items.Add(dialog.ItemName);
            ItemsListControl.ItemsSource = null;
            ItemsListControl.ItemsSource = _items;
        }
    }

    private void RemoveItemButton_Click(object sender, RoutedEventArgs e)
    {
        string item = (string)((Button)sender).Tag;
        _items.Remove(item);
        ItemsListControl.ItemsSource = null;
        ItemsListControl.ItemsSource = _items;
    }

    private void CreateButton_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateInputs()) return;

        var req = new CreateEventRequest
        {
            OrganizerId = _organizerId,
            NeighborhoodId = _neighborhoodId,
            Name = TitleTextBox.Text.Trim(),
            Description = DescriptionTextBox.Text.Trim(),
            EventDate = DateOnly.FromDateTime(EventDatePicker.SelectedDate!.Value),
            StartTime = TimeOnly.Parse(StartTimeTextBox.Text.Trim()),
            DurationMinutes = int.Parse(DurationTextBox.Text.Trim()),
            MinVolunteers = int.Parse(MinVolunteersTextBox.Text.Trim()),
            ItemNames = _items.ToList()
        };

        _eventsViewModel.CreateEvent(req);

        MessageBox.Show("Event created successfully!", "Success",
            MessageBoxButton.OK, MessageBoxImage.Information);
        Close();
    }

    private bool ValidateInputs()
    {
        return ValidateTitle()
            && ValidateDescription()
            && ValidateDate()
            && ValidateTime()
            && ValidateDuration()
            && ValidateMinVolunteers()
            && ValidateItems();
    }

    private bool ValidateTitle()
    {
        if (!string.IsNullOrWhiteSpace(TitleTextBox.Text)) return true;
        MessageBox.Show("Please enter a title.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
        return false;
    }

    private bool ValidateDescription()
    {
        if (!string.IsNullOrWhiteSpace(DescriptionTextBox.Text)) return true;
        MessageBox.Show("Please enter a description.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
        return false;
    }

    private bool ValidateDate()
    {
        if (!EventDatePicker.SelectedDate.HasValue)
        {
            MessageBox.Show("Please select a date.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        if (EventDatePicker.SelectedDate.Value.Date < DateTime.Today)
        {
            MessageBox.Show("Date cannot be in the past.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        return true;
    }

    private bool ValidateTime()
    {
        if (TimeOnly.TryParse(StartTimeTextBox.Text.Trim(), out _)) return true;
        MessageBox.Show("Please enter valid start time (HH:mm).", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
        return false;
    }

    private bool ValidateDuration()
    {
        if (int.TryParse(DurationTextBox.Text.Trim(), out int dur) && dur > 0) return true;
        MessageBox.Show("Please enter valid duration.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
        return false;
    }

    private bool ValidateMinVolunteers()
    {
        if (int.TryParse(MinVolunteersTextBox.Text.Trim(), out int min) && min > 0) return true;
        MessageBox.Show("Please enter valid minimum volunteers.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
        return false;
    }

    private bool ValidateItems()
    {
        if (_items.Count > 0) return true;
        MessageBox.Show("Please add at least one item.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
        return false;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();
}