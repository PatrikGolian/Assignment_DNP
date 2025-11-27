using Entities;
using Microsoft.EntityFrameworkCore;

namespace EfcRepositories;

public class AppContext : DbContext
{
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<User> Users => Set<User>();
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source= C:\\Users\\patri\\RiderProjects\\Assignment_DNP\\Server\\EfcRepositories\\app.db");
        Console.WriteLine("DB PATH: " + Path.GetFullPath("app.db"));

    }
}