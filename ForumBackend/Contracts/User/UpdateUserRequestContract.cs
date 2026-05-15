using System.ComponentModel.DataAnnotations;

namespace ForumBackend.Contracts.User;

public class UpdateUserRequestContract
{
    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
    [RegularExpression(@"^[A-Za-z][A-Za-z0-9_]{2,49}$", ErrorMessage = "Username must start with a letter and contain only letters, numbers, and underscores.")]
    public required string Name { get; init; }

    [Required]
    public required string Description { get; init; }

    [Required]
    [StringLength(255, MinimumLength = 3, ErrorMessage = "Avatar url must be between 3 and 255 characters.")]
    public required string AvatarUrl { get; init; }

    [Required]
    [StringLength(255, MinimumLength = 3, ErrorMessage = "Avatar url must be between 3 and 255 characters.")]
    public string Role { get; init; } = "standard";
    
    public required DateOnly RoleDate { get; init; }
    
}