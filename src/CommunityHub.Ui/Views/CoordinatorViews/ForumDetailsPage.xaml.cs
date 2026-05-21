using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Neighborhoods;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Services.Neighborhoods;
using CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;
using System.Windows;
using System.Windows.Controls;
using static CommunityHub.Application.DTOs.Neighborhoods.NeighborhoodAccessRequestDto;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class ForumDetailsPage : Page
{
    private readonly ForumDetailsViewModel _viewModel;
    private readonly long _coordinatorId;

    public ForumDetailsPage(ForumDto forum, long coordinatorId)
    {
        InitializeComponent();
        _coordinatorId = coordinatorId;
        ForumService forumService = Injector.CreateInstance<ForumService>();

        // TODO: get coordinator name and neighborhood from session/injector
        string coordinatorName = "Coordinator";
        string coordinatorSurname = string.Empty;
        string neighborhoodName = "Unknown";

        _viewModel = new ForumDetailsViewModel(forumService, forum, coordinatorId,
            coordinatorName, coordinatorSurname, neighborhoodName);
        DataContext = _viewModel;
    }

    private void SendCommentButton_Click(object sender, RoutedEventArgs e)
    {
        string text = CommentTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(text) || text == "Write a comment...") return;
        _viewModel.AddComment(text);
        CommentTextBox.Text = string.Empty;
    }

    private void LikeButton_Click(object sender, RoutedEventArgs e)
    {
        var commentViewModel = (ForumCommentViewModel)((Button)sender).Tag;
        _viewModel.React(commentViewModel.Id, ReactionType.Like);
    }

    private void DislikeButton_Click(object sender, RoutedEventArgs e)
    {
        var commentViewModel = (ForumCommentViewModel)((Button)sender).Tag;
        _viewModel.React(commentViewModel.Id, ReactionType.Dislike);
    }

    private void CloseForumButton_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.CloseForum();
        CoordinatorMainWindow.Instance.NavigateTo(new ForumsPage(_coordinatorId), "Forums");
    }
}