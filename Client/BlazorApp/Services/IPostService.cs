using ApiContracts.Post;

namespace BlazorApp.Services;

public interface IPostService
{
    public Task<PostDto> AddPostAsync(CreatePostDto request);
    public Task<PostDto?> GetPostAsync(int id);
    public Task<List<PostDto>?> GetPostsAsync();



}