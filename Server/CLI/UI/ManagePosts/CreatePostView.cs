using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class CreatePostView
{
    private readonly IPostRepository postRepo;

    public CreatePostView(IPostRepository postRepo)
    {
        this.postRepo = postRepo;
    }

    public async Task CreatePostAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Create Post ===");

        Console.Write("Title: ");
        string? title = Console.ReadLine();

        Console.Write("Body: ");
        string? body = Console.ReadLine();

        Console.Write("User Id: ");
        string? userIdInput = Console.ReadLine();
        int userId = int.TryParse(userIdInput, out var uid) ? uid : 0;

        var post = new Post(title ?? string.Empty, body ?? string.Empty,
            userId);

        Post created = await postRepo.AddAsync(post);

        Console.WriteLine($"Post created with ID {created.Id}");
        Console.WriteLine("Press any key to return to menu");
        Console.ReadKey();
    }
}