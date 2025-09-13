using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class SinglePostView
{
    private readonly IPostRepository postRepo;
    private readonly ICommentRepository commentRepo;

    public SinglePostView(IPostRepository postRepo, ICommentRepository commentRepo)
    {
        this.postRepo = postRepo;
        this.commentRepo = commentRepo;
    }

    public async Task ShowAsync()
    {
        Console.Clear();
        Console.WriteLine("=== View Specific Post ===");

        Console.Write("Enter Post Id: ");
        string? input = Console.ReadLine();
        if (!int.TryParse(input, out int postId))
        {
            Console.WriteLine("Invalid Post Id.");
            Console.ReadKey();
            return;
        }

        // Get the post
        Post post;
        try
        {
            post = await postRepo.GetSingleAsync(postId);
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"Post with Id {postId} not found.");
            Console.ReadKey();
            return;
        }

        Console.Clear();
        Console.WriteLine($"=== Post {post.Id} ===");
        Console.WriteLine($"Title: {post.Title}");
        Console.WriteLine($"Body:  {post.Body}");
        Console.WriteLine();

        // Get all comments for this post
        var comments = commentRepo.GetManyAsync()
            .Where(c => c.PostId == post.Id);

        Console.WriteLine("Comments:");
        if (!comments.Any())
        {
            Console.WriteLine(" (no comments yet)");
        }
        else
        {
            foreach (var c in comments)
            {
                Console.WriteLine($"- [{c.UserId}] {c.Body} (Comment Id: {c.Id})");
            }
        }

        Console.WriteLine("\nPress any key to return to menu");
        Console.ReadKey();
    }
}