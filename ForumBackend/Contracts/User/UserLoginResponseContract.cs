using Microsoft.AspNetCore.Identity;

namespace ForumBackend.Contracts.User;

public class UserLoginResponseContract
{
    public required Guid Uid { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public string Description { get; init; } = "";
    public string AvatarUrl { get; init; } = "";
    public required DateOnly CreatedAt { get; init; }
    public required long FollowersCount { get; init; }
    public required PasswordVerificationResult PwdVerificationResult { get; init; }
}