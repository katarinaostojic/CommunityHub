using System.Windows;
using System.Windows.Controls;
using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Services.Neighborhoods;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class CreateForumPage : Page
{
    private readonly long _coordinatorId;
    private readonly ForumService _forumService;

    public CreateForumPage(long coordinatorId)
    {
        InitializeComponent();
        _coordinatorId = coordinatorId;
        _forumService = Injector.CreateInstance<ForumService>();
        DescriptionTextBox.TextChanged += DescriptionTextBox_TextChanged;
    }

    private void DescriptionTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        CharCountText.Text = $"{DescriptionTextBox.Text.Length}/500";
    }

    private void CreateForumButton_Click(object sender, RoutedEventArgs e)
    {
        string title = TitleTextBox.Text.Trim();
        string description = DescriptionTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(title))
        {
            MessageBox.Show("Please enter a title.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            MessageBox.Show("Please enter a description.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _forumService.Create(new CreateForumRequest(title, description, _coordinatorId));
        MessageBox.Show("Forum created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        CoordinatorMainWindow.Instance.NavigateTo(new ForumsPage(_coordinatorId), "Forums");
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(new ForumsPage(_coordinatorId), "Forums");
    }
}