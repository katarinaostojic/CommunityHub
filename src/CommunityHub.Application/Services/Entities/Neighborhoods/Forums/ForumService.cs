using CommunityHub.Application.Domain.Entities.Neighborhoods.Forums;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.DTOs.Neighborhoods.Forums;
using CommunityHub.Application.Mappings.Neighborhoods;
using static CommunityHub.Application.Domain.Entities.Neighborhoods.Forums.ForumComment;
using static CommunityHub.Application.DTOs.Neighborhoods.NeighborhoodAccessRequestDto;
using CommunityHub.Application.DTOs.Neighborhoods.Forums;
using CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods.Forums;

namespace CommunityHub.Application.Services.Entities.Neighborhoods.Forums;

public class ForumService
{
    private readonly IForumRepository _repository;

    public ForumService(IForumRepository repository)
    {
        _repository = repository;
    }

    public long Create(CreateForumRequest request)
    {
        Forum forum = new Forum(request.Title, request.Description, request.CoordinatorId);
        return _repository.Create(forum);
    }

    public List<ForumDto> GetAll(long currentCoordinatorId)
    {
        return _repository.GetAll(currentCoordinatorId).ToDtoList(currentCoordinatorId);
    }

    public Forum? GetById(long id, long currentCoordinatorId)
    {
        return _repository.GetById(id, currentCoordinatorId);
    }

    public List<ForumCommentDto> GetComments(long forumId, long currentCoordinatorId)
    {
        Forum? forum = _repository.GetById(forumId, currentCoordinatorId);
        if (forum == null) return new List<ForumCommentDto>();
        return forum.Comments.ToDtoList(forum.CoordinatorId);
    }

    public void Close(long forumId,long coordinatorId)
    {
        Forum? forum = _repository.GetById(forumId, coordinatorId);
        if (forum == null) return;
        forum.Close();
        _repository.Update(forum);
    }

    public void AddComment(long forumId, long coordinatorId, string coordinatorName,
        string coordinatorSurname, string neighborhoodName, string text)
    {
        ForumComment comment = new ForumComment(
            0, forumId, coordinatorId, coordinatorName,
            coordinatorSurname, neighborhoodName, text, DateTime.UtcNow, 0, 0, null);
        _repository.CreateComment(comment);
    }

    public void ReactToComment(long commentId, long coordinatorId, ReactionType reaction)
    {
        ReactionType? existing = _repository.GetReaction(commentId, coordinatorId);

        ForumComment comment = new ForumComment(commentId, 0, coordinatorId, "", "", "", "", DateTime.UtcNow, 0, 0, existing);
        ReactionAction action = comment.React(reaction);

        switch (action)
        {
            case ReactionAction.Add:
                _repository.AddReaction(commentId, coordinatorId, reaction);
                break;
            case ReactionAction.Remove:
                _repository.RemoveReaction(commentId, coordinatorId);
                break;
            case ReactionAction.Update:
                _repository.UpdateReaction(commentId, coordinatorId, reaction);
                break;
        }
    }
}