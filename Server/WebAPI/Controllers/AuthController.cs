using ApiContracts;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class AuthController :ControllerBase
{
    private IUserRepository userRepo;

    public AuthController(IUserRepository userRepo)
    {
        this.userRepo = userRepo;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(
        [FromBody] LoginRequest request)
    {
        var users =  userRepo.GetManyAsync().ToList();

        var user = users.FirstOrDefault(u =>
            u.Username == request.username && u.Password == request.password);

        if (user == null)
        {
            return Unauthorized("Invalid username or password.");
        }
        
        UserDto dto = new()
        {
            Id = user.Id,

            UserName = user.Username

        };
        return Ok(dto);
        
    }
    
}