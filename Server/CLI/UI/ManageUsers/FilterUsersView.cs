using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class FilterUsersView
{
    private readonly IUserRepository userRepo;

    public FilterUsersView(IUserRepository userRepo)
    {
        this.userRepo = userRepo;
    }

    public async Task FilteredUsersAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Find Users by Keyword ===");
        Console.Write("Enter part of username to search for: ");
        string? keyword = Console.ReadLine();

        var users = userRepo.GetManyAsync();

        
        var filtered = string.IsNullOrWhiteSpace(keyword)
            ? users
            : users.Where(u => u.Username.Contains(keyword!, StringComparison.OrdinalIgnoreCase));

        if (!filtered.Any())
        {
            Console.WriteLine("No users match your search.");
        }
        else
        {
            Console.WriteLine("Matching users:");
            foreach (var u in filtered)
            {
                Console.WriteLine($"{u.Id}: {u.Username}");
            }
        }

        Console.WriteLine("\nPress any key to return to menu");
        Console.ReadKey();
    }
}

