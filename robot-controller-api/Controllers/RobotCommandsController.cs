using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using robot_controller_api.Models;

namespace robot_controller_api.Controllers;

using Microsoft.AspNetCore.Authorization;
using robot_controller_api.Persistence;

[ApiController]
[Route("api/robot-commands")]
public class RobotCommandsController : ControllerBase
{
    private readonly IRobotCommandDataAccess _robotCommandsRepo; // Instance of RobotCommandDataAccess

    public RobotCommandsController(IRobotCommandDataAccess repo)
    {
        _robotCommandsRepo = repo; // Instantiate RobotCommandDataAccess
    }

    // Robot commands endpoints here
    [AllowAnonymous]
    [HttpGet()]
    public IEnumerable<RobotCommand> GetAllRobotCommands() => _robotCommandsRepo.GetRobotCommands();

    [HttpGet("move")]
    public IEnumerable<RobotCommand> GetMoveCommandsOnly() => _robotCommandsRepo.GetRobotCommands().Where(x => x.IsMoveCommand);

    [HttpGet("{id}", Name = "GetRobotCommand")]
    public IActionResult GetRobotCommandById(int id)
    {
        var command = _robotCommandsRepo.GetRobotCommandById(id);
        if (command == null)
        {
            return NotFound("Command not found.");
        }
        return Ok(command);
    }

    [HttpPost()]
    public IActionResult AddRobotCommand(RobotCommand newCommand)
    {
        if (newCommand == null || string.IsNullOrEmpty(newCommand.Name))
        {
            return BadRequest();
        }
        _robotCommandsRepo.InsertRobotCommand(newCommand);
        return CreatedAtRoute("GetRobotCommand", new { id = newCommand.Id }, newCommand);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateRobotCommand(int id, RobotCommand updatedCommand)
    {
        var existingCommand = _robotCommandsRepo.GetRobotCommandById(id);
        if (existingCommand == null)
        {
            return NotFound();
        }

        if (updatedCommand == null)
        {
            return BadRequest();
        }

        existingCommand.Name = updatedCommand.Name;
        existingCommand.Description = updatedCommand.Description;
        existingCommand.IsMoveCommand = updatedCommand.IsMoveCommand;

        _robotCommandsRepo.UpdateRobotCommand(existingCommand);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteCommand(int id)
    {
        var existingCommand = _robotCommandsRepo.GetRobotCommandById(id);
        if (existingCommand == null)
        {
            return NotFound();
        }

        _robotCommandsRepo.DeleteRobotCommand(id);

        return NoContent();
    }


}
