using System.Windows.Controls;

namespace CommunityHub.Ui.Helpers.Tenant.Demo;

public class BrowseBuildingsSearchDemoPlayer
{
    private const int MediumPause = 1400;
    private const int LongPause = 2600;
    private const int TypingPause = 450;

    private readonly TenantDemoRunner _demoRunner;
    private readonly BrowseBuildingsDemoContext _context;

    public BrowseBuildingsSearchDemoPlayer(
        TenantDemoRunner demoRunner,
        BrowseBuildingsDemoContext context)
    {
        _demoRunner = demoRunner;
        _context = context;
    }

    public async Task ShowAsync(CancellationToken token)
    {
        await ShowSearchDemoAsync(token);
        await ShowFilterDemoAsync(token);
    }

    private async Task ShowSearchDemoAsync(CancellationToken token)
    {
        await TypeSearchTextAsync("knez", token);
        await _demoRunner.DelayAsync(token, LongPause);

        _context.SearchTextBox.Text = string.Empty;
        _context.SearchAllBuildings();

        await _demoRunner.DelayAsync(token, MediumPause);
    }

    private async Task ShowFilterDemoAsync(CancellationToken token)
    {
        _context.FilterPanel.Open();
        await _demoRunner.DelayAsync(token, MediumPause);

        await TypeTextAsync(_context.StreetTextBox, "knez", token);
        await TypeTextAsync(_context.NeighborhoodTextBox, "ari", token);

        _context.ViewModel.Search(
            BrowseBuildingsDemoContext.OptionalText(_context.StreetTextBox),
            BrowseBuildingsDemoContext.OptionalText(_context.NeighborhoodTextBox),
            null,
            null);

        await _demoRunner.DelayAsync(token, LongPause);

        _context.FilterPanel.Close();
        await _demoRunner.DelayAsync(token, MediumPause);
    }

    private async Task TypeSearchTextAsync(string text, CancellationToken token)
    {
        _context.SearchTextBox.Text = string.Empty;

        foreach (char character in text)
        {
            _context.SearchTextBox.Text += character;
            _context.SearchTextBox.CaretIndex = _context.SearchTextBox.Text.Length;
            _context.SearchByMainText();

            await _demoRunner.DelayAsync(token, TypingPause);
        }
    }

    private async Task TypeTextAsync(TextBox textBox, string text, CancellationToken token)
    {
        textBox.Text = string.Empty;

        foreach (char character in text)
        {
            textBox.Text += character;
            textBox.CaretIndex = textBox.Text.Length;

            await _demoRunner.DelayAsync(token, TypingPause);
        }
    }
}