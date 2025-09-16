using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class CreateUserView
{
    private readonly IUserRepository userRepo;

    public CreateUserView(IUserRepository userRepo)
    {
        this.userRepo = userRepo;
    }

    public async Task CreateUserAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Create User ===");
        Console.Write("Username: ");
        string? username = Console.ReadLine();
        Console.Write("Password: ");
        string? password = Console.ReadLine();

        var user = new User { Username = username!, Password = password! };
        User created = await userRepo.AddAsync(user);

        Console.WriteLine($"User created with ID {created.Id}");
        Console.WriteLine("Press any key to return to menu");
        Console.ReadKey();
    }
}