using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Services.Neighborhoods;
using CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;
using System.Windows;
using System.Windows.Controls;
using static CommunityHub.Application.DTOs.Neighborhoods.NeighborhoodAccessRequestDto;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class ForumsPage : Page
{
    private readonly long _coordinatorId;
    private readonly ForumsViewModel _viewModel;

    public ForumsPage(long coordinatorId)
    {
        InitializeComponent();
        _coordinatorId = coordinatorId;
        ForumService forumService = Injector.CreateInstance<ForumService>();
        _viewModel = new ForumsViewModel(forumService, coordinatorId);
        DataContext = _viewModel;
    }

    private void OpenForumButton_Click(object sender, RoutedEventArgs e)
    {
        var forumViewModel = (ForumItemViewModel)((Button)sender).Tag;
        CoordinatorMainWindow.Instance.NavigateTo(
            new ForumDetailsPage(forumViewModel.Forum, _coordinatorId), forumViewModel.Title);
    }

    private void NewForumButton_Click(object sender, RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(
            new CreateForumPage(_coordinatorId), "New Forum");
    }
}