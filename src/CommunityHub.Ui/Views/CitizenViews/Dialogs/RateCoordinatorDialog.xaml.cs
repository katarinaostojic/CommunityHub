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
using CommunityHub.Application.Services.Entities.Neighborhoods.Reviews;
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
        RadioButton[] radios = { Radio1, Radio2, Radio3, Radio4, Radio5 };
        for (int i = 0; i < radios.Length; i++)
        {
            if (radios[i].IsChecked == true)
                return i + 1;
        }
        return null;
    }

    private string? GetTrimmedComment()
    {
        return string.IsNullOrWhiteSpace(CommentTextBox.Text)
            ? null : CommentTextBox.Text.Trim();
    }

    private bool ValidateRating(int? rating)
    {
        if (rating != null) return true;
        MessageBox.Show("Molimo odaberite ocenu.", "Greška",
            MessageBoxButton.OK, MessageBoxImage.Warning);
        return false;
    }

    private bool ValidateComment(int rating, string? comment)
    {
        if (rating >= 3 || comment != null) return true;
        CommentRequiredText.Visibility = Visibility.Visible;
        MessageBox.Show("Za ocenu nižu od 3 morate ostaviti komentar.", "Greška",
            MessageBoxButton.OK, MessageBoxImage.Warning);
        return false;
    }

    private void SubmitButton_Click(object sender, RoutedEventArgs e)
    {
        int? rating = GetSelectedRating();
        if (!ValidateRating(rating)) return;

        string? comment = GetTrimmedComment();
        if (!ValidateComment(rating!.Value, comment)) return;

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

