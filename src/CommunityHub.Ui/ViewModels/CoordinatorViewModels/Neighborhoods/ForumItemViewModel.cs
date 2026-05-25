using CommunityHub.Application.DTOs.Neighborhoods;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

public class ForumItemViewModel
{
    private readonly ForumDto _forum;

    public ForumItemViewModel(ForumDto forum)
    {
        _forum = forum;
    }

    public long Id => _forum.Id;
    public string Title => _forum.Title;
    public string CoordinatorFullName => _forum.CoordinatorFullName;
    public string CreatedAtFormatted => _forum.CreatedAtFormatted;
    public bool IsAuthor => _forum.IsAuthor;
    public ForumDto Forum => _forum;

    public int CommentsCount => _forum.CommentsCount;
    public string CommentsDisplay => $"Comments: {CommentsCount}";
}