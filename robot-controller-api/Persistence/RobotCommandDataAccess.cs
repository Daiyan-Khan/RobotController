using Npgsql;
namespace robot_controller_api.Persistence;
public class RobotCommandDataAccess
{
    private const string CONNECTION_STRING = "Host=localhost;Username=postgres;Password=1234;Database=sit331";
    // Connection string is usually set in a config file for the easeof change.
    public List<RobotCommand> GetRobotCommands()
    {
        var robotCommands = new List<RobotCommand>();
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT * FROM robotcommand", conn);
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
                DateTime modifiedDate = (DateTime)dr["modifieddateusing Npgsql;\r\nusing System;\r\nusing System.Collections.Generic;\r\n\r\nnamespace robot_controller_api.Persistence\r\n{\r\n    public static class RobotCommandDataAccess\r\n    {\r\n        private const string CONNECTION_STRING = \"Host=localhost;Username=postgres;Password=1234;Database=sit331\";\r\n\r\n        public static List<RobotCommand> GetRobotCommands()\r\n        {\r\n            var robotCommands = new List<RobotCommand>();\r\n\r\n            using (var conn = new NpgsqlConnection(CONNECTION_STRING))\r\n            {\r\n                conn.Open();\r\n\r\n                using (var cmd = new NpgsqlCommand(\"SELECT * FROM robotcommand\", conn))\r\n                using (var dr = cmd.ExecuteReader())\r\n                {\r\n                    while (dr.Read())\r\n                    {\r\n                        robotCommands.Add(ReadRobotCommand(dr));\r\n                    }\r\n                }\r\n            }\r\n\r\n            return robotCommands;\r\n        }\r\n\r\n        public static void UpdateRobotCommand(RobotCommand updatedCommand)\r\n        {\r\n            using (var conn = new NpgsqlConnection(CONNECTION_STRING))\r\n            {\r\n                conn.Open();\r\n\r\n                using (var cmd = new NpgsqlCommand(\"UPDATE robotcommand SET \\\"Name\\\" = @Name, description = @Description, ismovecommand = @IsMoveCommand, modifieddate = @ModifiedDate WHERE id = @Id\", conn))\r\n                {\r\n                    AddCommandParameters(cmd, updatedCommand);\r\n                    cmd.ExecuteNonQuery();\r\n                }\r\n            }\r\n        }\r\n\r\n        public static void InsertRobotCommand(RobotCommand newCommand)\r\n        {\r\n            using (var conn = new NpgsqlConnection(CONNECTION_STRING))\r\n            {\r\n                conn.Open();\r\n\r\n                using (var cmd = new NpgsqlCommand(\"INSERT INTO robotcommand (\\\"Name\\\", description, ismovecommand, createddate, modifieddate) VALUES (@Name, @Description, @IsMoveCommand, @CreatedDate, @ModifiedDate)\", conn))\r\n                {\r\n                    AddCommandParameters(cmd, newCommand);\r\n                    cmd.ExecuteNonQuery();\r\n                }\r\n            }\r\n        }\r\n\r\n        public static RobotCommand GetRobotCommandById(int id)\r\n        {\r\n            using (var conn = new NpgsqlConnection(CONNECTION_STRING))\r\n            {\r\n                conn.Open();\r\n\r\n                var commandText = \"SELECT * FROM robotcommand WHERE id = @id\";\r\n                using (var cmd = new NpgsqlCommand(commandText, conn))\r\n                {\r\n                    cmd.Parameters.AddWithValue(\"id\", id);\r\n\r\n                    using (var dr = cmd.ExecuteReader())\r\n                    {\r\n                        if (dr.Read())\r\n                        {\r\n                            return ReadRobotCommand(dr);\r\n                        }\r\n                    }\r\n                }\r\n            }\r\n\r\n            return null; // Return null if no matching command is found\r\n        }\r\n\r\n        public static bool DeleteRobotCommand(int id)\r\n        {\r\n            using (var conn = new NpgsqlConnection(CONNECTION_STRING))\r\n            {\r\n                conn.Open();\r\n\r\n                var commandText = \"DELETE FROM robotcommand WHERE id = @id\";\r\n                using (var cmd = new NpgsqlCommand(commandText, conn))\r\n                {\r\n                    cmd.Parameters.AddWithValue(\"id\", id);\r\n\r\n                    int rowsAffected = cmd.ExecuteNonQuery();\r\n                    return rowsAffected > 0; // Return true if one or more rows were deleted\r\n                }\r\n            }\r\n        }\r\n\r\n        private static void AddCommandParameters(NpgsqlCommand cmd, RobotCommand command)\r\n        {\r\n            cmd.Parameters.AddWithValue(\"Id\", command.Id);\r\n            cmd.Parameters.AddWithValue(\"Name\", command.Name);\r\n            cmd.Parameters.AddWithValue(\"Description\", command.Description ?? DBNull.Value);\r\n            cmd.Parameters.AddWithValue(\"IsMoveCommand\", command.IsMoveCommand);\r\n            cmd.Parameters.AddWithValue(\"CreatedDate\", command.CreatedDate);\r\n            cmd.Parameters.AddWithValue(\"ModifiedDate\", command.ModifiedDate);\r\n        }\r\n\r\n        private static RobotCommand ReadRobotCommand(NpgsqlDataReader dr)\r\n        {\r\n            int id = (int)dr[\"id\"];\r\n            string name = (string)dr[\"Name\"];\r\n            string desc = dr[\"description\"] as string;\r\n            bool isMoveCommand = (bool)dr[\"ismovecommand\"];\r\n            DateTime createdDate = (DateTime)dr[\"createddate\"];\r\n            DateTime modifiedDate = (DateTime)dr[\"modifieddate\"];\r\n\r\n            return new RobotCommand(id, name, isMoveCommand, createdDate, modifiedDate, desc);\r\n        }\r\n    }\r\n}\r\n"];

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
