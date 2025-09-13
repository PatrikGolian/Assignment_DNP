using CLI.UI.ManageComments;
using CLI.UI.ManagePosts;
using CLI.UI.ManageUsers;

namespace CLI.UI;

using CLI.UI;
using InMemoryRepositories;
using RepositoryContracts;
public class CliApp
{
     private readonly IUserRepository userRepo;
    private readonly ICommentRepository commentRepo;
    private readonly IPostRepository postRepo;

    public CliApp(IUserRepository userRepo,
                  ICommentRepository commentRepo,
                  IPostRepository postRepo)
    {
        this.userRepo = userRepo;
        this.commentRepo = commentRepo;
        this.postRepo = postRepo;

    }

    public async Task StartAsync()
    {
        bool running = true;
        while (running)
        {
            Console.Clear();
            Console.WriteLine("=== LEGO CLI ===");
            Console.WriteLine("1. Create user");
            Console.WriteLine("2. List users");
            Console.WriteLine("3. Create post");
            Console.WriteLine("4. View posts overview");
            Console.WriteLine("5. View specific post");
            Console.WriteLine("6. Add comment to post");
            Console.WriteLine("0. Exit");
            Console.Write("> ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await new CreateUserView(userRepo).ShowAsync();
                    break;
                case "2":
                    await new ListUsersView(userRepo).ShowAsync();
                    break;
                case "3":
                    await new CreatePostView(postRepo).ShowAsync();
                    break;
                case "4":
                    await new ListPostsView(postRepo).ShowAsync();
                    break;
                case "5":
                    await new SinglePostView(postRepo, commentRepo).ShowAsync();
                    break;
                case "6":
                    await new CreateCommentView(commentRepo).ShowAsync();
                    break;
                case "0":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Unknown choice");
                    Console.ReadKey();
                    break;
            }
        }
    }
}