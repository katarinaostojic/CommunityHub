namespace CommunityHub.Application.Domain;

public class AppImage
{
    public long Id { get; private set; }
    public string Path { get; private set; }

    public AppImage(long id, string path)
    {
        Id = id;
        Path = path;
    }
}