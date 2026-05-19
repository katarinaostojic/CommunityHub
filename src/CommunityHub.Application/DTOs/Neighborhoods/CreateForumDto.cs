namespace CommunityHub.Application.DTOs.Neighborhoods;

public class CreateForumRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public long CoordinatorId { get; set; }

    public CreateForumRequest(string title, string description, long coordinatorId)
    {
        Title = title;
        Description = description;
        CoordinatorId = coordinatorId;
    }
}