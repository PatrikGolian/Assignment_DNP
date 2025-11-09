using ApiContracts;

namespace BlazorApp.Services;

public interface IUserService
{
   public Task<UserDto> AddUserAsync(CreateUserDto request); 
   public Task UpdateUserAsync(int id, UpdateUserDto request); 
   public Task DeleteUserAsync(int id);
   public Task<UserDto?> GetUserAsync(int id);
   public Task<List<UserDto>?> GetUsersAsync();
}