using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageComments;

public class CreateCommentView
{
    private readonly ICommentRepository commentRepo;
    private readonly IPostRepository postRepo;
    private readonly IUserRepository userRepo;

    public CreateCommentView(
        ICommentRepository commentRepo,
        IPostRepository postRepo,
        IUserRepository userRepo)
    {
        this.commentRepo = commentRepo;
        this.postRepo = postRepo;
        this.userRepo = userRepo;
    }

    public async Task CreateCommentAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Add Comment to Post ===");

        Console.Write("Post Id: ");
        string? postIdInput = Console.ReadLine();
        int postId = int.TryParse(postIdInput, out var pid) ? pid : 0;

        Console.Write("User Id: ");
        string? userIdInput = Console.ReadLine();
        int userId = int.TryParse(userIdInput, out var uid) ? uid : 0;

        Console.Write("Comment text: ");
        string? body = Console.ReadLine();

        // ✅ Check that the post exists
        bool postExists = postRepo.GetManyAsync().Any(p => p.Id == postId);
        if (!postExists)
        {
            Console.WriteLine($"No post found with Id {postId}.");
            Console.WriteLine("Press any key to return to menu");
            Console.ReadKey();
            return;
        }

        bool userExists = userRepo.GetManyAsync().Any(u => u.Id == userId);
        if (!userExists)
        {
            Console.WriteLine($"No user found with Id {userId}.");
            Console.WriteLine("Press any key to return to menu");
            Console.ReadKey();
            return;
        }

        var comment = new Comment
        {
            Body = body ?? string.Empty,
            PostId = postId,
            UserId = userId
        };

        Comment created = await commentRepo.AddAsync(comment);

        Console.WriteLine($"Comment created with ID {created.Id}");
        Console.WriteLine("Press any key to return to menu");
        Console.ReadKey();
    }
}
