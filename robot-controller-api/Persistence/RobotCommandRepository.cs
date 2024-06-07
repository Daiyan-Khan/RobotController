using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using robot_controller_api.Persistence;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence;
public class RobotCommandRepository : IRobotCommandDataAccess, IRepository
{
    private IRepository _repo => this;

    public List<RobotCommand> GetRobotCommands() => _repo.ExecuteReader<RobotCommand>("SELECT * FROM public.robotcommand");

    public List<RobotCommand> GetMoveCommands()
    {
        var sql = @"SELECT * FROM public.robotcommand WHERE ismovecommand = true";
        return _repo.ExecuteReader<RobotCommand>(sql);
    }

    public void UpdateRobotCommand(RobotCommand updatedCommand)
    {
        var sqlParams = new NpgsqlParameter[]
        {
            new NpgsqlParameter("id", updatedCommand.Id),
            new NpgsqlParameter("name", updatedCommand.Name),
            new NpgsqlParameter("description", updatedCommand.Description ?? (object)DBNull.Value),
            new NpgsqlParameter("ismovecommand", updatedCommand.IsMoveCommand)
        };

        var result = _repo.ExecuteReader<RobotCommand>(
            @"UPDATE robotcommand 
              SET name=@name, description=@description, ismovecommand=@ismovecommand, modifieddate=current_timestamp 
              WHERE id=@id 
              RETURNING *;",
            sqlParams).Single();
    }

    public RobotCommand GetRobotCommandById(int id)
    {
        var sqlParams = new NpgsqlParameter[]
        {
            new NpgsqlParameter("id", id)
        };

        // Implementing GetRobotCommandById method to fetch a specific robot command by its ID from the database
        var command = _repo.ExecuteReader<RobotCommand>("SELECT * FROM public.robotcommand WHERE id=@id", sqlParams)
                          .SingleOrDefault(); // Use SingleOrDefault to get a single result or null
        return command;
    }
    public void InsertRobotCommand(RobotCommand newCommand)
    {
        // Implementing InsertRobotCommand method to insert a new robot command into the database
        var sqlParams = new NpgsqlParameter[]
        {
            new NpgsqlParameter("name", newCommand.Name),
            new NpgsqlParameter("description", newCommand.Description ?? (object)DBNull.Value),
            new NpgsqlParameter("ismovecommand", newCommand.IsMoveCommand),
            new NpgsqlParameter("createddate", DateTime.Now),
            new NpgsqlParameter("modifieddate", DateTime.Now)
        };

        var entities = _repo.ExecuteReader<RobotCommand>(
            @"INSERT INTO robotcommand (name, description, ismovecommand, createddate, modifieddate) 
              VALUES (@name, @description, @ismovecommand, @createddate, @modifieddate);",
            sqlParams);
    }
    public bool DeleteRobotCommand(int id)
    {
        var sqlParams = new NpgsqlParameter[]
        {
            new NpgsqlParameter("id", id)
        };
        // Implementing DeleteRobotCommand method to delete a robot command from the database
        var affectedRows = _repo.ExecuteReader<RobotCommand>("DELETE FROM robotcommand WHERE id=@id", sqlParams);
        return affectedRows.Count > 0; // Return true if one or more rows were deleted
    }
}
