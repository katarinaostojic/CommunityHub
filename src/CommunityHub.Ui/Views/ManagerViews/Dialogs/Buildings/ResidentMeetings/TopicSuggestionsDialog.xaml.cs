using CommunityHub.Application.Domain.Entities.Buildings.ResidentMeetings;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.ManagerViews.Dialogs.Buildings.ResidentMeetings;

public partial class TopicSuggestionsDialog : Window
{
    private readonly Action<string> _onAddTopic;
    private readonly ObservableCollection<ResidentMeetingTopicSuggestion> _suggestions;
    private readonly bool _hadSuggestionsAtStart;
    private bool _addedAtLeastOne = false;

    public TopicSuggestionsDialog(
    List<ResidentMeetingTopicSuggestion> suggestions,
    bool alreadyAddedOne,
    Action<string> onAddTopic)
    {
        InitializeComponent();
        _onAddTopic = onAddTopic;
        _hadSuggestionsAtStart = suggestions.Count > 0;
        _addedAtLeastOne = alreadyAddedOne; // vec je dodana jedna ranije
        _suggestions = new ObservableCollection<ResidentMeetingTopicSuggestion>(suggestions);

        if (!_hadSuggestionsAtStart)
        {
            EmptyText.Visibility = Visibility.Visible;
            SuggestionsList.Visibility = Visibility.Collapsed;
        }
        else
        {
            SuggestionsList.ItemsSource = _suggestions;
        }
    }

    private void AddTopic_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string topic)
        {
            try
            {
                _onAddTopic(topic);
                _addedAtLeastOne = true;

                var item = _suggestions.FirstOrDefault(s => s.Topic == topic);
                if (item != null)
                    _suggestions.Remove(item);

                if (_suggestions.Count == 0)
                {
                    EmptyText.Visibility = Visibility.Visible;
                    SuggestionsList.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        if (_hadSuggestionsAtStart && !_addedAtLeastOne)
        {
            MessageBox.Show(
                "You must add at least one suggested topic before closing.",
                "Warning");
            return;
        }

        Close();
    }
}