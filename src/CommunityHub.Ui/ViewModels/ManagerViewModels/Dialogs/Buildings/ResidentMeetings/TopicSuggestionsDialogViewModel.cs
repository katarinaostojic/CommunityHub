using CommunityHub.Ui.ViewModels.ManagerViewModels.Buildings.ResidentMeetings;
using CommunityHub.Application.DTOs.Buildings.ResidentMeetings;
using System.Collections.ObjectModel;
using System.Windows;

namespace CommunityHub.Ui.ViewModels.ManagerViewModels.Dialogs.Buildings.ResidentMeetings;

public class TopicSuggestionsDialogViewModel : BaseViewModel
{
    private readonly ResidentMeetingsViewModel _parentViewModel;
    private readonly long _meetingId;
    private bool _addedAtLeastOne;

    private ObservableCollection<ResidentMeetingTopicSuggestionDto> _suggestions = new();
    public ObservableCollection<ResidentMeetingTopicSuggestionDto> Suggestions
    {
        get => _suggestions;
        private set => SetProperty(ref _suggestions, value);
    }

    public bool HadSuggestionsAtStart { get; }

    public bool HasSuggestions => Suggestions.Count > 0;

    public bool CanClose => !HadSuggestionsAtStart || _addedAtLeastOne;

    private readonly bool _canChangTopics;

    public TopicSuggestionsDialogViewModel(
        ResidentMeetingsViewModel parentViewModel,
        ResidentMeetingRowViewModel meeting)
    {
        _parentViewModel = parentViewModel;
        _meetingId = meeting.Id;
        _canChangTopics = DateTime.Now < meeting.DeadlineAt;

        var allSuggestions = parentViewModel.GetTopicSuggestions(meeting.Id);

        var filtered = allSuggestions
            .Where(s => !meeting.Topics.Contains(s.Topic))
            .ToList();

        HadSuggestionsAtStart = filtered.Count > 0 && _canChangTopics;
        _addedAtLeastOne = allSuggestions.Any(s => meeting.Topics.Contains(s.Topic));
        Suggestions = new ObservableCollection<ResidentMeetingTopicSuggestionDto>(filtered);
    }

    public void AddTopic(string topic)
    {
        try
        {
            _parentViewModel.AddTopicFromSuggestion(_meetingId, topic);
            _addedAtLeastOne = true;

            var item = Suggestions.FirstOrDefault(s => s.Topic == topic);
            if (item != null)
                Suggestions.Remove(item);

            OnPropertyChanged(nameof(HasSuggestions));
            OnPropertyChanged(nameof(CanClose));
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error");
        }
    }
}