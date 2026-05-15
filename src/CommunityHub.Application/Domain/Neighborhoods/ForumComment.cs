namespace CommunityHub.Application.Domain.Neighborhoods;

public class ForumComment
{
    public long Id { get; private set; }
    public long ForumId { get; private set; }
    public long CoordinatorId { get; private set; }
    public string CoordinatorName { get; private set; }
    public string CoordinatorSurname { get; private set; }
    public string NeighborhoodName { get; private set; }
    public string Text { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public int LikeCount { get; private set; }
    public int DislikeCount { get; private set; }
    public ReactionType? CurrentUserReaction { get; private set; }

    public ForumComment(long id, long forumId, long coordinatorId, string coordinatorName,
        string coordinatorSurname, string neighborhoodName, string text, DateTime createdAt,
        int likeCount, int dislikeCount, ReactionType? currentUserReaction)
    {
        Id = id;
        ForumId = forumId;
        CoordinatorId = coordinatorId;
        CoordinatorName = coordinatorName;
        CoordinatorSurname = coordinatorSurname;
        NeighborhoodName = neighborhoodName;
        Text = text;
        CreatedAt = createdAt;
        LikeCount = likeCount;
        DislikeCount = dislikeCount;
        CurrentUserReaction = currentUserReaction;
    }
}