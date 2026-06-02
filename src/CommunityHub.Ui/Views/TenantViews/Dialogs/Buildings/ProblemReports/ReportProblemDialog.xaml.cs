using CommunityHub.Application.Domain.Entities.Buildings.ProblemReports;
using CommunityHub.Ui.ViewModels.TenantViewModels.Dialogs.Buildings.ProblemReports;
using System.Windows;

namespace CommunityHub.Ui.Views.TenantViews.Dialogs.Buildings;

public partial class ReportProblemDialog : Window
{
    private readonly ReportProblemDialogViewModel _viewModel;

    public ReportProblemDialog(ReportProblemDialogViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;

        PriorityComboBox.SelectedIndex = 0;
    }

    public string Description => DescriptionTextBox.Text.Trim();

    public ProblemPriority SelectedPriority =>
        _viewModel.GetPriority(PriorityComboBox.SelectedIndex);

    private void SendReportButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_viewModel.Validate(DescriptionTextBox.Text, PriorityComboBox.SelectedIndex))
            return;

        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}