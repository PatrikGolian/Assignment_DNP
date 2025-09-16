using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class ListPostsView
{
    private readonly IPostRepository postRepo;

    public ListPostsView(IPostRepository postRepo)
    {
        this.postRepo = postRepo;
    }

    public async Task PostsListAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Posts Overview ===");

        // Get all posts (IQueryable<Post>)
        var posts = postRepo.GetManyAsync();

        if (!posts.Any())
        {
            Console.WriteLine("No posts found.");
        }
        else
        {
            foreach (var p in posts)
            {
                Console.WriteLine($"{p.Id}: {p.Title}");
            }
        }

        Console.WriteLine("\nPress any key to return to menu");
        Console.ReadKey();
    }
}