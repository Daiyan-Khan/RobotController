using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Models;
using System.Collections.Generic;
using robot_controller_api.Persistence;
using Microsoft.AspNetCore.Authorization;
using robot_controller_api.Authentication;

[ApiController]
[Route("api/user")]
public class UsersController : ControllerBase
{
    private readonly IUserModelDataAccess _userDataAccess;
    

    public UsersController(IUserModelDataAccess userService)
    {
        _userDataAccess = userService;
    }

    // GET: api/users

    [HttpGet()]
    public ActionResult<IEnumerable<UserModel>> GetUsers(){ var users = _userDataAccess.GetAllUsers(); return Ok(users); }

    // GET: api/users/admin
    [HttpGet("admins")]
    public IEnumerable<UserModel>GetAllAdmins()
    {
        return _userDataAccess.GetAllAdmins().Where(x => x.Role == "admin");
    }

    // GET: api/users/{id}
    [HttpGet("{id}")]
    public ActionResult<UserModel> GetUserById(int id)
    {
        var user = _userDataAccess.GetUserById(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    // POST: api/users
    [HttpPost()]
    public IActionResult AddUser(UserModel newUser)
    {
        UserModel? userExists = _userDataAccess.GetUserByEmail(newUser.Email);
        if (userExists != null && userExists.Email == newUser.Email)
        {
            return Conflict();
        }
        var hasher = new EncodeSHA();
        var pwHash = hasher.HashPassword(newUser.PasswordHash);

        newUser.PasswordHash = pwHash;
        _userDataAccess.AddUser(newUser);

        return CreatedAtAction("GetUsers", new { id = newUser.Id }, newUser);
    }

    // PUT: api/users/{id}
    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, UserModel updatedUser)
    {
        var existingUser = _userDataAccess.GetUserById(id);
        if (existingUser == null)
        {
            return NotFound();
        }

        // Ensure we only update allowed properties
        existingUser.FirstName = updatedUser.FirstName;
        existingUser.LastName = updatedUser.LastName;
        existingUser.Description = updatedUser.Description;
        existingUser.Role = updatedUser.Role;
        existingUser.ModifiedDate = DateTime.UtcNow;

        _userDataAccess.UpdateUser(existingUser);

        return NoContent();
    }

    // DELETE: api/users/{id}
    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        var user = _userDataAccess.GetUserById(id);
        if (user == null)
        {
            return NotFound();
        }

        _userDataAccess.DeleteUser(id);

        return NoContent();
    }


    // PATCH: api/users/{id}
    [HttpPatch("{id}")]
    public IActionResult PatchUser(int id, LoginModel loginModel)
    {
        var user = _userDataAccess.GetUserById(id);
        if (user == null)
        {
            return NotFound();
        }

        _userDataAccess.PatchUser(id, loginModel);

        return NoContent();
    }
}
