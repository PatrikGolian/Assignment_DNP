using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class UserFileRepository : IUserRepository
{
    private readonly string filePath = "users.json";

    public UserFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }
    public async Task<User> AddAsync(User user)
    {
        string userAsJson = await File.ReadAllTextAsync(filePath);
        List<User> users = JsonSerializer.Deserialize<List<User>>(userAsJson)!;

        int maxId = users.Count > 0 ? users.Max(u => u.Id) : 0;
        user.Id = maxId + 1;
        users.Add(user);

        userAsJson = JsonSerializer.Serialize(users);
        await File.WriteAllTextAsync(filePath, userAsJson);
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        string userAsJson = await File.ReadAllTextAsync(filePath);
        List<User> users = JsonSerializer.Deserialize<List<User>>(userAsJson)!;

        User? existing = users.SingleOrDefault(u => u.Id == user.Id);

        if (existing is null)
        {
            throw new InvalidOperationException(
                $"User with ID {user.Id} was not found.");
        }

        users.Remove(existing);
        users.Add(user);

        userAsJson = JsonSerializer.Serialize(users);
        await File.WriteAllTextAsync(filePath, userAsJson);
    }

    public async Task DeleteAsync(int id)
    {
        string userAsJson = await File.ReadAllTextAsync(filePath);
        List<User> users = JsonSerializer.Deserialize<List<User>>(userAsJson)!;

        User? toRemove = users.SingleOrDefault(u => u.Id == id);

        if (toRemove is null)
        {
            throw new InvalidOperationException(
                $"User with ID {id} was not found.");
        }

        users.Remove(toRemove);

        userAsJson = JsonSerializer.Serialize(users);
        await File.WriteAllTextAsync(filePath, userAsJson);
    }

    public async Task<User> GetSingleAsync(int id)
    {
        string userAsJson = await File.ReadAllTextAsync(filePath);
        List<User> users = JsonSerializer.Deserialize<List<User>>(userAsJson)!;

        User? toGet = users.SingleOrDefault(u => u.Id == id);
        
        if (toGet is null)
        {
            throw new InvalidOperationException(
                $"User with ID {id} was not found.");
        }

        return toGet;
    }

    public IQueryable<User> GetManyAsync()
    {
        string userAsJson = File.ReadAllTextAsync(filePath).Result;
        List<User> users =
            JsonSerializer.Deserialize<List<User>>(userAsJson)!;
        return users.AsQueryable();
    }
}