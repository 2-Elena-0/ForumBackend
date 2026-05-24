using ForumBackend.Contracts.Posts;
using ForumBackend.Contracts.Topic;
using ForumBackend.Ef;
using ForumBackend.Exceptions.Topic;
using ForumBackend.Exceptions.User;
using ForumBackend.Filters.Post;
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

    public async Task<IReadOnlyCollection<TopicResponseContract>> AddTopicToPostAsync(
        Guid postUid, Guid topicUid, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting add topic {topicUid} to post {postUid}", topicUid, postUid);

        var post = dbContext.Posts.Include(x => x.Topics).SingleOrDefault(x => x.Uid == postUid);
        if (post == null)
        {
            logger.LogWarning("Post with uId {Uid} not found", postUid);
            throw new PostNotFoundException($"Post with uId {postUid} not found");
        }

        var topic = dbContext.Topics.SingleOrDefault(x => x.Uid == topicUid);
        if (topic == null)
        {
            logger.LogWarning("Topic with uId {Uid} not found", topicUid);
            throw new TopicNotFoundException($"Topic with uId {topicUid} not found");
        }

        if (!post.Topics.Contains(topic))
        {
            post.Topics.Add(topic);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        logger.LogInformation("Finished add topic: {Topic}. Response created.", topicUid);

        var response = new List<TopicResponseContract>();
        foreach (var postTopic in post.Topics)
        {
            response.Add(new TopicResponseContract
            {
                UId = postTopic.Uid,
                Title = postTopic.Name,
                Description = postTopic.Description,
            });
        }

        logger.LogInformation("Finished add topic. Topic count: {Topic}. Response created.", response.Count);

        return response;
    }

    public async Task<IReadOnlyCollection<TopicResponseContract>> GetPostTopicsAsync(Guid uid,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting get all topic of post with uid {}", uid);

        var post = dbContext.Posts.Include(x => x.Topics).SingleOrDefault(x => x.Uid == uid);

        if (post == null)
        {
            logger.LogWarning("Post with {uId} not found", uid);
            throw new PostNotFoundException($"Post with uId {uid} not found");
        }

        var topics = new List<TopicResponseContract>();
        foreach (var postTopic in post.Topics)
        {
            topics.Add(new TopicResponseContract
            {
                UId = postTopic.Uid,
                Title = postTopic.Name,
                Description = postTopic.Description,
            });
        }

        logger.LogInformation("Ending search topics. Topic count: {}", topics.Count);

        return topics;
    }
    
    public async Task<IReadOnlyCollection<TopicResponseContract>> GetUserTopicsAsync(Guid uid,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting get all topic of post with uid {}", uid);

        var user = dbContext.Users.Include(x => x.Topics).SingleOrDefault(x => x.Uid == uid);

        if (user == null)
        {
            logger.LogWarning("User with {uId} not found", uid);
            throw new UserNotFoundException($"Post with uId {uid} not found");
        }

        var topics = new List<TopicResponseContract>();
        foreach (var userTopic in user.Topics)
        {
            topics.Add(new TopicResponseContract
            {
                UId = userTopic.Uid,
                Title = userTopic.Name,
                Description = userTopic.Description,
            });
        }

        logger.LogInformation("Ending search topics. Topic count: {}", topics.Count);

        return topics;
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