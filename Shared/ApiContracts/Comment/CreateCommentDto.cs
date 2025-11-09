namespace ApiContracts;

public record CreateCommentDto(string body, int PostId, int UserId);
