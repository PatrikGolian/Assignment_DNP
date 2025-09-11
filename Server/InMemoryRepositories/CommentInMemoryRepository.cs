using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class CommentInMemoryRepository : ICommentRepository
{

    private List<Comment> comments = new();
    
    
    public CommentInMemoryRepository()
    {
        SomeDummyData();
    }

    private void SomeDummyData()
    {
        comments.Add(new Comment
        {
            Id = 1,
            Body = "First comment!",
            PostId = 1,
            UserId = 2
        });
        comments.Add(new Comment
        {
            Id = 3,
            Body = "Second comment.",
            PostId = 1,
            UserId = 3
        });
        comments.Add(new Comment
        {
            Id = 2,
            Body = "Third comm.",
            PostId = 2,
            UserId = 1
        });
    }
    
    public Task<Comment> AddAsync(Comment comment)
    {
        comment.Id = comments.Any()
            ? comments.Max(p => p.Id) + 1
            : 1;
        comments.Add(comment);
        return Task.FromResult(comment);
    }

    public Task UpdateAsync(Comment comment)
    {
        Comment? existingComment = comments.SingleOrDefault(c => c.Id == comment.Id);
        if (existingComment is null)
        {
            throw new InvalidOperationException(

                $"Comment with ID '{comment.Id}' not found");
        }

        comments.Remove(existingComment);
        comments.Add(comment); 
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        Comment? commentToRemove = comments.SingleOrDefault(c => c.Id == id);
        if (commentToRemove is null)
        { throw new InvalidOperationException(
            $"Comment with ID '{id}' not found"); } 
        comments.Remove(commentToRemove); 
        return Task.CompletedTask;
    }

    public Task<Comment> GetSingleAsync(int id)
    {
        Comment? commentToGet = comments.SingleOrDefault(c => c.Id == id);
        if (commentToGet is null)
        { throw new InvalidOperationException(
            $"Comment with ID '{id}' not found"); } 

        return Task.FromResult(commentToGet);
    }

    public IQueryable<Comment> GetManyAsync()
    {
        return comments.AsQueryable();
        
    }
}