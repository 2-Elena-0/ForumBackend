using ForumBackend.Contracts.User;

namespace ForumBackend.Services.User;

public interface IUserService
{
    Task<IReadOnlyCollection<UserResponseContract>> GetAllAsync(CancellationToken cancellationToken);

    Task<UserResponseContract?> GetByUidAsync(Guid uid, CancellationToken cancellationToken);
    
    Task<UserLoginResponseContract?> LoginByEmailAsync(UserLoginContract request, CancellationToken cancellationToken);

    Task<UserResponseContract> CreateAsync(CreateUserRequestContract request, CancellationToken cancellationToken);

    Task<UserResponseContract?> UpdateAsync(Guid uid, UpdateUserRequestContract request, CancellationToken cancellationToken);
    
    Task<UserResponseContract?> AddLikePostAsync(Guid userUid, Guid postUid, CancellationToken cancellationToken);
    
    Task<UserResponseContract?> AddFavoritePostAsync(Guid userUid, Guid postUid, CancellationToken cancellationToken);
    
    Task<UserResponseContract?> AddFollowAsync(Guid userFollowerUid, Guid followUid, CancellationToken cancellationToken);
    
    Task<UserResponseContract?> AddInterestingTopic(Guid userUid, Guid topicUid, CancellationToken cancellationToken);

    
    Task<bool> DeleteAsync(Guid uid, CancellationToken cancellationToken);
    
    Task<bool> CheckName(string name, CancellationToken cancellationToken);
    
    Task<bool> CheckEmail(string email, CancellationToken cancellationToken);
    
    Task<string?> GetAvatarByUidAsync(Guid uid, CancellationToken cancellationToken);
}