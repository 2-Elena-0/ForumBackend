using System.ComponentModel.DataAnnotations;

namespace ForumBackend.Contracts.Posts;

public class UpdatePostRequestContract
{
    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
    [RegularExpression(@"^[A-Za-z][A-Za-z0-9_]{2,49}$", ErrorMessage = "Username must start with a letter and contain only letters, numbers, and underscores.")]
    public required string Name { get; init; }
    
    [Required]
    public required string Body { get; init; }
    
    [Required]
    public required bool UserDeleted { get; init; }
}