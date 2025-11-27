using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace EfcRepositories;

public class EfcCommentRepository : ICommentRepository
{
    private readonly AppContext ctx;

    public EfcCommentRepository(AppContext ctx)
    {
        this.ctx = ctx;
    }
    public  async Task<Comment> AddAsync(Comment comment)
    {
        await ctx.Comments.AddAsync(comment);
        await ctx.SaveChangesAsync();
        return comment;
    }

    public async Task UpdateAsync(Comment comment)
    {
        if(!(await ctx.Comments.AnyAsync(c => c.Id == comment.Id)))
        {
            throw new ArgumentException($"Comment with id {comment.Id} does not exist.");
        }
        ctx.Comments.Update(comment);
        await ctx.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        Comment? existing =
            await ctx.Comments.SingleOrDefaultAsync(c => c.Id == id);
        if (existing == null)
        {
            throw new ArgumentException($"Comment with id {id} does not exist.");
        }
        ctx.Comments.Remove(existing);
        await ctx.SaveChangesAsync();
    }

    public async Task<Comment> GetSingleAsync(int id)
    {
        Comment? existing =
            await ctx.Comments.SingleOrDefaultAsync(c => c.Id == id);
        if (existing == null)
        {
            throw new ArgumentException(
                $"Comment with id {id} does not exist.");
        }
        return  existing;
    }

        
    public IQueryable<Comment> GetManyAsync()
    {
        return  ctx.Comments.AsQueryable();  
    }
}