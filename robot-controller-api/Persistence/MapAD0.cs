using Npgsql;
using System;
using System.Collections.Generic;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence
{
    public class MapAD0 : IMapDataAccess
    {
        private const string CONNECTION_STRING = "Host=localhost;Username=postgres;Password=1234;Database=postgres";

        public List<Map> GetMaps()
        {
            var maps = new List<Map>();

            using (var conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT * FROM public.robotmap", conn))
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

        public List<Map> GetSquareMaps()
        {
            List<Map> squareMaps = new List<Map>();

            using (NpgsqlConnection connection = new NpgsqlConnection(CONNECTION_STRING))
            {
                string query = @"
                    SELECT *
                    FROM public.robotmap
                    WHERE rows > 0 AND columns > 0 AND rows = columns
                ";

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    connection.Open();

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Map map = new Map
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Rows = Convert.ToInt32(reader["rows"]),
                                Columns = Convert.ToInt32(reader["columns"]),
                                Name = Convert.ToString(reader["Name"]),
                                Description = reader["description"] != DBNull.Value ? Convert.ToString(reader["description"]) : null,
                                CreatedDate = Convert.ToDateTime(reader["createddate"]),
                                ModifiedDate = Convert.ToDateTime(reader["modifieddate"])
                            };

                            squareMaps.Add(map);
                        }
                    }
                }
            }

            return squareMaps;
        }
        public void UpdateMap(Map updatedMap)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            using var cmd = new NpgsqlCommand(@"UPDATE public.robotmap 
                                                SET columns = @Columns, 
                                                    rows = @Rows, 
                                                    Name = @Name, 
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

                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM public.robotmap WHERE Name = @Name", conn))
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
