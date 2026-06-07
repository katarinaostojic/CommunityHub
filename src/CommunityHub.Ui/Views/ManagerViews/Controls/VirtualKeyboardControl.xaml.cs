using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CommunityHub.Ui.Views.ManagerViews.Controls;

public partial class VirtualKeyboardControl : UserControl
{
    private TextBox? _activeTextBox;

    // 3 stanja: Lower, ShiftOnce, CapsLock
    private enum ShiftMode { Lower, ShiftOnce, CapsLock }
    private ShiftMode _shiftMode = ShiftMode.ShiftOnce;

    public event EventHandler? HideRequested;

    public VirtualKeyboardControl()
    {
        InitializeComponent();
        UpdateShiftVisual();
    }

    public void SetActiveTextBox(TextBox textBox)
    {
        _activeTextBox = textBox;
    }

    private void KeyButton_Click(object sender, RoutedEventArgs e)
    {
        if (_activeTextBox == null) return;
        if (e.OriginalSource is not Button btn) return;

        string key = btn.Content.ToString()!;

        if (key == "⌫")
        {
            if (_activeTextBox.SelectionLength > 0)
                _activeTextBox.SelectedText = "";
            else if (_activeTextBox.Text.Length > 0)
            {
                int caret = _activeTextBox.CaretIndex;
                if (caret > 0)
                {
                    _activeTextBox.Text = _activeTextBox.Text.Remove(caret - 1, 1);
                    _activeTextBox.CaretIndex = caret - 1;
                }
            }
            return;
        }

        if (key == "Space")
        {
            InsertAtCaret(" ");
            return;
        }

        if (key is "Hide ✕" or "⇧" or "ABC" or "?123")
            return;

        // Odredjujemo da li je veliko ili malo
        bool isUpper = _shiftMode != ShiftMode.Lower;
        string toInsert = isUpper ? key.ToUpper() : key.ToLower();
        InsertAtCaret(toInsert);

        // Posle ShiftOnce se vraca na Lower
        if (_shiftMode == ShiftMode.ShiftOnce)
        {
            _shiftMode = ShiftMode.Lower;
            UpdateShiftVisual();
        }
    }

    private void InsertAtCaret(string text)
    {
        if (_activeTextBox == null) return;
        int caret = _activeTextBox.CaretIndex;
        _activeTextBox.Text = _activeTextBox.Text.Insert(caret, text);
        _activeTextBox.CaretIndex = caret + text.Length;
    }

    private void ShiftButton_Click(object sender, RoutedEventArgs e)
    {
        // Klik 1: Lower → ShiftOnce (jedno veliko)
        // Klik 2: ShiftOnce → CapsLock (sve veliko)
        // Klik 3: CapsLock → Lower
        _shiftMode = _shiftMode switch
        {
            ShiftMode.Lower => ShiftMode.ShiftOnce,
            ShiftMode.ShiftOnce => ShiftMode.CapsLock,
            ShiftMode.CapsLock => ShiftMode.Lower,
            _ => ShiftMode.Lower
        };
        UpdateShiftVisual();
    }

    private void UpdateShiftVisual()
    {
        bool showUpper = _shiftMode != ShiftMode.Lower;

        var letterButtons = new[]
        {
            BtnQ, BtnW, BtnE, BtnR, BtnT, BtnY, BtnU, BtnI, BtnO, BtnP,
            BtnA, BtnS, BtnD, BtnF, BtnG, BtnH, BtnJ, BtnK, BtnL,
            BtnZ, BtnX, BtnC, BtnV, BtnB, BtnN, BtnM
        };

        foreach (var btn in letterButtons)
        {
            string letter = btn.Tag?.ToString() ?? btn.Content.ToString()!;
            btn.Tag ??= btn.Content.ToString(); // sacuvaj originalnu vrednost
            btn.Content = showUpper ? letter.ToUpper() : letter.ToLower();
        }

        // Vizuelno stanje Shift dugmeta
        BtnShift.Background = _shiftMode switch
        {
            ShiftMode.Lower => new SolidColorBrush(Color.FromRgb(189, 195, 199)),     // sivo
            ShiftMode.ShiftOnce => new SolidColorBrush(Color.FromRgb(41, 128, 185)),  // plavo
            ShiftMode.CapsLock => new SolidColorBrush(Color.FromRgb(39, 174, 96)),    // zeleno
            _ => Brushes.LightGray
        };
    }

    private void HideButton_Click(object sender, RoutedEventArgs e)
    {
        HideRequested?.Invoke(this, EventArgs.Empty);
    }

    private void SymbolsButton_Click(object sender, RoutedEventArgs e)
    {
        LettersPanel.Visibility = Visibility.Collapsed;
        SymbolsPanel.Visibility = Visibility.Visible;
    }

    private void LettersButton_Click(object sender, RoutedEventArgs e)
    {
        SymbolsPanel.Visibility = Visibility.Collapsed;
        LettersPanel.Visibility = Visibility.Visible;
    }
}