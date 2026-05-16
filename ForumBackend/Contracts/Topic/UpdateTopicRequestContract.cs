using System.ComponentModel.DataAnnotations;

namespace ForumBackend.Contracts.Topic;

public class UpdateTopicRequestContract
{
    [Required] 
    public required string Description { get; init; }
}