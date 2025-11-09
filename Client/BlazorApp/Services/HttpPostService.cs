using System.Text.Json;
using ApiContracts.Post;

namespace BlazorApp.Services;

public class HttpPostService : IPostService
{
    private readonly HttpClient client;
    
    public HttpPostService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<PostDto> AddPostAsync(CreatePostDto request)
    {
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("posts", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode) { throw new Exception(response); } 
        return JsonSerializer.Deserialize<PostDto>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        
    }
    
    public async Task<PostDto?> GetPostAsync(int id)
    {
        var response = await client.GetAsync($"posts{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PostDto>() ?? throw new InvalidOperationException("User not found");
    }

    public Task<List<PostDto>?> GetPostsAsync()
    {
        return client.GetFromJsonAsync<List<PostDto>>("posts")?? throw new InvalidOperationException("Users not found");
    }
}