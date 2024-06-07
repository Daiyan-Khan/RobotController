using Npgsql;
using NpgsqlTypes;
using robot_controller_api.Models;
namespace robot_controller_api.Persistence
{
    public class MapRepository : IMapDataAccess,IRepository
    {
        private IRepository _repo => this;

        public List<Map> GetMaps()
        {
            var maps = _repo.ExecuteReader<Map>("SELECT * FROM robotmap");
            return maps;
        }

        public void UpdateMap(Map updateMap)
        {
            var sqlParams = new NpgsqlParameter[]
            {
                new NpgsqlParameter("id", updateMap.Id),
                new NpgsqlParameter("rows", updateMap.Rows),
                new NpgsqlParameter("columns", updateMap.Columns),
                new NpgsqlParameter("name", updateMap.Name),
                new NpgsqlParameter("description", updateMap.Description ?? (object)DBNull.Value),
            };

            var result = _repo.ExecuteReader<RobotCommand>(
            @"UPDATE robotmap 
              SET name=@name, rows = @rows, columns = @columns, description=@description, modifieddate=current_timestamp 
              WHERE id=@id 
              RETURNING *;",
            sqlParams).Single();
        }

        public bool DeleteMap(int id)
        {
            var sqlParams = new NpgsqlParameter[]
            {
            new NpgsqlParameter("id", id)
            };
            // Implementing DeleteRobotCommand method to delete a robot command from the database
            var affectedRows = _repo.ExecuteReader<RobotCommand>("DELETE FROM robotmap WHERE id=@id", sqlParams);
            return affectedRows.Count > 0; // Return true if one or more rows were deleted
        }

        public bool MapExists(string name)
        {
            var sqlParams = new NpgsqlParameter[]
            {
                new NpgsqlParameter("name", name)
            };
            var countMaps = _repo.ExecuteReader<RobotCommand>("SELECT * FROM robotmap WHERE name = @name");
            return countMaps.Count > 0;
        }

        public void InsertMap(Map newMap)
        {
            var sqlParams = new NpgsqlParameter[]
            {
                new NpgsqlParameter("id", newMap.Id),
                new NpgsqlParameter("rows", newMap.Rows),
                new NpgsqlParameter("columns", newMap.Columns),
                new NpgsqlParameter("name", newMap.Name),
                new NpgsqlParameter("description", newMap.Description ?? (object)DBNull.Value),
            };
            _repo.ExecuteReader<RobotCommand>(@"INSERT INTO robotmap (columns, rows, name, description, modifieddate) 
                                                VALUES (@Columns, @Rows, @Name, @Description, @CreatedDate, current_timestamp)", sqlParams);
        }

        public bool CheckCoordinate(int id, int x, int y)
        {
            var sqlParams = new NpgsqlParameter[]
            {
                new NpgsqlParameter("id", id)
            };

            var map = _repo.ExecuteReader<Map>(
                "SELECT columns, rows FROM robotmap WHERE id=@id",
                sqlParams).SingleOrDefault();

            if (map != null)
            {
                int columns = map.Columns;
                int rows = map.Rows;
                return x >= 0 && x < columns && y >= 0 && y < rows;
            }

            return false;
        }

        public Map GetMapById(int id)
        {
            var sqlParams = new NpgsqlParameter[]
            {
                new NpgsqlParameter("id", id)
            };

            var map = _repo.ExecuteReader<Map>(
                "SELECT * FROM robotmap WHERE id=@id",
                sqlParams).SingleOrDefault();

            return map;
        }
        public List<Map> GetSquareMaps()
        {
            var sql = @"
                SELECT *
                FROM robotmap
                WHERE rows > 0 AND columns > 0 AND rows = columns
            ";

            var squareMaps = _repo.ExecuteReader<Map>(sql);
            return squareMaps;
        }

    }

}
