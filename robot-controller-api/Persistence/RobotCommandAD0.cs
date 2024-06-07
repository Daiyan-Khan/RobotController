using Npgsql;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence;
public class RobotCommandAD0 : IRobotCommandDataAccess
{
    private const string CONNECTION_STRING = "Host=localhost;Username=postgres;Password=1234;Database=postgres";
    // Connection string is usually set in a config file for the easeof change.
    public List<RobotCommand> GetRobotCommands()
    {
        var robotCommands = new List<RobotCommand>();
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT * FROM public.robotcommand", conn);
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            //read values off the data reader and create a new robotCommand here and then add it to the result list.
            int id = (int)dr["id"];
            string name = (string)dr["Name"];
            string? desc = dr["description"] as string;
            bool isMoveCommand = (bool)dr["ismovecommand"];
            DateTime createdDate = (DateTime)dr["createddate"];
            DateTime modifiedDate = (DateTime)dr["modifieddate"];

            // Create a new RobotCommand instance
            var robotCommand = new RobotCommand(id, name, isMoveCommand, createdDate, modifiedDate, desc);

            // Add the RobotCommand to the list
            robotCommands.Add(robotCommand);
        }
        return robotCommands;
    }
    public List<RobotCommand> GetMoveCommands()
    {
        List<RobotCommand> moveCommands = new List<RobotCommand>();

        using (var connection = new NpgsqlConnection(CONNECTION_STRING))
        {
            connection.Open();

            var sql = "SELECT * FROM public.robotcommand WHERE ismovecommand = true";

            using (var command = new NpgsqlCommand(sql, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RobotCommand cmd = new RobotCommand
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["name"].ToString(),
                            Description = reader["description"] == DBNull.Value ? null : reader["description"].ToString(),
                            IsMoveCommand = Convert.ToBoolean(reader["ismovecommand"]),
                            CreatedDate = Convert.ToDateTime(reader["createddate"]),
                            ModifiedDate = Convert.ToDateTime(reader["modifieddate"])
                        };

                        moveCommands.Add(cmd);
                    }
                }
            }
        }

        return moveCommands;
    }
    public void UpdateRobotCommand(RobotCommand updatedCommand)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand("UPDATE robotcommand SET \"Name\" = @Name, description = @Description, ismovecommand = @IsMoveCommand, modifieddate = current_timestamp WHERE id = @Id", conn);

        cmd.Parameters.AddWithValue("Id", updatedCommand.Id);
        cmd.Parameters.AddWithValue("Name", updatedCommand.Name);
        cmd.Parameters.AddWithValue("Description", updatedCommand.Description ?? (object)DBNull.Value); // Handle null description
        cmd.Parameters.AddWithValue("IsMoveCommand", updatedCommand.IsMoveCommand);
        cmd.Parameters.AddWithValue("ModifiedDate", updatedCommand.ModifiedDate);

        cmd.ExecuteNonQuery();
    }

    public void InsertRobotCommand(RobotCommand newCommand)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand("INSERT INTO robotcommand (\"Name\", description, ismovecommand, createddate, modifieddate) VALUES (@Name, @Description, @IsMoveCommand, @CreatedDate, @ModifiedDate)", conn);

        cmd.Parameters.AddWithValue("Name", newCommand.Name);
        cmd.Parameters.AddWithValue("Description", newCommand.Description ?? (object)DBNull.Value); // Handle null description
        cmd.Parameters.AddWithValue("IsMoveCommand", newCommand.IsMoveCommand);
        cmd.Parameters.AddWithValue("CreatedDate", newCommand.CreatedDate);
        cmd.Parameters.AddWithValue("ModifiedDate", newCommand.ModifiedDate);

        cmd.ExecuteNonQuery();
    }

    public RobotCommand GetRobotCommandById(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        var commandText = "SELECT * FROM robotcommand WHERE id = @id";
        using var cmd = new NpgsqlCommand(commandText, conn);
        cmd.Parameters.AddWithValue("id", id);

        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
                int Id = (int)dr["id"];
                string name = (string)dr["Name"];
                string? desc = dr["description"] as string;
                bool isMoveCommand = (bool)dr["ismovecommand"];
                DateTime createdDate = (DateTime)dr["createddate"];
                DateTime modifiedDate = (DateTime)dr["modifieddate"];

                // Create a new RobotCommand instance
                return new RobotCommand(Id, name, isMoveCommand, createdDate, modifiedDate, desc);
        }

        return null; // Return null if no matching command is found
    }
    public bool DeleteRobotCommand(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        var commandText = "DELETE FROM robotcommand WHERE id = @id";
        using var cmd = new NpgsqlCommand(commandText, conn);
        cmd.Parameters.AddWithValue("id", id);

        int rowsAffected = cmd.ExecuteNonQuery();
        return rowsAffected > 0; // Return true if one or more rows were deleted
    }

}
