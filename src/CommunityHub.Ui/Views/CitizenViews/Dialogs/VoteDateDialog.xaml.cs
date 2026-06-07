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
using System.Collections.Generic;
using CommunityHub.Application.DTOs.Neighborhoods.Meetings;

namespace CommunityHub.Ui.Views.CitizenViews.Dialogs;

public partial class VoteDateDialog : Window
{
    private readonly MeetingDto _meeting;
    private readonly List<string> _dates = new();
    public DateOnly SelectedDate { get; private set; }

    public VoteDateDialog(MeetingDto meeting)
    {
        InitializeComponent();
        _meeting = meeting;
        MeetingTimeText.Text = meeting.MeetingTime;
        LoadDates();
    }

    private void LoadDates()
    {
        DateOnly start = DateOnly.ParseExact(_meeting.DateRangeStart, "dd/MM/yyyy");
        DateOnly end = DateOnly.ParseExact(_meeting.DateRangeEnd, "dd/MM/yyyy");

        DateOnly current = start;
        while (current <= end)
        {
            _dates.Add(current.ToString("dd/MM/yyyy"));
            current = current.AddDays(1);
        }

        DatesListControl.ItemsSource = _dates;
    }

    private void VoteButton_Click(object sender, RoutedEventArgs e)
    {
        string? selectedDateStr = null;

        foreach (var item in DatesListControl.Items)
        {
            var container = DatesListControl.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
            if (container == null) continue;

            var radioButton = FindVisualChild<RadioButton>(container);
            if (radioButton != null && radioButton.IsChecked == true)
            {
                selectedDateStr = item.ToString();
                break;
            }
        }

        if (selectedDateStr == null)
        {
            MessageBox.Show("Please select a date.", "Validation",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        SelectedDate = DateOnly.ParseExact(selectedDateStr, "dd/MM/yyyy");
        DialogResult = true;
        Close();
    }

    private static T? FindVisualChild<T>(System.Windows.DependencyObject parent) where T : System.Windows.DependencyObject
    {
        for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
            if (child is T result) return result;
            var found = FindVisualChild<T>(child);
            if (found != null) return found;
        }
        return null;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
