using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using CommunityHub.Ui.Helpers.Manager.Onboarding;

namespace CommunityHub.Ui.Views.ManagerViews.Controls;

public partial class ManagerOnboardingWizard : Window
{
    private record WizardStep(string Icon, string Title, string Description);

    private readonly List<WizardStep> _steps = new()
    {
        new WizardStep(
            "👋",
            "Welcome to BuildingHub",
            "A short tour through the main sections of the Manager Portal. " +
            "You can skip this at any time."),

        new WizardStep(
            "🏢",
            "Buildings",
            "Register new buildings, define their floors and units, upload images, " +
            "and manage common rooms available for residents to rent."),

        new WizardStep(
            "📥",
            "Access Requests",
            "Review requests from tenants who want to join your buildings. " +
            "Approve or reject them, with an optional explanation when rejecting."),

        new WizardStep(
            "📣",
            "Noticeboard",
            "Track tenant ads for offering or seeking help, view statistics by " +
            "category and period, and export a PDF report of the current status."),

        new WizardStep(
            "👥",
            "Resident Meetings",
            "Schedule meetings for your buildings, manage proposed topics, " +
            "and track resident attendance confirmations."),

        new WizardStep(
            "⚠️",
            "Problems",
            "Review problems reported by tenants and mark them as resolved " +
            "once they have been addressed."),

        new WizardStep(
            "✅",
            "You're all set!",
            "If you ever need a reminder, contextual tooltips can be toggled " +
            "on and off from the sidebar, and the Help section has more details.")
    };

    private int _currentStepIndex;

    public ManagerOnboardingWizard()
    {
        InitializeComponent();
        BuildDots();
        ShowStep(0);
    }

    private void BuildDots()
    {
        DotsPanel.Children.Clear();

        for (int i = 0; i < _steps.Count; i++)
        {
            Ellipse dot = new Ellipse
            {
                Width = 8,
                Height = 8,
                Margin = new Thickness(4, 0, 4, 0),
                Fill = new SolidColorBrush(Color.FromRgb(0xDD, 0xE3, 0xEA))
            };

            DotsPanel.Children.Add(dot);
        }
    }

    private void ShowStep(int index)
    {
        _currentStepIndex = index;
        WizardStep step = _steps[index];

        StepIconText.Text = step.Icon;
        StepTitleText.Text = step.Title;
        StepDescriptionText.Text = step.Description;

        UpdateDots();
        UpdateButtons();
    }

    private void UpdateDots()
    {
        for (int i = 0; i < DotsPanel.Children.Count; i++)
        {
            if (DotsPanel.Children[i] is Ellipse dot)
            {
                dot.Fill = i == _currentStepIndex
                    ? new SolidColorBrush(Color.FromRgb(0x29, 0x80, 0xB9))
                    : new SolidColorBrush(Color.FromRgb(0xDD, 0xE3, 0xEA));
            }
        }
    }

    private void UpdateButtons()
    {
        BackButton.Visibility = _currentStepIndex == 0
            ? Visibility.Collapsed
            : Visibility.Visible;

        bool isLastStep = _currentStepIndex == _steps.Count - 1;
        NextButton.Content = isLastStep ? "Get started" : "Next";
        SkipButton.Visibility = isLastStep ? Visibility.Collapsed : Visibility.Visible;
    }

    private void NextButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentStepIndex < _steps.Count - 1)
        {
            ShowStep(_currentStepIndex + 1);
        }
        else
        {
            FinishWizard();
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentStepIndex > 0)
            ShowStep(_currentStepIndex - 1);
    }

    private void SkipButton_Click(object sender, RoutedEventArgs e)
    {
        FinishWizard();
    }

    private void FinishWizard()
    {
        OnboardingState.MarkWizardAsSeen();
        Close();
    }
}