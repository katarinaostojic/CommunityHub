namespace CommunityHub.Application.Domain;

public class Country
{
    public long Id { get; private set; }
    public string Name { get; private set; }
    public string Code { get; private set; }


    public Country(long id, string name, string code)
    {
        Id = id;
        Name = name;
        Code = code;
    }

    public Country(string name, string code)
    {
        Id = 0;
        Name = name;
        Code = code;
    }
}
