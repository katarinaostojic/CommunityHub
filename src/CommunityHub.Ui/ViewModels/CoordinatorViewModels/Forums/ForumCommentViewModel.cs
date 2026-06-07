using CommunityHub.Application.Domain.Entities.Neighborhoods.Forums;
using CommunityHub.Application.DTOs.Neighborhoods;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

public class ForumCommentViewModel
{
    private readonly ForumCommentDto _comment;

    public ForumCommentViewModel(ForumCommentDto comment)
    {
        _comment = comment;
    }

    public long Id => _comment.Id;
    public string CoordinatorFullName => _comment.CoordinatorFullName;
    public string NeighborhoodName => _comment.NeighborhoodName;
    public string Text => _comment.Text;
    public int LikeCount => _comment.LikeCount;
    public int DislikeCount => _comment.DislikeCount;
    public string AuthorLabel => _comment.AuthorLabel;
    public ReactionType? CurrentUserReaction => _comment.CurrentUserReaction;
    public ForumCommentDto Comment => _comment;
}