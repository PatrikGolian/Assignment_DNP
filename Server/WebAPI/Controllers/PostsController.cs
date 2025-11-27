using ApiContracts;
using ApiContracts.Post;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        var post = new Post(request.Title, request.Body, request.UserId);

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

    [HttpGet("{postId:int}")]
    public async Task<ActionResult<PostWithCommentsDto>> GetSinglePost(int postId)
    {
        // Get the post
        Post post = await postRepo.GetSingleAsync(postId);
        if (post == null)
            return NotFound($"Post with ID {postId} was not found.");

        // Get comments and users
        List<Comment> allComments =  commentRepo.GetManyAsync().ToList();
        List<Comment> postComments = allComments.Where(c => c.PostId == postId).ToList();
        List<User> users =  userRepo.GetManyAsync().ToList();

        // Map comments to DTOs using object initializers (match your DTO shape)
        List<CommentDto> commentDtos = new List<CommentDto>();
        foreach (Comment comment in postComments)
        {
            commentDtos.Add(new CommentDto
            {
                Id = comment.Id,
                Body = comment.Body,
                UserId = comment.UserId,
                PostId = comment.PostId
            });
        }

        // Map post to DTO using object initializer
        PostDto postDto = new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Body = post.Body,
            UserId = post.UserId
        };

        // Create PostWithCommentsDto.
        // If PostWithCommentsDto has a constructor taking (PostDto, List<CommentDto>) use that,
        // otherwise use an object initializer with properties (Post and Comments).
        PostWithCommentsDto dto = new PostWithCommentsDto(postDto, commentDtos);
        // OR if it has properties:
        // PostWithCommentsDto dto = new PostWithCommentsDto { Post = postDto, Comments = commentDtos };

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
    public async Task<ActionResult<IEnumerable<PostDto>>> GetAllPosts(
        [FromQuery] string? title,
        [FromQuery] int? userId,
        [FromQuery] string? username)
    {
        IQueryable<Post> posts = postRepo.GetManyAsync();

        // Filter by title
        if (!string.IsNullOrWhiteSpace(title))
        {
            string lowerTitle = title.ToLower();
            posts = posts.Where(p => p.Title.ToLower().Contains(lowerTitle));
        }

        // Filter by user ID
        if (userId.HasValue)
        {
            posts = posts.Where(p => p.UserId == userId.Value);
        }

        // Filter by username
        if (!string.IsNullOrWhiteSpace(username))
        {
            string lowerUsername = username.ToLower();

            // Query users whose username matches
            var matchingUserIds = await userRepo.GetManyAsync()
                .Where(u => u.Username.ToLower().Contains(lowerUsername))
                .Select(u => u.Id)
                .ToListAsync();

            posts = posts.Where(p => matchingUserIds.Contains(p.UserId));
        }

        // Convert to DTOs (await required!)
        var dtos = await posts.Select(p => new PostDto
        {
            Id = p.Id,
            Title = p.Title,
            Body = p.Body,
            UserId = p.UserId
        }).ToListAsync();

        return Ok(dtos);
    }

}