namespace Entities;

public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public int Id { get; set; }
    
    private User() {}
    
    public User(string username, string password)
    {
        Username = username;
        Password = password;
    }
    
    
}