using CommunityHub.Application.Database.Mappers.Buildings;
using CommunityHub.Application.Domain.Entities.Buildings;
using System.Data;

namespace CommunityHub.Application.Database.Readers.Buildings;

public static class BuildingReader
{
    public static List<Building> ReadBuildings(IDataReader reader)
    {
        Dictionary<long, Building> buildings = new();
        Dictionary<long, Floor> floors = new();
        HashSet<long> addedUnits = new();

        while (reader.Read())
        {
            long buildingId = Convert.ToInt64(reader["id"]);

            if (!buildings.ContainsKey(buildingId))
            {
                buildings[buildingId] = BuildingMapper.Map(reader);
            }

            AddFloorIfMissing(reader, buildings, floors);
            AddUnitIfMissing(reader, floors, addedUnits);
        }

        return buildings.Values.ToList();
    }

    private static void AddFloorIfMissing(
        IDataReader reader,
        Dictionary<long, Building> buildings,
        Dictionary<long, Floor> floors)
    {
        if (reader.IsDBNull(reader.GetOrdinal("floor_id")))
        {
            return;
        }

        long floorId = Convert.ToInt64(reader["floor_id"]);

        if (floors.ContainsKey(floorId))
        {
            return;
        }

        long buildingId = Convert.ToInt64(reader["id"]);
        int floorNumber = Convert.ToInt32(reader["floor_number"]);

        Floor floor = new(floorId, buildings[buildingId], floorNumber);
        floors[floorId] = floor;
        buildings[buildingId].AddFloor(floor);
    }

    private static void AddUnitIfMissing(
        IDataReader reader,
        Dictionary<long, Floor> floors,
        HashSet<long> addedUnits)
    {
        if (reader.IsDBNull(reader.GetOrdinal("floor_id")) ||
            reader.IsDBNull(reader.GetOrdinal("unit_id")))
        {
            return;
        }

        long unitId = Convert.ToInt64(reader["unit_id"]);

        if (addedUnits.Contains(unitId))
        {
            return;
        }

        long floorId = Convert.ToInt64(reader["floor_id"]);
        string unitNumber = reader["unit_number"].ToString()!;

        Unit unit = new(unitId, floors[floorId], unitNumber);
        floors[floorId].AddUnit(unit);
        addedUnits.Add(unitId);
    }
}