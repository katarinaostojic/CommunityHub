using CommunityHub.Application.Domain.Entities.Buildings.Ads;
using CommunityHub.Ui.Helpers.Manager;
using System.Windows;

namespace CommunityHub.Ui.Views.ManagerViews.Dialogs;

public partial class ExportAdsReportDialog : Window
{
    public AdType SelectedType { get; private set; } = AdType.Offering;

    public ExportAdsReportDialog()
    {
        InitializeComponent();
        TooltipsManager.Apply(this);
    }

    private void ExportButton_Click(object sender, RoutedEventArgs e)
    {
        SelectedType = OfferingRadioButton.IsChecked == true
            ? AdType.Offering
            : AdType.Seeking;

        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}