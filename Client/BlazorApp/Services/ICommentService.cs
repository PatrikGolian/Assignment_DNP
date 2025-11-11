using ApiContracts;

namespace BlazorApp.Services;

public interface ICommentService
{
    public Task<CommentDto> AddCommentAsync(CreateCommentDto request); 
    public Task<List<CommentDto>?> GetCommentsAsync();


}