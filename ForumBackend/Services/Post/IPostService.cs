using ForumBackend.Contracts.Posts;

namespace ForumBackend.Services.Post;

public interface IPostService
{
    Task<IReadOnlyCollection<PostResponseContract>> GetAllAsync(CancellationToken cancellationToken);
    
    Task<IReadOnlyCollection<PostResponseContract>> GetAllByUserUidAsync(Guid userUid, CancellationToken cancellationToken);

    Task<PostResponseContract?> GetByUidAsync(Guid uid, CancellationToken cancellationToken);

    Task<PostResponseContract> CreateAsync(CreatePostRequestContract request, CancellationToken cancellationToken);

    Task<PostResponseContract?> UpdateAsync(Guid uid, UpdatePostRequestContract request, CancellationToken cancellationToken);

    Task<PostResponseContract?> AddTopicToPostAsync(Guid postUid, Guid topicUid, UpdatePostRequestContract request, CancellationToken cancellationToken);
    
    Task<bool> DeleteAsync(Guid uid, CancellationToken cancellationToken);
}