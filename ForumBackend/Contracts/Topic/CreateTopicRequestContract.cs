using System.ComponentModel.DataAnnotations;

namespace ForumBackend.Contracts.Topic;

public class CreateTopicRequestContract
{
    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Name op topic must be between 3 and 50 characters.")]
    public required string Name { get; init; }
    
    [Required]
    public string? Description { get; init; }
}