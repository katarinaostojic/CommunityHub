using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Helpers.Tenant.Demo.Building.Ads;

public class NewAdDemoController
{
    private const int MediumPause = 2200;
    private const int LongPause = 3400;
    private const int TypingPause = 120;

    private readonly TextBox _descriptionTextBox;
    private readonly ComboBox _categoryComboBox;
    private readonly DatePicker _dateFromPicker;
    private readonly DatePicker _dateToPicker;
    private readonly Button _seekingButton;
    private readonly Button _stopButton;
    private readonly Action<bool> _returnToNoticeBoard;

    private CancellationTokenSource? _cancellationTokenSource;

    public NewAdDemoController(
        TextBox descriptionTextBox,
        ComboBox categoryComboBox,
        DatePicker dateFromPicker,
        DatePicker dateToPicker,
        Button seekingButton,
        Button stopButton,
        Action<bool> returnToNoticeBoard)
    {
        _descriptionTextBox = descriptionTextBox;
        _categoryComboBox = categoryComboBox;
        _dateFromPicker = dateFromPicker;
        _dateToPicker = dateToPicker;
        _seekingButton = seekingButton;
        _stopButton = stopButton;
        _returnToNoticeBoard = returnToNoticeBoard;
    }

    public async Task StartAsync()
    {
        bool shouldContinueDemo = true;

        _cancellationTokenSource = new CancellationTokenSource();
        _stopButton.Visibility = Visibility.Visible;

        try
        {
            await RunCycleAsync(_cancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            shouldContinueDemo = false;
        }
        finally
        {
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
            _stopButton.Visibility = Visibility.Collapsed;

            _returnToNoticeBoard(shouldContinueDemo);
        }
    }

    public void Stop()
    {
        _cancellationTokenSource?.Cancel();
    }

    private async Task RunCycleAsync(CancellationToken token)
    {
        ResetForm();
        await Task.Delay(MediumPause, token);

        _seekingButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        await Task.Delay(MediumPause, token);

        await TypeDescriptionAsync(token);

        SelectCategoryIfAvailable(1);
        await Task.Delay(MediumPause, token);

        await PickDateAsync(_dateFromPicker, DateTime.Today.AddDays(1), token);
        await PickDateAsync(_dateToPicker, DateTime.Today.AddDays(3), token);
        await Task.Delay(LongPause, token);
    }

    private async Task PickDateAsync(DatePicker datePicker, DateTime date, CancellationToken token)
    {
        datePicker.IsDropDownOpen = true;
        await Task.Delay(MediumPause, token);

        datePicker.SelectedDate = date;
        await Task.Delay(MediumPause, token);

        datePicker.IsDropDownOpen = false;
    }

    private async Task TypeDescriptionAsync(CancellationToken token)
    {
        _descriptionTextBox.Text = string.Empty;
        const string description = "Need help carrying boxes and small furniture.";

        foreach (char character in description)
        {
            _descriptionTextBox.Text += character;
            _descriptionTextBox.CaretIndex = _descriptionTextBox.Text.Length;

            await Task.Delay(TypingPause, token);
        }
    }

    private void SelectCategoryIfAvailable(int index)
    {
        if (_categoryComboBox.Items.Count > index)
            _categoryComboBox.SelectedIndex = index;
    }

    private void ResetForm()
    {
        _descriptionTextBox.Text = string.Empty;
        _dateFromPicker.SelectedDate = null;
        _dateToPicker.SelectedDate = null;

        if (_categoryComboBox.Items.Count > 0)
            _categoryComboBox.SelectedIndex = 0;
    }
}