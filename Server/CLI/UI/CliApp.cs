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
            Console.WriteLine("=== Starting CLI ===");
            Console.WriteLine("=== Please choose your action ===");
            Console.WriteLine("1. Create user");
            Console.WriteLine("2. List users");
            Console.WriteLine("3. Create post");
            Console.WriteLine("4. View posts overview");
            Console.WriteLine("5. View specific post");
            Console.WriteLine("6. Add comment to post");
            Console.WriteLine("0. Exit");
            
            
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await new CreateUserView(userRepo).CreateUserAsync();
                    break;
                case "2":
                    await new ListUsersView(userRepo).UsersListAsync();
                    break;
                case "3":
                    await new CreatePostView(postRepo).CreatePostAsync();
                    break;
                case "4":
                    await new ListPostsView(postRepo).PostsListAsync();
                    break;
                case "5":
                    await new SinglePostView(postRepo, commentRepo).SinglePostAsync();
                    break;
                case "6":
                    await new CreateCommentView(commentRepo, postRepo, userRepo).CreateCommentAsync();
                    break;
                case "7":
                    await new FilterUsersView(userRepo).FilteredUsersAsync();
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