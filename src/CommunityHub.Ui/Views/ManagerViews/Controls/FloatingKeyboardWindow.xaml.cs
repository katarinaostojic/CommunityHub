using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CommunityHub.Ui.Views.ManagerViews.Controls;

public partial class FloatingKeyboardWindow : Window
{
    private readonly TextBox? _targetTextBox;
    private readonly PasswordBox? _targetPasswordBox;
    private readonly FrameworkElement _targetElement;
    private readonly Window _owner;

    public FloatingKeyboardWindow(TextBox targetTextBox, Window owner, string? fieldName = null)
    {
        InitializeComponent();
        _targetTextBox = targetTextBox;
        _targetElement = targetTextBox;
        _owner = owner;

        FloatingTextBox.Text = targetTextBox.Text;
        FloatingTextBox.CaretIndex = FloatingTextBox.Text.Length;

        Initialize(owner, fieldName);

        FloatingTextBox.TextChanged += (s, e) => _targetTextBox.Text = FloatingTextBox.Text;
    }

    // Konstruktor za PasswordBox - npr. polje za lozinku na LogInForm-i
    public FloatingKeyboardWindow(PasswordBox targetPasswordBox, Window owner, string? fieldName = null)
    {
        InitializeComponent();
        _targetPasswordBox = targetPasswordBox;
        _targetElement = targetPasswordBox;
        _owner = owner;

        // Maskiramo unos i u plutajucem polju, da se lozinka ne vidi na ekranu
        FloatingTextBox.Text = targetPasswordBox.Password;
        FloatingTextBox.CaretIndex = FloatingTextBox.Text.Length;

        Initialize(owner, fieldName);

        FloatingTextBox.TextChanged += (s, e) => _targetPasswordBox.Password = FloatingTextBox.Text;
    }

    private void Initialize(Window owner, string? fieldName)
    {
        VirtualKeyboard.SetActiveTextBox(FloatingTextBox);
        VirtualKeyboard.HideRequested += (s, e) => ConfirmAndClose();

        Owner = owner;

        if (!string.IsNullOrWhiteSpace(fieldName))
        {
            FieldNameTextBlock.Text = fieldName;
            FieldNameTextBlock.Visibility = Visibility.Visible;
        }

        // Tastatura se pozicionira odmah ispod polja na koje je kliknuto.
        // Ako nema mesta ispod (npr. polje je pri dnu ekrana), prikazuje se iznad polja.
        Loaded += (s, e) => PositionNearTarget();
    }

    private void PositionNearTarget()
    {
        const double margin = 30;
        var workArea = SystemParameters.WorkArea;

        GeneralTransform transform = _targetElement.TransformToAncestor(_owner);
        Point topLeftInOwner = transform.Transform(new Point(0, 0));
        Point bottomLeftInOwner = transform.Transform(new Point(0, _targetElement.ActualHeight));

        double targetTop = _owner.Top + topLeftInOwner.Y;
        double targetBottom = _owner.Top + bottomLeftInOwner.Y;
        double targetLeft = _owner.Left + topLeftInOwner.X;

        double spaceBelow = workArea.Bottom - targetBottom;
        double spaceAbove = targetTop - workArea.Top;

        if (spaceBelow >= ActualHeight + margin || spaceBelow >= spaceAbove)
        {
            // Ima mesta ispod polja (ili ima vise mesta ispod nego iznad)
            Top = Math.Min(targetBottom + margin, workArea.Bottom - ActualHeight - margin);
        }
        else
        {
            // Nema mesta ispod - prikazi tastaturu iznad polja
            Top = Math.Max(targetTop - ActualHeight - margin, workArea.Top + margin);
        }

        // Horizontalno: poravnaj sa poljem, ali ostani unutar radne površine
        double left = targetLeft;
        if (left + ActualWidth > workArea.Right)
            left = workArea.Right - ActualWidth - margin;
        if (left < workArea.Left)
            left = workArea.Left + margin;

        Left = left;
    }

    private void Confirm_Click(object sender, RoutedEventArgs e)
    {
        ConfirmAndClose();
    }

    private void ConfirmAndClose()
    {
        if (_targetTextBox != null)
        {
            _targetTextBox.Text = FloatingTextBox.Text;
            _targetTextBox.CaretIndex = _targetTextBox.Text.Length;
        }
        else if (_targetPasswordBox != null)
        {
            _targetPasswordBox.Password = FloatingTextBox.Text;
        }

        Close();
    }
}