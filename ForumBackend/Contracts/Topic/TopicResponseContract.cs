namespace ForumBackend.Contracts.Topic;

public class TopicResponseContract
{
    public required Guid UId  { get; set; }
    public required string Title { get; set; }
    public required string? Description { get; set; }
}