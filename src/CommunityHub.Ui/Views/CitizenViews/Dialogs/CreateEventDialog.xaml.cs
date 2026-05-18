using System.Windows;
using System.Windows.Controls;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;
using System.Collections.ObjectModel;

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

        DateOnly eventDate = DateOnly.FromDateTime(EventDatePicker.SelectedDate!.Value);
        TimeOnly startTime = TimeOnly.Parse(StartTimeTextBox.Text.Trim());
        int duration = int.Parse(DurationTextBox.Text.Trim());
        int minVolunteers = int.Parse(MinVolunteersTextBox.Text.Trim());

        _eventsViewModel.CreateEvent(_organizerId, TitleTextBox.Text.Trim(),
            DescriptionTextBox.Text.Trim(), eventDate, startTime,
            duration, minVolunteers, _items.ToList());

        MessageBox.Show("Event created successfully!", "Success",
            MessageBoxButton.OK, MessageBoxImage.Information);
        Close();
    }

    private bool ValidateInputs()
    {
        if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
        {
            MessageBox.Show("Please enter a title.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
        {
            MessageBox.Show("Please enter a description.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
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
        if (!TimeOnly.TryParse(StartTimeTextBox.Text.Trim(), out _))
        {
            MessageBox.Show("Please enter valid start time (HH:mm).", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        if (!int.TryParse(DurationTextBox.Text.Trim(), out int dur) || dur <= 0)
        {
            MessageBox.Show("Please enter valid duration.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        if (!int.TryParse(MinVolunteersTextBox.Text.Trim(), out int min) || min <= 0)
        {
            MessageBox.Show("Please enter valid minimum volunteers.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        if (_items.Count == 0)
        {
            MessageBox.Show("Please add at least one item.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        return true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();
}