using Npgsql;
using robot_controller_api.Persistence;
using robot_controller_api.Models;
using Microsoft.EntityFrameworkCore;

namespace robot_controller_api.Persistence
{
    // This class implements data access operations for robot commands using Entity Framework
    public class RobotCommandEF : RobotContext, IRobotCommandDataAccess
    {
        public RobotCommandEF(DbContextOptions<RobotContext> options) : base(options)
        {

        }
        private RobotContext _robotContext = new();


        // Method to retrieve all robot commands from the database
        public List<RobotCommand> GetRobotCommands()
        {
            return _robotContext.Robotcommands.OrderBy(x => x.Id).ToList();
        }

        // Method to retrieve only move commands from the database
        public List<RobotCommand> GetMoveCommands()
        {
            var commands = _robotContext.Robotcommands
                .Where(x => x.IsMoveCommand == true)
                .ToList();
            return commands;
        }

        // Method to retrieve a robot command from the database, based on its ID 
        public RobotCommand? GetRobotCommandById(int id)
        {
            return (RobotCommand?)_robotContext.Robotcommands.FirstOrDefault(x => x.Id == id);
        }

        // Method to update an existing robot command in the database, based on id
        public void UpdateRobotCommand(RobotCommand updatedCommand)
        {
            _robotContext.Entry(updatedCommand).State = EntityState.Modified;
            _robotContext.SaveChanges();
        }

        // Method to add a new robot command to the database
        public void InsertRobotCommand(RobotCommand newCommand)
        {
            _robotContext.Robotcommands.Add(newCommand);
            _robotContext.SaveChanges();
        }

        // Method to delete a robot command from the database by its ID
        public bool DeleteRobotCommand(int id)
        {
            // Find the existing robot command by its ID
            var commandToDelete = _robotContext.Robotcommands.FirstOrDefault(c => c.Id == id);

            // If the command with the given ID is found
            if (commandToDelete != null)
            {
                // Remove the command from the DbSet in the context
                _robotContext.Robotcommands.Remove(commandToDelete);

                // Save changes to the database
                _robotContext.SaveChanges();
            }
            return true;
        }
    }
}