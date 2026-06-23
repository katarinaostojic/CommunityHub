using CommunityHub.Application.DTOs.Neighborhoods.Events;
using CommunityHub.Ui.Helpers.Citizen;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CommunityHub.Ui.Views.CitizenViews.Dialogs;

public partial class CreateEventDialog : Window
{
    private readonly long _organizerId;
    private readonly long _neighborhoodId;
    private readonly EventsViewModel _eventsViewModel;
    private ObservableCollection<string> _items = new();
    private bool _userStarted = false;

    public CreateEventDialog(long organizerId, long neighborhoodId, EventsViewModel eventsViewModel)
    {
        InitializeComponent();
        _organizerId = organizerId;
        _neighborhoodId = neighborhoodId;
        _eventsViewModel = eventsViewModel;
        ItemsListControl.ItemsSource = _items;
        EventDatePicker.SelectedDate = DateTime.Today;
    }

    private void TitleTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _userStarted = true;
        bool ok = !string.IsNullOrWhiteSpace(TitleTextBox.Text);
        ShowError(TitleError, ok ? null : ResourceHelper.Get("Validate_Required", "⚠ Cannot be empty."));
        Highlight(TitleTextBox, ok);
    }

    private void DescriptionTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        bool ok = !string.IsNullOrWhiteSpace(DescriptionTextBox.Text);
        ShowError(DescriptionError, ok ? null : ResourceHelper.Get("Validate_Required", "⚠ Cannot be empty."));
        Highlight(DescriptionTextBox, ok);
    }

    private void EventDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_userStarted) return;
        bool hasDate = EventDatePicker.SelectedDate.HasValue;
        bool future = hasDate && EventDatePicker.SelectedDate!.Value.Date >= DateTime.Today;
        if (!hasDate) ShowError(DateError, ResourceHelper.Get("Validate_SelectDate", "⚠ Please select a date."));
        else if (!future) ShowError(DateError, ResourceHelper.Get("Validate_PastDate", "⚠ Date cannot be in the past."));
        else ShowError(DateError, null);
    }

    private void StartTimeTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        bool ok = TimeOnly.TryParse(StartTimeTextBox.Text.Trim(), out _);
        ShowError(TimeError, ok || string.IsNullOrWhiteSpace(StartTimeTextBox.Text)
            ? null : ResourceHelper.Get("Validate_TimeFormat", "⚠ Enter valid time (HH:mm), e.g. 14:30."));
        if (!string.IsNullOrWhiteSpace(StartTimeTextBox.Text)) Highlight(StartTimeTextBox, ok);
    }

    private void DurationTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        bool ok = int.TryParse(DurationTextBox.Text.Trim(), out int d) && d > 0;
        ShowError(DurationError, ok || string.IsNullOrWhiteSpace(DurationTextBox.Text)
            ? null : ResourceHelper.Get("Validate_PositiveNumber", "⚠ Enter a positive number."));
        if (!string.IsNullOrWhiteSpace(DurationTextBox.Text)) Highlight(DurationTextBox, ok);
    }

    private void MinVolunteersTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        bool ok = int.TryParse(MinVolunteersTextBox.Text.Trim(), out int m) && m > 0;
        ShowError(MinVolunteersError, ok || string.IsNullOrWhiteSpace(MinVolunteersTextBox.Text)
            ? null : ResourceHelper.Get("Validate_PositiveNumber", "⚠ Enter a positive number."));
        if (!string.IsNullOrWhiteSpace(MinVolunteersTextBox.Text)) Highlight(MinVolunteersTextBox, ok);
    }

    private static void ShowError(TextBlock tb, string? msg)
    {
        if (msg == null) { tb.Visibility = Visibility.Collapsed; return; }
        tb.Text = msg;
        tb.Visibility = Visibility.Visible;
    }

    private static void Highlight(TextBox tb, bool isValid)
    {
        tb.BorderBrush = isValid
            ? new SolidColorBrush(Color.FromRgb(0x43, 0xA0, 0x47))
            : new SolidColorBrush(Color.FromRgb(0xE5, 0x39, 0x35));
        tb.BorderThickness = new Thickness(2);
    }

    private void AddItemButton_Click(object sender, RoutedEventArgs e)
    {
        string name = NewItemTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(name)) return;
        _items.Add(name);
        NewItemTextBox.Text = string.Empty;
        RefreshItemsList();
        ShowError(ItemsError, null);
    }

    private void NewItemTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            AddItemButton_Click(sender, new RoutedEventArgs());
    }

    private void RemoveItemButton_Click(object sender, RoutedEventArgs e)
    {
        string item = (string)((Button)sender).Tag;
        _items.Remove(item);
        RefreshItemsList();
    }

    private void RefreshItemsList()
    {
        ItemsListControl.ItemsSource = null;
        ItemsListControl.ItemsSource = _items;
    }

    private bool ValidateAll()
    {
        _userStarted = true;
        bool ok = true;

        if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
        { ShowError(TitleError, ResourceHelper.Get("Validate_Required", "⚠ Cannot be empty.")); Highlight(TitleTextBox, false); ok = false; }

        if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
        { ShowError(DescriptionError, ResourceHelper.Get("Validate_Required", "⚠ Cannot be empty.")); Highlight(DescriptionTextBox, false); ok = false; }

        if (!EventDatePicker.SelectedDate.HasValue)
        { ShowError(DateError, ResourceHelper.Get("Validate_SelectDate", "⚠ Please select a date.")); ok = false; }
        else if (EventDatePicker.SelectedDate.Value.Date < DateTime.Today)
        { ShowError(DateError, ResourceHelper.Get("Validate_PastDate", "⚠ Date cannot be in the past.")); ok = false; }

        if (!TimeOnly.TryParse(StartTimeTextBox.Text.Trim(), out _))
        { ShowError(TimeError, ResourceHelper.Get("Validate_TimeFormat", "⚠ Enter valid time (HH:mm).")); Highlight(StartTimeTextBox, false); ok = false; }

        if (!int.TryParse(DurationTextBox.Text.Trim(), out int dur) || dur <= 0)
        { ShowError(DurationError, ResourceHelper.Get("Validate_PositiveNumber", "⚠ Enter a positive number.")); Highlight(DurationTextBox, false); ok = false; }

        if (!int.TryParse(MinVolunteersTextBox.Text.Trim(), out int minV) || minV <= 0)
        { ShowError(MinVolunteersError, ResourceHelper.Get("Validate_PositiveNumber", "⚠ Enter a positive number.")); Highlight(MinVolunteersTextBox, false); ok = false; }

        if (_items.Count == 0)
        { ShowError(ItemsError, ResourceHelper.Get("Event_AddAtLeastOne", "⚠ Add at least one item.")); ok = false; }
        else ShowError(ItemsError, null);

        return ok;
    }

    private void CreateButton_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateAll()) return;

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
        MsgHelper.Info("Msg_EventCreated", "Msg_Success");
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();

    private void NumberOnly_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        => e.Handled = !e.Text.All(char.IsDigit);
}
