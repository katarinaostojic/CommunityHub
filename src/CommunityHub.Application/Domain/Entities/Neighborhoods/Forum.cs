namespace CommunityHub.Application.Domain.Entities.Neighborhoods;

public class Forum
{
    public long Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public long CoordinatorId { get; private set; }
    public string CoordinatorName { get; private set; }
    public string CoordinatorSurname { get; private set; }
    public bool IsClosed { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public List<ForumComment> Comments { get; private set; }

    public Forum(long id, string title, string description, long coordinatorId,
        string coordinatorName, string coordinatorSurname, bool isClosed, DateTime createdAt)
    {
        Id = id;
        Title = title;
        Description = description;
        CoordinatorId = coordinatorId;
        CoordinatorName = coordinatorName;
        CoordinatorSurname = coordinatorSurname;
        IsClosed = isClosed;
        CreatedAt = createdAt;
        Comments = new List<ForumComment>();
    }

    public Forum(string title, string description, long coordinatorId)
    {
        Id = 0;
        Title = title;
        Description = description;
        CoordinatorId = coordinatorId;
        CoordinatorName = string.Empty;
        CoordinatorSurname = string.Empty;
        IsClosed = false;
        CreatedAt = DateTime.UtcNow;
        Comments = new List<ForumComment>();
    }

    public void Close() => IsClosed = true;
    public void AddComment(ForumComment comment) => Comments.Add(comment);

    public int CommentsCount { get; private set; }

    public void SetCommentsCount(int count) => CommentsCount = count;
}