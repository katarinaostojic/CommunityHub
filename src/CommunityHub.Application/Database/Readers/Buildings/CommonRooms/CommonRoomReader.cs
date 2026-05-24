using CommunityHub.Application.Database.Mappers.Buildings.CommonRooms;
using CommunityHub.Application.Domain.Entities.Buildings.CommonRooms;
using System.Data;

namespace CommunityHub.Application.Database.Readers.Buildings.CommonRooms;

public static class CommonRoomReader
{
    public static List<CommonRoom> ReadRooms(IDataReader reader)
    {
        List<CommonRoom> rooms = new();

        while (reader.Read())
        {
            rooms.Add(CommonRoomMapper.Map(reader));
        }

        return rooms;
    }

    public static CommonRoom? ReadSingleRoom(IDataReader reader)
    {
        if (!reader.Read())
        {
            return null;
        }

        return CommonRoomMapper.Map(reader);
    }

    public static List<DateTime> ReadOccupiedDates(IDataReader reader)
    {
        List<DateTime> dates = new();

        while (reader.Read())
        {
            dates.Add(DateTime.Parse(reader["booked_date"].ToString()!));
        }

        return dates;
    }
}