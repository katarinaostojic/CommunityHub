using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.ManagerViews.Controls;

public partial class FloatingKeyboardWindow : Window
{
    private readonly TextBox _targetTextBox;

    public FloatingKeyboardWindow(TextBox targetTextBox, Window owner)
    {
        InitializeComponent();
        _targetTextBox = targetTextBox;

        FloatingTextBox.Text = targetTextBox.Text;
        FloatingTextBox.CaretIndex = FloatingTextBox.Text.Length;

        VirtualKeyboard.SetActiveTextBox(FloatingTextBox);
        VirtualKeyboard.HideRequested += (s, e) => ConfirmAndClose();

        Owner = owner;

        Loaded += (s, e) =>
        {
            Left = (SystemParameters.WorkArea.Width - ActualWidth) / 2;
            Top = (SystemParameters.WorkArea.Height - ActualHeight) / 2;
        };

        FloatingTextBox.TextChanged += (s, e) => _targetTextBox.Text = FloatingTextBox.Text;
    }

    private void Confirm_Click(object sender, RoutedEventArgs e)
    {
        ConfirmAndClose();
    }

    private void ConfirmAndClose()
    {
        _targetTextBox.Text = FloatingTextBox.Text;
        _targetTextBox.CaretIndex = _targetTextBox.Text.Length;
        Close();
    }
}