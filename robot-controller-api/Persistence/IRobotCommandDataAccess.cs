using System.Collections.Generic;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence
{
    public interface IRobotCommandDataAccess
    {
        List<RobotCommand> GetRobotCommands();
        RobotCommand GetRobotCommandById(int id);
        public List<RobotCommand> GetMoveCommands();
        void UpdateRobotCommand(RobotCommand updatedCommand);
        void InsertRobotCommand(RobotCommand newCommand);
        bool DeleteRobotCommand(int id);
    }
}
