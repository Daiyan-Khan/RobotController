using Npgsql;
using System;
using System.Collections.Generic;

namespace robot_controller_api.Persistence
{
    public class MapDataAccess
    {
        private const string CONNECTION_STRING = "Host=localhost;Username=postgres;Password=1234;Database=sit331";

        public List<Map> GetMaps()
        {
            var maps = new List<Map>();

            using (var conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT * FROM robotmap", conn))
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        maps.Add(ReadMap(dr));
                    }
                }
            }

            return maps;
        }

        public void UpdateMap(Map updatedMap)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            using var cmd = new NpgsqlCommand(@"UPDATE robotmap 
                                                SET columns = @Columns, 
                                                    rows = @Rows, 
                                                    name = @Name, 
                                                    description = @Description, 
                                                    modifieddate = current_timestamp
                                                WHERE id = @Id", conn);

            AddMapParameters(cmd, updatedMap);
            cmd.ExecuteNonQuery();
        }
        public bool MapExists(string name)
        {
            using (var conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM robotmap WHERE name = @Name", conn))
                {
                    cmd.Parameters.AddWithValue("Name", name);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }


        public void InsertMap(Map newMap)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            using var cmd = new NpgsqlCommand(@"INSERT INTO robotmap (columns, rows, name, description, createddate, modifieddate) 
                                                VALUES (@Columns, @Rows, @Name, @Description, @CreatedDate, @ModifiedDate)", conn);

            AddMapParameters(cmd, newMap);
            cmd.ExecuteNonQuery();
        }

        public bool DeleteMap(int id)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            using var cmd = new NpgsqlCommand("DELETE FROM robotmap WHERE id = @Id", conn);
            cmd.Parameters.AddWithValue("Id", id);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool CheckCoordinate(int id, int x, int y)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            using var cmd = new NpgsqlCommand("SELECT columns, rows FROM robotmap WHERE id = @Id", conn);
            cmd.Parameters.AddWithValue("Id", id);

            using var dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                int columns = (int)dr["columns"];
                int rows = (int)dr["rows"];
                return x >= 0 && x < columns && y >= 0 && y < rows;
            }

            return false;
        }

        public  Map GetMapById(int id)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            using var cmd = new NpgsqlCommand("SELECT * FROM robotmap WHERE id = @Id", conn);
            cmd.Parameters.AddWithValue("Id", id);

            using var dr = cmd.ExecuteReader();
            return dr.Read() ? ReadMap(dr) : null;
        }

        private Map ReadMap(NpgsqlDataReader dr)
        {
            int id = (int)dr["id"];
            int columns = (int)dr["columns"];
            int rows = (int)dr["rows"];
            string name = (string)dr["name"];
            string description = dr["description"] as string;
            DateTime createdDate = (DateTime)dr["createddate"];
            DateTime modifiedDate = (DateTime)dr["modifieddate"];

            return new Map(id, columns, rows, name, description, createdDate, modifiedDate);
        }

        private void AddMapParameters(NpgsqlCommand cmd, Map map)
        {
            cmd.Parameters.AddWithValue("Id", map.Id);
            cmd.Parameters.AddWithValue("Columns", map.Columns);
            cmd.Parameters.AddWithValue("Rows", map.Rows);
            cmd.Parameters.AddWithValue("Name", map.Name);
            if (map.Description != null)
            {
                cmd.Parameters.AddWithValue("Description", map.Description);
            }
            else
            {
                cmd.Parameters.AddWithValue("Description", DBNull.Value);
            }
            cmd.Parameters.AddWithValue("CreatedDate", map.CreatedDate);
            cmd.Parameters.AddWithValue("ModifiedDate", map.ModifiedDate);
        }
    }
}
