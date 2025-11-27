using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace EfcRepositories;

public class EfcUserRepository : IUserRepository
{
    private readonly AppContext ctx;

    public EfcUserRepository(AppContext ctx)
    {
        this.ctx = ctx;
    }
    public async Task<User> AddAsync(User user)
    {
        await ctx.Users.AddAsync(user);
        await ctx.SaveChangesAsync();
        return user;
    }

    public async Task  UpdateAsync(User user)
    {
        if(!(await ctx.Users.AnyAsync(u => u.Id == user.Id)))
        {
            throw new ArgumentException($"User with id {user.Id} does not exist.");
        }
        ctx.Users.Update(user);
        await ctx.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        User? existing = await ctx.Users.SingleOrDefaultAsync(u=>u.Id == id);
        if (existing == null)
        {
            throw new ArgumentException($"User with id {id} does not exist.");
        }
        ctx.Users.Remove(existing);
        await ctx.SaveChangesAsync();
    }

    public async Task<User> GetSingleAsync(int id)
    {
        User? existing = await ctx.Users.SingleOrDefaultAsync(u=>u.Id == id);
        if (existing == null)
        {
            throw new ArgumentException($"User with id {id} does not exist.");
        }
        return existing;
    }

    public IQueryable<User> GetManyAsync()
    {
        return ctx.Users.AsQueryable();
    }
}