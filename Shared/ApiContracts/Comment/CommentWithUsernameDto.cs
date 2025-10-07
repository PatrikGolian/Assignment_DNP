namespace ApiContracts;

public class CommentWithUsernameDto
{
    public int Id { get; set; }
    public string Body { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } 
    public int PostId { get; set; }

}