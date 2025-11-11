using System.Text.Json;
using ApiContracts;
using ApiContracts.Post;

namespace BlazorApp.Services;

public class HttpCommentService : ICommentService
{

    private readonly HttpClient client;

    public HttpCommentService(HttpClient client)
    {
        this.client = client;
    }
    public async Task<CommentDto> AddCommentAsync(CreateCommentDto request)
    {
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("comment", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode) { throw new Exception(response); } 
        return JsonSerializer.Deserialize<CommentDto>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public Task<List<CommentDto>?> GetCommentsAsync()
    {
        return client.GetFromJsonAsync<List<CommentDto>>("comment")?? throw new InvalidOperationException("Users not found");
    }
}