using CommunityHub.Application.Domain.Neighborhoods;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Services.Neighborhoods;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

public class ForumDetailsViewModel : BaseViewModel
{
    private readonly ForumService _forumService;
    private readonly long _coordinatorId;
    private readonly string _coordinatorName;
    private readonly string _coordinatorSurname;
    private readonly string _neighborhoodName;

    private ObservableCollection<ForumCommentViewModel> _comments = new();
    private ForumDto _forum;

    public ForumDetailsViewModel(ForumService forumService, ForumDto forum, long coordinatorId,
        string coordinatorName, string coordinatorSurname, string neighborhoodName)
    {
        _forumService = forumService;
        _forum = forum;
        _coordinatorId = coordinatorId;
        _coordinatorName = coordinatorName;
        _coordinatorSurname = coordinatorSurname;
        _neighborhoodName = neighborhoodName;
        LoadComments();
    }

    public ForumDto Forum => _forum;

    public ObservableCollection<ForumCommentViewModel> Comments
    {
        get => _comments;
        private set => SetProperty(ref _comments, value);
    }

    public bool IsAuthor => _forum.CoordinatorId == _coordinatorId;
    public bool CanClose => IsAuthor && !_forum.IsClosed;

    public void AddComment(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        _forumService.AddComment(_forum.Id, _coordinatorId, _coordinatorName,
            _coordinatorSurname, _neighborhoodName, text);
        LoadComments();
    }

    public void React(long commentId, ReactionType reaction)
    {
        _forumService.ReactToComment(commentId, _coordinatorId, reaction);
        LoadComments();
    }

    public void CloseForum()
    {
        _forumService.Close(_forum.Id);
    }

    private void LoadComments()
    {
        var comments = _forumService.GetComments(_forum.Id, _coordinatorId)
            .Select(c => new ForumCommentViewModel(c))
            .ToList();
        Comments = new ObservableCollection<ForumCommentViewModel>(comments);
    }
}