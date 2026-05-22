namespace ForumBackend.Contracts.Comments;

public class CommentResponseContract
{
    public required Guid Uid { get; init; }
    public required Guid UserUId { get; init; }
    public required string UserAvatar { get; init; }
    public required string UserName { get; init; }
    public required Guid PostUId { get; init; }
    public required string Body { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required long Likes { get; init; }
    public required bool WasDeleted { get; init; }
}