using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

namespace CommunityHub.Ui.Views.CitizenViews.Dialogs;

public partial class RateCoordinatorDialog : Window
{
    private readonly CoordinatorReviewService _service;
    private readonly long _citizenId;
    private readonly long _coordinatorId;
    private readonly long _neighborhoodId;
    private readonly CoordinatorReviewsViewModel _viewModel;

    public RateCoordinatorDialog(
        CoordinatorReviewService service,
        long citizenId,
        long coordinatorId,
        long neighborhoodId,
        CoordinatorReviewsViewModel viewModel)
    {
        InitializeComponent();
        _service = service;
        _citizenId = citizenId;
        _coordinatorId = coordinatorId;
        _neighborhoodId = neighborhoodId;
        _viewModel = viewModel;
    }

    private int? GetSelectedRating()
    {
        if (Radio1.IsChecked == true) return 1;
        if (Radio2.IsChecked == true) return 2;
        if (Radio3.IsChecked == true) return 3;
        if (Radio4.IsChecked == true) return 4;
        if (Radio5.IsChecked == true) return 5;
        return null;
    }

    private void SubmitButton_Click(object sender, RoutedEventArgs e)
    {
        int? rating = GetSelectedRating();
        if (rating == null)
        {
            MessageBox.Show("Molimo odaberite ocenu.", "Greška",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        string? comment = string.IsNullOrWhiteSpace(CommentTextBox.Text)
            ? null : CommentTextBox.Text.Trim();

        if (rating < 3 && comment == null)
        {
            CommentRequiredText.Visibility = Visibility.Visible;
            MessageBox.Show("Za ocenu nižu od 3 morate ostaviti komentar.", "Greška",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var (success, error) = _service.CreateReview(
            _citizenId, _coordinatorId, _neighborhoodId, rating.Value, comment);

        if (!success)
        {
            MessageBox.Show(error, "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _viewModel.LoadReviews();
        MessageBox.Show("Recenzija je uspešno dodata.", "Uspeh",
            MessageBoxButton.OK, MessageBoxImage.Information);
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();
}
