using ApiContracts;
using ApiContracts.Post;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;


[ApiController]
[Route("[controller]")]
public class CommentController : ControllerBase
{
    private readonly ICommentRepository commentRepo;
    private readonly IPostRepository postRepo;
    private readonly IUserRepository userRepo;

    public CommentController(ICommentRepository commentRepo, IPostRepository postRepo, IUserRepository userRepo)
    {
        this.commentRepo = commentRepo;
        this.postRepo = postRepo;
        this.userRepo = userRepo;
    }
    
    [HttpPost]
    public async Task<ActionResult<CommentDto>> CreateComment(
        [FromBody] CreateCommentDto request)
    {
        var comment = new Comment(request.body, request.PostId, request.UserId);


        Comment created = await commentRepo.AddAsync(comment);

        CommentDto withUsernameDto = new()
        {
            Id = created.Id,
            Body = created.Body,
            PostId = created.PostId,
            UserId = created.UserId
        };
        
        return Created($"/comments/{withUsernameDto.Id}", created);
    }
    
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CommentDto>> UpdateComment(int id,
        [FromBody] UpdateCommentDto request)
    {
        var existing = await commentRepo.GetSingleAsync(id);
        if (existing == null)
        {
            return NotFound($"Comment with ID {id} was not found.");
            
        }

        existing.Body = request.Body;

        await commentRepo.UpdateAsync(existing);

        CommentDto dto = new()
        {
            Id = existing.Id,
            Body = existing.Body,
            PostId = existing.PostId,
            UserId = existing.UserId
        };
        return Ok(dto);
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentDto>> GetCommentById(int id)
    {
        Comment? comment = await commentRepo.GetSingleAsync(id);

        if (comment == null)
        {
            return NotFound($"Comment with ID {id} not found.");
        }

        var dto = new CommentDto
        {
            Id = comment.Id,
            Body = comment.Body,
            PostId = comment.PostId,
            UserId = comment.UserId
            
        };

        return Ok(dto);
    }
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteComment(int id)
    {
        var existing = await commentRepo.GetSingleAsync(id);
        if (existing == null)
        {
            return NotFound($"Comment with id {id} not found");
        }

        await commentRepo.DeleteAsync(id);

        return NoContent();
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetAllComments(
        [FromQuery] int? postId,
        [FromQuery] int? userId,
        [FromQuery] string? username)
    {
        IQueryable<Comment> comments = commentRepo.GetManyAsync();

        // Filter by post id
        if (postId.HasValue)
        {
            comments = comments.Where(c => c.PostId == postId.Value);
        }

        // Filter by user id
        if (userId.HasValue)
        {
            comments = comments.Where(c => c.UserId == userId.Value);
        }

        // Filter by username
        if (!string.IsNullOrWhiteSpace(username))
        {
            string lowerUsername = username.ToLower();

            var matchingUserIds = await userRepo.GetManyAsync()
                .Where(u => u.Username.ToLower().Contains(lowerUsername))
                .Select(u => u.Id)
                .ToListAsync();

            comments = comments.Where(c => matchingUserIds.Contains(c.UserId));
        }

        var dtos = await comments
            .Select(c => new CommentDto
            {
                Id = c.Id,
                Body = c.Body,
                PostId = c.PostId,
                UserId = c.UserId
            })
            .ToListAsync();

        return Ok(dtos);
    }


}