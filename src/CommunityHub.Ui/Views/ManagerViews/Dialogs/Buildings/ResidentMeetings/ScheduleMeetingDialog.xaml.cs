using System.Windows;

namespace CommunityHub.Ui.Views.ManagerViews.Dialogs.Buildings.ResidentMeetings;

public partial class ScheduleMeetingDialog : Window
{
    public DateTime? SelectedDate { get; private set; }
    public TimeSpan? SelectedTime { get; private set; }
    public List<string> Topics { get; private set; } = new();

    public ScheduleMeetingDialog()
    {
        InitializeComponent();

        // Popuni sate 0-23
        for (int h = 0; h < 24; h++)
            HourComboBox.Items.Add(h.ToString("00"));

        // Popuni minute 00 i 30
        MinuteComboBox.Items.Add("00");
        MinuteComboBox.Items.Add("30");

        HourComboBox.SelectedIndex = 18; // default 18h
        MinuteComboBox.SelectedIndex = 0;
    }

    private void ScheduleButton_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateFields()) return;

        SelectedDate = DatePicker.SelectedDate!.Value;
        SelectedTime = new TimeSpan(
            int.Parse(HourComboBox.SelectedItem.ToString()!),
            int.Parse(MinuteComboBox.SelectedItem.ToString()!),
            0);

        Topics = TopicsTextBox.Text
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .ToList();

        DialogResult = true;
        Close();
    }

    private bool ValidateFields()
    {
        if (DatePicker.SelectedDate == null)
        {
            MessageBox.Show("Please select a date.", "Error");
            return false;
        }

        if (HourComboBox.SelectedItem == null || MinuteComboBox.SelectedItem == null)
        {
            MessageBox.Show("Please select a time.", "Error");
            return false;
        }

        var selectedDateTime = DatePicker.SelectedDate.Value.Date.Add(
            new TimeSpan(
                int.Parse(HourComboBox.SelectedItem.ToString()!),
                int.Parse(MinuteComboBox.SelectedItem.ToString()!),
                0));

        if (selectedDateTime <= DateTime.Now.AddHours(24))
        {
            MessageBox.Show("Meeting must be scheduled at least 24 hours in advance.", "Error");
            return false;
        }

        var topics = TopicsTextBox.Text
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .ToList();

        if (topics.Count == 0)
        {
            MessageBox.Show("Please enter at least one topic.", "Error");
            return false;
        }

        return true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}