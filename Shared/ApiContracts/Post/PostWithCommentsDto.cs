namespace ApiContracts.Post;

public record PostWithCommentsDto(PostDto Post, List<CommentDto> Comments);
