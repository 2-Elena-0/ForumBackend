using ForumBackend.Contracts.Posts;
using ForumBackend.Contracts.Topic;

namespace ForumBackend.Services.Topic;

public interface ITopicService
{
    Task<IReadOnlyCollection<TopicResponseContract>> GetAllAsync(CancellationToken cancellationToken);
    
    Task<TopicResponseContract?> GetByUidAsync(Guid uid, CancellationToken cancellationToken);

    Task<TopicResponseContract> CreateAsync(CreateTopicRequestContract request, CancellationToken cancellationToken);

    Task<TopicResponseContract?> UpdateAsync(Guid uid, UpdateTopicRequestContract request, CancellationToken cancellationToken);
    
    Task<IReadOnlyCollection<TopicResponseContract>> AddTopicToPostAsync(Guid postUid, Guid topicUid, CancellationToken cancellationToken);
    
    Task<IReadOnlyCollection<TopicResponseContract>> GetPostTopicsAsync(Guid uid, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid uid, CancellationToken cancellationToken);
}