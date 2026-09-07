using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CommunityHub.Application.Services.Entities.Neighborhoods.Reviews;
using CommunityHub.Ui.Helpers.Citizen;
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
            if (radios[i].IsChecked == true)
                return i + 1;
        return null;
    }

    private string? GetTrimmedComment() =>
        string.IsNullOrWhiteSpace(CommentTextBox.Text) ? null : CommentTextBox.Text.Trim();

    private void Rating_Checked(object sender, RoutedEventArgs e)
    {
        RatingErrorText.Visibility = Visibility.Collapsed;

        int rating = GetSelectedRating() ?? 0;
        if (rating < 3)
        {
            CommentRequiredText.Text = ResourceHelper.Get("Rate_CommentRequired", "* Required for ratings below 3");
            CommentRequiredText.Visibility = Visibility.Visible;
        }
        else
        {
            CommentRequiredText.Visibility = Visibility.Collapsed;
            CommentErrorText.Visibility = Visibility.Collapsed;
            CommentTextBox.BorderBrush = (Brush)FindResource("BorderBrush");
            CommentTextBox.BorderThickness = new Thickness(1);
        }
    }

    private void CommentTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        int? rating = GetSelectedRating();
        if (rating == null || rating >= 3) return;

        bool hasComment = !string.IsNullOrWhiteSpace(CommentTextBox.Text);
        if (hasComment)
        {
            CommentErrorText.Visibility = Visibility.Collapsed;
            CommentTextBox.BorderBrush = new SolidColorBrush(Color.FromRgb(0x43, 0xA0, 0x47));
            CommentTextBox.BorderThickness = new Thickness(2);
        }
        else
        {
            CommentTextBox.BorderBrush = new SolidColorBrush(Color.FromRgb(0xE5, 0x39, 0x35));
            CommentTextBox.BorderThickness = new Thickness(2);
        }
    }

    private void SubmitButton_Click(object sender, RoutedEventArgs e)
    {
        int? rating = GetSelectedRating();

        if (rating == null)
        {
            RatingErrorText.Text = ResourceHelper.Get("Msg_SelectRating", "Please select a rating.");
            RatingErrorText.Visibility = Visibility.Visible;
            return;
        }

        string? comment = GetTrimmedComment();

        if (rating < 3 && comment == null)
        {
            CommentErrorText.Text = ResourceHelper.Get("Msg_CommentRequired", "Comment is required for ratings below 3.");
            CommentErrorText.Visibility = Visibility.Visible;
            CommentTextBox.BorderBrush = new SolidColorBrush(Color.FromRgb(0xE5, 0x39, 0x35));
            CommentTextBox.BorderThickness = new Thickness(2);
            return;
        }

        var (success, error) = _service.CreateReview(
            _citizenId, _coordinatorId, _neighborhoodId, rating.Value, comment);

        if (!success)
        {
            MessageBox.Show(error, MsgHelper.Get("Msg_Error", "Error"), MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _viewModel.LoadReviews();
        MsgHelper.Info("Msg_ReviewAdded", "Msg_Success");
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();
}
