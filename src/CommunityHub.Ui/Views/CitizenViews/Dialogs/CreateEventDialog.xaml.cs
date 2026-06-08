using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CommunityHub.Application.DTOs.Neighborhoods.Events;

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

    private static void ShowValidationError(string message)
        => MessageBox.Show(message, "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);

    private void RefreshItemsList()
    {
        ItemsListControl.ItemsSource = null;
        ItemsListControl.ItemsSource = _items;
    }

    private void AddItemButton_Click(object sender, RoutedEventArgs e)
    {
        AddItemDialog dialog = new AddItemDialog();
        dialog.Owner = this;
        if (dialog.ShowDialog() == true)
        {
            _items.Add(dialog.ItemName);
            RefreshItemsList();
        }
    }

    private void RemoveItemButton_Click(object sender, RoutedEventArgs e)
    {
        string item = (string)((Button)sender).Tag;
        _items.Remove(item);
        RefreshItemsList();
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
        ShowValidationError("Please enter a title.");
        return false;
    }

    private bool ValidateDescription()
    {
        if (!string.IsNullOrWhiteSpace(DescriptionTextBox.Text)) return true;
        ShowValidationError("Please enter a description.");
        return false;
    }

    private bool ValidateDate()
    {
        if (!EventDatePicker.SelectedDate.HasValue)
        {
            ShowValidationError("Please select a date.");
            return false;
        }
        if (EventDatePicker.SelectedDate.Value.Date < DateTime.Today)
        {
            ShowValidationError("Date cannot be in the past.");
            return false;
        }
        return true;
    }

    private bool ValidateTime()
    {
        if (TimeOnly.TryParse(StartTimeTextBox.Text.Trim(), out _)) return true;
        ShowValidationError("Please enter valid start time (HH:mm).");
        return false;
    }

    private bool ValidateDuration()
    {
        if (int.TryParse(DurationTextBox.Text.Trim(), out int dur) && dur > 0) return true;
        ShowValidationError("Please enter valid duration.");
        return false;
    }

    private bool ValidateMinVolunteers()
    {
        if (int.TryParse(MinVolunteersTextBox.Text.Trim(), out int min) && min > 0) return true;
        ShowValidationError("Please enter valid minimum volunteers.");
        return false;
    }

    private bool ValidateItems()
    {
        if (_items.Count > 0) return true;
        ShowValidationError("Please add at least one item.");
        return false;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();

    private void NumberOnly_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
        e.Handled = !e.Text.All(char.IsDigit);
    }
}