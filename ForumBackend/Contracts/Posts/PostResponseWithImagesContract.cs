namespace ForumBackend.Contracts.Posts;

public class PostResponseWithImagesContract
{
    public required Guid Uid { get; init; }
    
    public required Guid UserUId { get; init; }
    
    public required string Name { get; init; }
    
    public required string Body { get; init; }
    
    public required DateTimeOffset CreatedAt { get; init; }
    
    public required long Favorites { get; init; }
    
    public required long Likes { get; init; }
    
    public required ICollection<string> Images { get; init; } = new List<string>();
    
}