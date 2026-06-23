using CommunityHub.Ui.Views.ManagerViews.Controls;
using System.Windows;
using System.Windows.Controls;

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
            ExplanationBox.PreviewMouseDown += (s2, e2) => OpenKeyboard(ExplanationBox, "Explanation");
        };
    }

    private void OpenKeyboard(TextBox textBox, string fieldName)
    {
        _activeFloating?.Close();
        _activeFloating = new FloatingKeyboardWindow(textBox, this, fieldName);
        _activeFloating.Closed += (_, _) => _activeFloating = null;
        _activeFloating.Show();
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
        _activeFloating?.Close();
        _activeFloating = null;
        DialogResult = false;
        Close();
    }
}