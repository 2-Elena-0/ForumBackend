using ForumBackend.Contracts.Posts;
using ForumBackend.Contracts.Topic;

namespace ForumBackend.Services.Post;

public interface IPostService
{
    Task<IReadOnlyCollection<PostResponseContract>> GetAllAsync(CancellationToken cancellationToken);
    
    Task<IReadOnlyCollection<PostResponseContract>> GetAllByUserUidAsync(Guid userUid, CancellationToken cancellationToken);

    Task<PostResponseWithImagesContract?> GetByUidAsync(Guid uid, CancellationToken cancellationToken);

    Task<PostResponseWithImagesContract> CreateAsync(CreatePostRequestContract request, CancellationToken cancellationToken);

    Task<PostResponseWithImagesContract?> UpdateAsync(Guid uid, UpdatePostRequestContract request, CancellationToken cancellationToken);
    
    Task<bool> DeleteAsync(Guid uid, CancellationToken cancellationToken);
}