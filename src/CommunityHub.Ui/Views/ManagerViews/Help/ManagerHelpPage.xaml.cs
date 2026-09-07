using System.Windows;
using System.Windows.Controls;
using CommunityHub.Ui.Views.ManagerViews.Controls;

namespace CommunityHub.Ui.Views.ManagerViews;

public partial class ManagerHelpPage : Page
{
    public record HelpTopic(string Icon, string Title, string Description);

    private readonly List<HelpTopic> _allTopics = new()
    {
        new HelpTopic(
            "🏢",
            "Registering a building",
            "Go to Buildings and click \"Register New Building\". Fill in the address " +
            "and basic info, confirm the number of floors, list the units per floor, " +
            "and optionally add some pictures."),

        new HelpTopic(
            "🛋️",
            "Common rooms and rental requests",
            "Add common rooms to a building from its details page. Rental requests " +
            "are approved automatically when the requested dates are free, or you'll " +
            "be offered alternative dates to choose from."),

        new HelpTopic(
            "📥",
            "Access requests",
            "Tenants asking to join your buildings show up here. Approve to create " +
            "their membership, or reject with an optional explanation."),

        new HelpTopic(
            "📣",
            "Noticeboard and reports",
            "See all tenant ads and noticeboard statistics per building. Use " +
            "\"Export Report\" to get a PDF of currently active ads."),

        new HelpTopic(
            "👥",
            "Resident meetings",
            "Schedule meetings with a date, time and list of topics. Tenants can " +
            "propose extra topics and confirm their attendance."),

        new HelpTopic(
            "⚠️",
            "Problems",
            "Review problems reported by tenants and mark them as resolved once " +
            "fixed. The tenant confirms before it's fully closed."),

        new HelpTopic(
            "⌨️",
            "Typing without a physical keyboard",
            "Click any text field to open an on-screen keyboard near it. Confirm " +
            "your entry with the checkmark."),

        new HelpTopic(
            "💡",
            "Tooltips",
            "Toggle helpful tooltips on or off anytime using the switch at the top " +
            "of the sidebar.")
    };

    private FloatingKeyboardWindow? _activeFloating;

    public ManagerHelpPage()
    {
        InitializeComponent();
        TopicsItemsControl.ItemsSource = _allTopics;

        SearchTextBox.PreviewMouseDown += (s, e) => OpenKeyboard(SearchTextBox, "Search");
    }

    private void OpenKeyboard(TextBox textBox, string fieldName)
    {
        Window owner = Window.GetWindow(this)!;

        _activeFloating?.Close();
        _activeFloating = new FloatingKeyboardWindow(textBox, owner, fieldName);
        _activeFloating.Closed += (_, _) => _activeFloating = null;
        _activeFloating.Show();
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        string query = SearchTextBox.Text.Trim();

        List<HelpTopic> filtered = string.IsNullOrWhiteSpace(query)
            ? _allTopics
            : _allTopics
                .Where(topic =>
                    topic.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    topic.Description.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();

        TopicsItemsControl.ItemsSource = filtered;
        NoResultsText.Visibility = filtered.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }
}