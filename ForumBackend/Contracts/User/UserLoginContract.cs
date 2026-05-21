using System.ComponentModel.DataAnnotations;

namespace ForumBackend.Contracts.User;

public class UserLoginContract
{
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public required string Email { get; init; }

    [Required]
    [StringLength(255, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
    public required string Password { get; init; }
}