using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class UserInMemoryRepository : IUserRepository
{
    
    private List<User> users = new();

    public UserInMemoryRepository()
    {
        //SomeDummyData();
    }
    // private void SomeDummyData()
    // {
    //     users.Add(new User { Id = 1, Username = "alice", Password = "password1" });
    //     users.Add(new User { Id = 3, Username = "bob", Password = "password2" });
    //     users.Add(new User { Id = 2, Username = "charlie", Password = "password3" });
    // }
    
    public Task<User> AddAsync(User user)
    {
        user.Id = users.Any()
            ? users.Max(p => p.Id) + 1
            : 1;
        users.Add(user);
        return Task.FromResult(user);
    }

    public Task UpdateAsync(User user)
    {
        User? existingUser = users.SingleOrDefault(u => u.Id == user.Id);
        if (existingUser is null)
        {
            throw new InvalidOperationException(

                $"User with ID '{user.Id}' not found");
        }

        users.Remove(existingUser);
        users.Add(user); 
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        User? userToRemove = users.SingleOrDefault(u => u.Id == id);
        if (userToRemove is null)
        { throw new InvalidOperationException(
            $"User with ID '{id}' not found"); } 
        users.Remove(userToRemove); 
        return Task.CompletedTask;
    }

    public Task<User> GetSingleAsync(int id)
    {
        User? userToGet = users.SingleOrDefault(u => u.Id == id);
        if (userToGet is null)
        { throw new InvalidOperationException(
            $"User with ID '{id}' not found"); } 

        return Task.FromResult(userToGet);
    }

    public IQueryable<User> GetManyAsync()
    {
        return users.AsQueryable();
    }
}