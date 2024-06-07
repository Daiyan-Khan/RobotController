using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using robot_controller_api.Models;
using System.Collections.Generic;
using System.Linq;

namespace robot_controller_api.Persistence
{
    public class MapEF : RobotContext, IMapDataAccess
    {
        public MapEF(DbContextOptions<RobotContext> options) : base(options)
        {

        }
        private readonly RobotContext _mapContext = new();


        public List<Map> GetSquareMaps()
        {
            // Retrieve square maps using LINQ query
            var squareMaps = _mapContext.Robotmaps
                .Where(m => m.Rows > 0 && m.Columns > 0 && m.Rows == m.Columns)
                .ToList();

            return squareMaps;
        }


        [AllowAnonymous]
        public List<Map> GetMaps() => _mapContext.Robotmaps.ToList();

        public void UpdateMap(Map updatedMap)
        {
            _mapContext.Update(updatedMap);
            _mapContext.SaveChanges();
        }

        public bool MapExists(string name) => _mapContext.Robotmaps.Any(m => m.Name == name);

        public void InsertMap(Map newMap)
        {
            _mapContext.Robotmaps.Add(newMap);
            _mapContext.SaveChanges();
        }

        public bool DeleteMap(int id)
        {
            var map = _mapContext.Robotmaps.Find(id);
            if (map == null)
                return false;

            _mapContext.Robotmaps.Remove(map);
            _mapContext.SaveChanges();
            return true;
        }

        public bool CheckCoordinate(int id, int x, int y)
        {
            var map = _mapContext.Robotmaps.Find(id);
            return map != null && x >= 0 && x < map.Columns && y >= 0 && y < map.Rows;
        }

        public Map GetMapById(int id) => _mapContext.Robotmaps.Find(id);
    }
}
