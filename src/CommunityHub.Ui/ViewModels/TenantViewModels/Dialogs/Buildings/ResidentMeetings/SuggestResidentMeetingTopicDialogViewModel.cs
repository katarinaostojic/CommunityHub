using CommunityHub.Application.Domain.Entities.Buildings.ResidentMeetings;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Dialogs.Buildings.ResidentMeetings;

public class SuggestResidentMeetingTopicDialogViewModel : BaseViewModel
{
    private string _topic = string.Empty;
    private string _topicError = string.Empty;
    private bool _hasTopicError;

    public SuggestResidentMeetingTopicDialogViewModel(
        string buildingInfo,
        string meetingDate,
        string meetingTime)
    {
        BuildingInfo = buildingInfo;
        MeetingDate = meetingDate;
        MeetingTime = meetingTime;
    }

    public string BuildingInfo { get; }
    public string MeetingDate { get; }
    public string MeetingTime { get; }

    public string Topic
    {
        get => _topic;
        set
        {
            if (!SetProperty(ref _topic, value))
                return;

            ClearTopicError();
        }
    }

    public string TopicError
    {
        get => _topicError;
        private set => SetProperty(ref _topicError, value);
    }

    public bool HasTopicError
    {
        get => _hasTopicError;
        private set => SetProperty(ref _hasTopicError, value);
    }

    public bool Validate()
    {
        string? error = ResidentMeeting.ValidateTopic(Topic);

        if (error == null)
        {
            ClearTopicError();
            return true;
        }

        TopicError = error;
        HasTopicError = true;
        return false;
    }

    private void ClearTopicError()
    {
        TopicError = string.Empty;
        HasTopicError = false;
    }
}