using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class CommentFileRepository : ICommentRepository
{
    private readonly string filePath = "comments.json";

    public CommentFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
        
    }
    public async Task<Comment> AddAsync(Comment comment)
    {
        string commentAsJson = await File.ReadAllTextAsync(filePath);
        List<Comment> comments =
            JsonSerializer.Deserialize<List<Comment>>(commentAsJson)!;
        int maxId = comments.Count > 0 ? comments.Max(c => c.Id) : 01;
        comment.Id = maxId + 1;
        comments.Add(comment);
        commentAsJson = JsonSerializer.Serialize(comments);
        await File.WriteAllTextAsync(filePath, commentAsJson);
        return comment;

    }

    public async Task UpdateAsync(Comment comment)
    {
        string commentAsJson = await File.ReadAllTextAsync(filePath);
        List<Comment> comments =
            JsonSerializer.Deserialize<List<Comment>>(commentAsJson)!;

        Comment? existing = comments.SingleOrDefault(c => c.Id == comment.Id);

        if (existing is null)
        {
            throw new InvalidOperationException(
                $"Comment with id {comment.Id} was not found.");
        }

        comments.Remove(existing);
        comments.Add(comment);

        commentAsJson = JsonSerializer.Serialize(comments);
        await File.WriteAllTextAsync(filePath, commentAsJson);
    }

    public async Task DeleteAsync(int id)
    {
        string commentAsJson = await File.ReadAllTextAsync(filePath);
        List<Comment> comments =
            JsonSerializer.Deserialize<List<Comment>>(commentAsJson)!;

        Comment? toRemove = comments.SingleOrDefault(c => c.Id == id);

        if (toRemove is null)
        {
            throw new InvalidOperationException(
                $"Comment with ID {id} was not found");
        }

        comments.Remove(toRemove);

        commentAsJson = JsonSerializer.Serialize(comments);
        await File.WriteAllTextAsync(filePath, commentAsJson);
    }

    public async Task<Comment> GetSingleAsync(int id)
    {
        string commentAsJson = await File.ReadAllTextAsync(filePath);
        List<Comment> comments =
            JsonSerializer.Deserialize<List<Comment>>(commentAsJson)!;

        Comment? commentToGet = comments.SingleOrDefault(c => c.Id == id);

        if (commentToGet is null)
        {
            throw new InvalidOperationException(
                $"Comment with id {id} was not found");
        }

        return commentToGet;
    }

    public IQueryable<Comment> GetManyAsync()
    {
        string commentAsJson = File.ReadAllTextAsync(filePath).Result;
        List<Comment> comments =
            JsonSerializer.Deserialize<List<Comment>>(commentAsJson)!;
        return comments.AsQueryable();
    }
}