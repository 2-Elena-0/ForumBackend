using ForumBackend.Contracts.Comments;

namespace ForumBackend.Services.Comment;

public interface ICommentService
{
    Task<IReadOnlyCollection<CommentResponseContract>> GetAllAsync(CancellationToken cancellationToken);
    
    Task<IReadOnlyCollection<CommentResponseContract>> GetAllByUserUidAsync(Guid userUid, CancellationToken cancellationToken);
    
    Task<IReadOnlyCollection<CommentResponseContract>> GetAllByPostUidAsync(Guid postUid, CancellationToken cancellationToken);
    
    Task<CommentResponseContract?> GetByUidAsync(Guid uid, CancellationToken cancellationToken);

    Task<CommentResponseContract> CreateAsync(CreateCommentRequestContract request, CancellationToken cancellationToken);

    Task<CommentResponseContract?> UpdateAsync(Guid uid, UpdateCommentRequestContract request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid uid, CancellationToken cancellationToken);
}