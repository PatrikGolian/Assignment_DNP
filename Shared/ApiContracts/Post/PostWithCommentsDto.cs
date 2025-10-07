namespace ApiContracts.Post;

public class PostWithCommentsDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } // author username

    public List<CommentWithUsernameDto> Comments { get; set; } = new();
}