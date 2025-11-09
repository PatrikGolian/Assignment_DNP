using System.Text.Json;
using ApiContracts;

namespace BlazorApp.Services;

public class HttpUserService : IUserService
{
    private readonly HttpClient _client;

    public HttpUserService(HttpClient _client)
    {
        this._client = _client;
    }

    public async Task<UserDto> AddUserAsync(CreateUserDto request)
    {
        var response = await _client.PostAsJsonAsync("users", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDto>() ?? throw new InvalidOperationException("Something went wrong, try again later");
    }

    public async Task UpdateUserAsync(int id, UpdateUserDto request)
    {
        var response = await _client.PutAsJsonAsync("users", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteUserAsync(int id)
    {
        var response = new HttpRequestMessage(HttpMethod.Delete, $"users/{id}");
        var responseMessage = await _client.SendAsync(response);
        responseMessage.EnsureSuccessStatusCode();
    }

    public async Task<UserDto?> GetUserAsync(int id)
    {
        var response = await _client.GetAsync($"users{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDto>() ?? throw new InvalidOperationException("User not found");
    }

    public Task<List<UserDto>?> GetUsersAsync()
    {
        return _client.GetFromJsonAsync<List<UserDto>>("users")?? throw new InvalidOperationException("Users not found");
    }
}