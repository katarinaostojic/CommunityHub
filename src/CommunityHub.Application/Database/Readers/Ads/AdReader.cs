using CommunityHub.Application.Database.Mappers.Ads;
using CommunityHub.Application.Database.Mappers.Users;
using CommunityHub.Application.Domain.Entities.Ads;
using CommunityHub.Application.Domain.Entities.Shared;
using System.Data;

namespace CommunityHub.Application.Database.Readers.Ads;

public static class AdReader
{
    public static List<Ad> ReadAds(IDataReader reader)
    {
        List<Ad> ads = new();

        while (reader.Read())
        {
            ads.Add(ReadAd(reader));
        }

        return ads;
    }

    public static Ad? ReadSingleAd(IDataReader reader)
    {
        if (!reader.Read())
        {
            return null;
        }

        return ReadAd(reader);
    }

    private static Ad ReadAd(IDataReader reader)
    {
        User author = UserMapper.Map(reader);
        return AdMapper.Map(reader, author);
    }
}