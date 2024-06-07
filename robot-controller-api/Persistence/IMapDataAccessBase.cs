using System;
using System.Collections.Generic;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence
{
    public interface IMapDataAccess
    {
        List<Map> GetMaps();
        void UpdateMap(Map updatedMap);
        bool MapExists(string name);

        List<Map> GetSquareMaps();
        void InsertMap(Map newMap);
        bool DeleteMap(int id);
        bool CheckCoordinate(int id, int x, int y);
        Map GetMapById(int id);
    }
}
