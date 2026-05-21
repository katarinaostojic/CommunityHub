using CommunityHub.Application.Domain.Entities.Neighborhoods;
using CommunityHub.Application.Services;
using System.Windows.Controls;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

public class AddNewMeetingViewModel : BaseViewModel
{
    private readonly MeetingService _meetingService;
    private readonly long _neighborhoodId;

    public AddNewMeetingViewModel(MeetingService meetingService, long neighborhoodId)
    {
        _meetingService = meetingService;
        _neighborhoodId = neighborhoodId;
    }

    public string? Validate(object? selectedThemeItem, DateTime? startDate, DateTime? endDate, string timeText)
    {
        if (selectedThemeItem == null || string.IsNullOrWhiteSpace(GetThemeText(selectedThemeItem)))
            return "Please select or enter a topic.";
        if (startDate == null || endDate == null)
            return "Please select start and end date.";
        if (endDate < startDate)
            return "End date cannot be before start date.";
        if (!TimeOnly.TryParse(timeText, out _))
            return "Please enter a valid time (HH:mm).";
        return null;
    }

    public void CreateMeeting(object selectedThemeItem, TimeOnly meetingTime, DateOnly startDate, DateOnly endDate)
    {
        string themeText = GetThemeText(selectedThemeItem);

        MeetingTheme theme;
        string? customThemeName = null;

        if (themeText.ToLower() == "welcome")
            theme = MeetingTheme.Welcome;
        else if (themeText.ToLower() == "motivation")
            theme = MeetingTheme.Motivation;
        else
        {
            theme = MeetingTheme.Custom;
            customThemeName = themeText;
        }

        Meeting meeting = new Meeting(_neighborhoodId, theme, customThemeName, meetingTime, startDate, endDate);
        _meetingService.CreateMeeting(meeting);
    }

    private string GetThemeText(object selectedThemeItem)
    {
        if (selectedThemeItem is ComboBoxItem comboBoxItem)
            return comboBoxItem.Content.ToString() ?? string.Empty;
        return selectedThemeItem.ToString() ?? string.Empty;
    }
}