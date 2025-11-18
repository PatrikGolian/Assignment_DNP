using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;


[ApiController]
[Route("[controller]")]

public class UsersController : ControllerBase
{
    private readonly IUserRepository userRepo;

    public UsersController(IUserRepository userRepo)
    {
        this.userRepo = userRepo;
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> AddUser(
        [FromBody] CreateUserDto request)
    {
        if (!IsPasswordValid(request.Password))
        {
            return BadRequest("Password must be at least 4 characters long.");
        }

        if (await VerifyUserNameIsAvailableAsync(request.UserName))
        {
            return BadRequest(
                $"Username '{request.UserName}' is already taken.");
        }

        var user = new User(request.UserName, request.Password);
        
        User created = await userRepo.AddAsync(user);

        UserDto dto = new()

        {

            Id = created.Id,

            UserName = created.Username

        };
        return Created($"/users/{dto.Id}", dto);

    }


    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserDto request)
    {
        User? existing;
        try
        {
            existing = await userRepo.GetSingleAsync(id);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"User with ID {id} was not found");
        }

        // Only update username if provided and not taken
        if (!string.IsNullOrWhiteSpace(request.UserName) &&
            !request.UserName.Equals(existing.Username, StringComparison.OrdinalIgnoreCase))
        {
            bool usernameExists = userRepo.GetManyAsync()
                .Any(u => u.Username.Equals(request.UserName, StringComparison.OrdinalIgnoreCase) && u.Id != id);
            if (usernameExists)
                return BadRequest($"Username '{request.UserName}' is already taken.");

            existing.Username = request.UserName;
        }

        // Only update password if provided
        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            if (request.Password.Length < 4)
                return BadRequest("Password must be at least 4 characters long.");

            existing.Password = request.Password;
        }

        await userRepo.UpdateAsync(existing);

        var dto = new UserDto
        {
            Id = existing.Id,
            UserName = existing.Username
        };

        return Ok(dto);
    }


    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        User? user = await userRepo.GetSingleAsync(id);

        if (user == null)
        {
            return NotFound($"User with ID {id} not found.");
        }

        var dto = new UserDto
        {
            Id = user.Id,
            UserName = user.Username
        };

        return Ok(dto);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        User? user = await userRepo.GetSingleAsync(id);
        if (user == null)
        {
            return NotFound($"User with ID {id} not found.");
        }

        await userRepo.DeleteAsync(id);
        return NoContent();
    }

    /*[HttpGet]
    public ActionResult<IEnumerable<UserDto>> GetAllUsers()
    {
        
        var users = userRepo.GetManyAsync();
        
        var dtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            UserName = u.Username
        }).ToList();

        return Ok(dtos);
    }*/
    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> GetAllUsers([FromQuery] string? search)
    {
        var users = userRepo.GetManyAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            users = users.Where(u =>
                u.Username.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        var dtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            UserName = u.Username
        }).ToList();

        return Ok(dtos);
    }


    private Task<bool> VerifyUserNameIsAvailableAsync(string userName)
    {

        bool exists = userRepo.GetManyAsync()
            .Any(u =>
                u.Username.Equals(userName,
                    StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(exists);
    }

    private bool IsPasswordValid(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 4)
            return false;

        return true;

    }
    
}