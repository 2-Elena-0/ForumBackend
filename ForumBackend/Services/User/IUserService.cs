using ForumBackend.Contracts.Posts;
using ForumBackend.Contracts.User;

namespace ForumBackend.Services.User;

public interface IUserService
{
    Task<IReadOnlyCollection<UserResponseContract>> GetAllAsync(CancellationToken cancellationToken);

    Task<UserResponseContract?> GetByUidAsync(Guid uid, CancellationToken cancellationToken);

    Task<UserLoginResponseContract?> LoginByEmailAsync(UserLoginContract request, CancellationToken cancellationToken);

    Task<UserResponseContract> CreateAsync(CreateUserRequestContract request, CancellationToken cancellationToken);

    Task<UserResponseContract?> UpdateAsync(Guid uid, UpdateUserRequestContract request,
        CancellationToken cancellationToken);

    Task<UserResponseContract?> AddLikePostAsync(Guid userUid, Guid postUid, CancellationToken cancellationToken);

    Task<UserResponseContract?> AddFavoritePostAsync(Guid userUid, Guid postUid, CancellationToken cancellationToken);
    
    Task<UserResponseContract?> RemoveLikePostAsync(Guid userUid, Guid postUid, CancellationToken cancellationToken);
    
    Task<UserResponseContract?> RemoveFavoritePostAsync(Guid userUid, Guid postUid, CancellationToken cancellationToken);
    
    Task<UserResponseContract?> AddInterestingTopic(Guid userUid, Guid topicUid, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid uid, CancellationToken cancellationToken);
    
    Task<IReadOnlyCollection<string>> GetLikePostUids(Guid uid, CancellationToken cancellationToken);
    
    Task<IReadOnlyCollection<string>> GetFavoritePostUids(Guid uid, CancellationToken cancellationToken);
    
    Task<IReadOnlyCollection<PostResponseContract>> GetLikePostFullUids(Guid uid, CancellationToken cancellationToken);
    
    Task<IReadOnlyCollection<PostResponseContract>> GetFavoritePostFullUids(Guid uid, CancellationToken cancellationToken);
}