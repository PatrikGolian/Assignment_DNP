using ApiContracts;
using ApiContracts.Post;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository postRepo;
    private readonly IUserRepository userRepo;
    private readonly ICommentRepository commentRepo;

    public PostsController(IPostRepository postRepo, IUserRepository userRepo, ICommentRepository commentRepo)
    {
        this.postRepo = postRepo;
        this.userRepo = userRepo;
        this.commentRepo = commentRepo;
    }
    
    [HttpPost]
    public async Task<ActionResult<PostDto>> CreatePost(
        [FromBody] CreatePostDto request)
    {
        var post = new Post
        {
            Title = request.Title, Body = request.Body, UserId = request.UserId
        };

        Post created = await postRepo.AddAsync(post);

        PostDto dto = new()
        {
            Id = created.Id,
            Title = created.Title,
            Body = created.Body,
            UserId = created.UserId
        };
        
        return Created($"/posts/{dto.Id}", created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PostDto>> UpdatePost(int id,
        [FromBody] UpdatePostDto request)
    {
        var existing = await postRepo.GetSingleAsync(id);
        if (existing == null)
        {
            return NotFound($"Post with ID {id} was not found.");
            
        }

        existing.Title = request.Title;
        existing.Body = request.Body;

        await postRepo.UpdateAsync(existing);

        PostDto dto = new()
        {
            Id = existing.Id,
            Title = existing.Title,
            Body = existing.Body,
            UserId = existing.UserId
        };
        return Ok(dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDto>> GetPostById(int id)
    {
        Post? post = await postRepo.GetSingleAsync(id);
        if (post == null)
        {
            return NotFound($"Post with ID {id} was not found.");
            
        }

        var author = await userRepo.GetSingleAsync(post.UserId);

        var comments = commentRepo.GetManyAsync().Where(c => c.PostId == id)
            .Select(c => new CommentWithUsernameDto()
            {
                Id = c.Id,
                Body = c.Body,
                UserId = c.UserId,
                UserName = userRepo.GetSingleAsync(c.UserId).Result.Username,
                PostId = c.PostId
            }).ToList();

        PostWithCommentsDto dto = new()
        {
            Id = post.Id,
            Title = post.Title,
            Body = post.Body,
            UserId = post.UserId,
            UserName = author.Username,
            Comments = comments
        };
        return Ok(dto);
    }
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeletePost(int id)
    {
        var existing = await postRepo.GetSingleAsync(id);
        if (existing == null)
        {
            return NotFound($"Post with id {id} not found");
        }

        await postRepo.DeleteAsync(id);

        return NoContent();
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<PostDto>> GetAllPosts(
        [FromQuery] string? title,
        [FromQuery] int? userId,
        [FromQuery] string? username)
    {
        var posts = postRepo.GetManyAsync();

        // by title 
        if (!string.IsNullOrWhiteSpace(title))
        {
            posts = posts.Where(p =>
                p.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
        }

        // by user id
        if (userId.HasValue)
        {
            posts = posts.Where(p => p.UserId == userId.Value);
        }

        //  by username
        if (!string.IsNullOrWhiteSpace(username))
        {
            // Need access to users to check names
            var users = userRepo.GetManyAsync()
                .Where(u => u.Username.Contains(username, StringComparison.OrdinalIgnoreCase))
                .Select(u => u.Id)
                .ToList();

            posts = posts.Where(p => users.Contains(p.UserId));
        }

        var dtos = posts.Select(p => new PostDto
        {
            Id = p.Id,
            Title = p.Title,
            Body = p.Body,
            UserId = p.UserId
        }).ToList();

        return Ok(dtos);
    }
}