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

        var user = new User
            { Username = request.UserName, Password = request.Password };

        User created = await userRepo.AddAsync(user);

        UserDto dto = new()

        {

            Id = created.Id,

            UserName = created.Username

        };
        return Created($"/users/{dto.Id}", created);

    }


    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserDto>> UpdateUser(int id,
        [FromBody] UpdateUserDto request)
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

        User? existing;
        try
        {
            existing = await userRepo.GetSingleAsync(id);
        }
        catch(InvalidOperationException)
        {
            return NotFound($"User with ID {id} was not found");
        }

        existing.Username = request.UserName;
        existing.Password = request.Password;

        await userRepo.UpdateAsync(existing);

            
        UserDto dto = new()

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