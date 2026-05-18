using CommunityHub.Application.Domain.Neighborhoods;

namespace CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods;

public interface IForumRepository
{
    long Create(Forum forum);
    List<Forum> GetAll(long currentCoordinatorId);
    Forum? GetById(long id, long currentCoordinatorId);
    void Close(long forumId);
    long CreateComment(ForumComment comment);
    void AddReaction(long commentId, long coordinatorId, ReactionType reaction);
    void UpdateReaction(long commentId, long coordinatorId, ReactionType reaction);
    void RemoveReaction(long commentId, long coordinatorId);
    ReactionType? GetReaction(long commentId, long coordinatorId);
}