using System.Windows;
using System.Windows.Controls;
using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Services.Entities.Neighborhoods.Forums;
using CommunityHub.Application.DTOs.Neighborhoods.Forums;

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

        TitleErrorText.Visibility = Visibility.Collapsed;
        DescriptionErrorText.Visibility = Visibility.Collapsed;

        bool isValid = true;

        if (string.IsNullOrWhiteSpace(title))
        {
            TitleErrorText.Text = "Please enter a title.";
            TitleErrorText.Visibility = Visibility.Visible;
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            DescriptionErrorText.Text = "Please enter a description.";
            DescriptionErrorText.Visibility = Visibility.Visible;
            isValid = false;
        }

        if (!isValid) return;

        _forumService.Create(new CreateForumRequest(title, description, _coordinatorId));
        CoordinatorMainWindow.Instance.NavigateTo(new ForumsPage(_coordinatorId), "Forums");
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(new ForumsPage(_coordinatorId), "Forums");
    }
}