using ForumBackend.Contracts.Topic;
using ForumBackend.Ef;
using ForumBackend.Exceptions.Topic;
using Microsoft.EntityFrameworkCore;

namespace ForumBackend.Services.Topic;

public class TopicService(ForumDbContext dbContext, ILogger<TopicService> logger) : ITopicService
{
    public async Task<IReadOnlyCollection<TopicResponseContract>> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Start getting all topics");

        var topics = await dbContext.Topics.Select(x => new TopicResponseContract
        {
            UId = x.Uid,
            Title = x.Name,
            Description = x.Description,
        }).ToArrayAsync(cancellationToken);

        logger.LogInformation("End getting all topics. Count: {Count}", topics.Length);

        return topics;
    }

    public async Task<TopicResponseContract?> GetByUidAsync(Guid uid, CancellationToken cancellationToken)
    {
        logger.LogInformation("Start getting topic by uid: {Uid}", uid);

        var topic = await dbContext.Topics.Select(x => new TopicResponseContract
            {
                UId = x.Uid,
                Title = x.Name,
                Description = x.Description,
            })
            .Where(x => x.UId == uid)
            .SingleOrDefaultAsync(cancellationToken);

        if (topic == null)
        {
            logger.LogWarning("Topic with uid {uid} not found", uid);
            throw new TopicNotFoundException($"Topic with uid {uid} not found");
        }

        logger.LogInformation("End getting topic by uid: {Uid}", uid);

        return topic;
    }

    public async Task<TopicResponseContract> CreateAsync(CreateTopicRequestContract request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Start creating topic");

        var topic = new Ef.Entities.Topic
        {
            Name = request.Name,
            Description = request.Description,
        };

        await dbContext.Topics.AddAsync(topic, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("End creating topic. Topic: {TopicUId}", topic.Uid);

        var response = new TopicResponseContract
        {
            UId = topic.Uid,
            Title = topic.Name,
            Description = topic.Description,
        };

        logger.LogInformation("End creating topic. Topic: {TopicUId}. response was created", topic.Uid);

        return response;
    }

    public async Task<TopicResponseContract?> UpdateAsync(Guid uid, UpdateTopicRequestContract request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Start updating topic with uid: {Uid}", uid);

        var topic = await dbContext.Topics.SingleOrDefaultAsync(x => x.Uid == uid, cancellationToken);

        if (topic == null)
        {
            logger.LogWarning("Topic with uid {uid} not found", uid);
            throw new TopicNotFoundException($"Topic with uid {uid} not found");
        }

        topic.Description = request.Description;

        dbContext.Topics.Update(topic);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("End updating topic. Topic: {TopicUId}", uid);

        var response = new TopicResponseContract
        {
            UId = topic.Uid,
            Title = topic.Name,
            Description = topic.Description,
        };

        logger.LogInformation("End updating topic. Topic: {TopicUId}. Response was created", uid);

        return response;
    }

    public async Task<bool> DeleteAsync(Guid uid, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting deleting topic with uid: {Uid}", uid);

        var topic = await dbContext.Topics.SingleOrDefaultAsync(x => x.Uid == uid, cancellationToken);

        if (topic == null)
        {
            logger.LogWarning("Topic with uid {uid} not found", uid);
            return false;
        }

        topic.Posts.Clear();
        topic.Users.Clear();
        
        dbContext.Topics.Remove(topic);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Deleted was ended");

        return true;
    }
}