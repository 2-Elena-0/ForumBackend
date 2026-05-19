using System.ComponentModel.DataAnnotations;

namespace ForumBackend.Contracts.Posts;

public class CreatePostRequestContract
{
    [Required]
    public required Guid UserUId { get; init; }
    
    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Post must be between 3 and 50 characters.")]
    public required string Name { get; init; }
    
    [Required]
    public required string Body { get; init; }
}