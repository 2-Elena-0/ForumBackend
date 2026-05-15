using System.ComponentModel.DataAnnotations;

namespace ForumBackend.Contracts.User;

public class CreateUserRequestContract
{
    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
    [RegularExpression(@"^[A-Za-z][A-Za-z0-9_]{2,49}$", ErrorMessage = "Username must start with a letter and contain only letters, numbers, and underscores.")]
    public required string Name { get; init; }
    
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public required string Email { get; init; }

    [Required]
    [StringLength(255, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
    public required string Password { get; init; }

    [Required]
    [StringLength(255, MinimumLength = 3, ErrorMessage = "Avatar url must be between 3 and 255 characters.")]
    public string Role { get; init; } = "standard";
}