using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class PostFileRepository : IPostRepository
{
    private readonly string filePath = "posts.json";

    public PostFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    public async Task<Post> AddAsync(Post post)
    {
        string postAsJson = await File.ReadAllTextAsync(filePath);
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(postAsJson)!;
        int maxId = posts.Count > 0 ? posts.Max(c => c.Id) : 0;
        post.Id = maxId + 1;
        posts.Add(post);

        postAsJson = JsonSerializer.Serialize(posts);
        await File.WriteAllTextAsync(filePath, postAsJson);
        return post;
    }

    public async Task UpdateAsync(Post post)
    {
        string postAsJson = await File.ReadAllTextAsync(filePath);
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(postAsJson)!;

        Post? existing = posts.SingleOrDefault(p => p.Id == post.Id);

        if (existing is null)
        {
            throw new InvalidOperationException(
                $"Post with id {post.Id} was not found.");
        }

        posts.Remove(existing);
        posts.Add(post);

        postAsJson = JsonSerializer.Serialize(posts);
        await File.WriteAllTextAsync(filePath, postAsJson);
    }

    public async Task DeleteAsync(int id)
    {
        string postAsJson = await File.ReadAllTextAsync(filePath);
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(postAsJson)!;

        Post? toRemove = posts.SingleOrDefault(p => p.Id == id);
        
        if(toRemove is null)
        {
            throw new InvalidOperationException(
                $"Post with ID {id} was not found");
        }

        posts.Remove(toRemove);

        postAsJson = JsonSerializer.Serialize(posts);
        await File.WriteAllTextAsync(filePath, postAsJson);
    }

    public async Task<Post> GetSingleAsync(int id)
    {
        string postAsJson = await File.ReadAllTextAsync(filePath);
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(postAsJson)!;

        Post? postToGet = posts.SingleOrDefault(p => p.Id == id);

        if (postToGet is null)
        {
            throw new InvalidOperationException(
                $"Post with ID {id} was not found.");
        }

        return postToGet;
    }

    public IQueryable<Post> GetManyAsync()
    {
        string postAsJson = File.ReadAllTextAsync(filePath).Result;
        List<Post> posts =
            JsonSerializer.Deserialize<List<Post>>(postAsJson)!;
        return posts.AsQueryable();
    }
}