using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Services.Entities.Buildings.CommonRooms;
using System.Windows;

namespace CommunityHub.Ui.Views.ManagerViews.Dialogs;

public partial class CommonRoomCalendarDialog : Window
{
    private List<DateTime> _occupiedDates = new();
    private DateTime _currentMonth;

    public CommonRoomCalendarDialog(long commonRoomId, string roomName)
    {
        InitializeComponent();
        RoomTitleText.Text = roomName;
        _currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        LoadOccupiedDates(commonRoomId);
        RenderCalendar();
    }

    private void LoadOccupiedDates(long commonRoomId)
    {
        CommonRoomService service = Injector.CreateInstance<CommonRoomService>();
        _occupiedDates = service.GetOccupiedDates(commonRoomId);
    }

    private void RenderCalendar()
    {
        MonthYearText.Text = _currentMonth.ToString("MMMM yyyy");

        var calendarDays = new List<CalendarDayItem>();
        AddEmptyDays(calendarDays);
        AddMonthDays(calendarDays);

        CalendarItemsControl.ItemsSource = calendarDays;
    }

    private void AddEmptyDays(List<CalendarDayItem> calendarDays)
    {
        int firstDayOfWeek = (int)_currentMonth.DayOfWeek;
        firstDayOfWeek = firstDayOfWeek == 0 ? 6 : firstDayOfWeek - 1;

        for (int i = 0; i < firstDayOfWeek; i++)
            calendarDays.Add(new CalendarDayItem { Day = "", Color = "Transparent" });
    }

    private void AddMonthDays(List<CalendarDayItem> calendarDays)
    {
        int daysInMonth = DateTime.DaysInMonth(_currentMonth.Year, _currentMonth.Month);
        for (int day = 1; day <= daysInMonth; day++)
        {
            DateTime date = new DateTime(_currentMonth.Year, _currentMonth.Month, day);
            calendarDays.Add(new CalendarDayItem
            {
                Day = day.ToString(),
                Color = GetDayColor(date)
            });
        }
    }

    private string GetDayColor(DateTime date)
    {
        if (_occupiedDates.Any(d => d.Date == date.Date)) return "#E74C3C";
        if (date.Date == DateTime.Today) return "#2980B9";
        return "#27AE60";
    }

    private void PrevMonth_Click(object sender, RoutedEventArgs e)
    {
        _currentMonth = _currentMonth.AddMonths(-1);
        RenderCalendar();
    }

    private void NextMonth_Click(object sender, RoutedEventArgs e)
    {
        _currentMonth = _currentMonth.AddMonths(1);
        RenderCalendar();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}

public class CalendarDayItem
{
    public string Day { get; set; } = "";
    public string Color { get; set; } = "";
}