using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageComments;

public class CreateCommentView
{
    private readonly ICommentRepository commentRepo;

    public CreateCommentView(ICommentRepository commentRepo)
    {
        this.commentRepo = commentRepo;
    }

    public async Task ShowAsync()
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