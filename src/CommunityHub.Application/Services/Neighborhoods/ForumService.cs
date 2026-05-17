using CommunityHub.Application.Domain.Neighborhoods;
using CommunityHub.Application.Domain.Neighborhoods.NeighborhoodRepositoryInterfaces;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Mappings.Neighborhoods;
using static CommunityHub.Application.DTOs.Neighborhoods.NeighborhoodAccessRequestDto;

namespace CommunityHub.Application.Services.Neighborhoods;

public class ForumService
{
    private readonly IForumRepository _repository;

    public ForumService(IForumRepository repository)
    {
        _repository = repository;
    }

    public long Create(string title, string description, long coordinatorId)
    {
        Forum forum = new Forum(title, description, coordinatorId);
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

    public void Close(long forumId)
    {
        _repository.Close(forumId);
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

        if (existing == null)
            _repository.AddReaction(commentId, coordinatorId, reaction);
        else if (existing == reaction)
            _repository.RemoveReaction(commentId, coordinatorId);
        else
            _repository.UpdateReaction(commentId, coordinatorId, reaction);
    }
}