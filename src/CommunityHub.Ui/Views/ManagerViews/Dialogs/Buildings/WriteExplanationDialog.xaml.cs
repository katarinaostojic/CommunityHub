using CommunityHub.Ui.Views.ManagerViews.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CommunityHub.Ui.Views.ManagerViews.Dialogs;

public partial class WriteExplanationDialog : Window
{
    public string ExplanationText => ExplanationBox.Text.Trim();
    private FloatingKeyboardWindow? _activeFloating;

    public WriteExplanationDialog()
    {
        InitializeComponent();

        Loaded += (s, e) =>
        {
            ExplanationBox.PreviewMouseDown += TextBox_Click;
        };
    }

    private void TextBox_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is TextBox tb)
        {
            _activeFloating?.Close();
            _activeFloating = new FloatingKeyboardWindow(tb, this);
            _activeFloating.Closed += (_, _) => _activeFloating = null;
            _activeFloating.Show();
        }
    }

    private void ConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        _activeFloating?.Close();
        _activeFloating = null;
        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}